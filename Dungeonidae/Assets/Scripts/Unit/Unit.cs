using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;
using Unity.Collections;
using Unity.Jobs;

[System.Serializable]
abstract public class Unit : MonoBehaviour
{
    protected DungeonManager dm;

    public bool Controllable { get; protected set; } = false;
    public bool IsDead { get; private set; } = false;

    readonly float walkDelay = 0.2f;
    public List<Coordinate> TilesInSight { get; protected set; } = new();
    protected bool isFollowingPath = false;
    protected Stack<Directions> path;
    [field:SerializeField] public List<Unit> UnitsInSight { get; private set; } = new();
    public bool FoundSomething { get; protected set; } = false;
    [System.NonSerialized] public bool isAnimationFinished = true;
    [System.NonSerialized] public bool isHitFinished = true;

    [field: SerializeField] public  UnitBase UnitBase { get; private set; }
    public UnitData UnitData { get; private set; }
    public SpriteRenderer MySpriteRenderer { get; private set; }
    Material spriteMaterial;
    public Animator MyAnimator { get; private set; }
    [field: SerializeField] public Animator EffectAnimator { get; private set; }
    SpriteRenderer effectRenderer;
    public Canvas canvas;
    [SerializeField] Image hpBarBg;
    [SerializeField] Image hpBar;
    [SerializeField] Image mpBarBg;
    [SerializeField] Image mpBar;
    [SerializeField] Image lvBg;
    [SerializeField] TMP_Text lvText;
    [SerializeField] Image bubble;
    [SerializeField] TMP_Text bubbleText;

    [SerializeField] SpriteRenderer shadow;
    [field:SerializeField] public Projectile BasicProjectilePrefab { get; private set; }

    protected ItemEffectDirector itemEffectDirector;
    public AbilityDirector abilityDirector { get; private set; }
    public BuffDirector BuffDirector { get; private set; }

    public List<Coordinate> AvailableRange { get; private set; } = new();
    public List<Coordinate> UnavailableRange { get; private set; } = new();
    [field: SerializeField] public SkillBase BasicAttack { get; private set; }

    Tweener moveTween;

    RaycastHit2D[] raycastResult = new RaycastHit2D[50];
    protected virtual void Awake()
    {
        MySpriteRenderer = GetComponent<SpriteRenderer>();
        spriteMaterial = MySpriteRenderer.material;
        MyAnimator = GetComponent<Animator>();
        effectRenderer = EffectAnimator.GetComponent<SpriteRenderer>();
        //map = GameManager.Instance.dungeonManager.map;
        UnitData = new();
        itemEffectDirector = new ItemEffectDirector(this);
        abilityDirector = new AbilityDirector(this);
        BuffDirector = new BuffDirector(this);
    }

    protected virtual void Start()
    {
        UpdateHpBar();
        UnitData.OnHpValueChange += new UnitData.EventHandler(UpdateHpBar);
        UpdateMpBar();
        UnitData.OnMpValueChange += new UnitData.EventHandler(UpdateMpBar);
        if (MySpriteRenderer.enabled)
        {
            MySpriteRenderer.color = Constants.Transparent;
            MySpriteRenderer.DOFade(1.0f, 1);
        }
    }

    public void SetUnitData(DungeonManager dungeonManager, UnitData unitData)
    {
        UnitData = unitData;
        UnitData.Init(this);
        dm = dungeonManager;
        SetPosition();
        UnitData.SetStartLevel(UnitData.level);
        lvText.text = (UnitData.level + 1).ToString();
        UnitData.OnLevelChange += LevelUp;
    }

    public virtual void Init(DungeonManager dungeonManager, Coordinate c, int level)
    {
        dm = dungeonManager;
        UnitData.coord = c;
        SetPosition();
        UnitData.Init(this);
        UnitData.SetStartLevel(level);
        if (lvText != null) lvText.text = (UnitData.level + 1).ToString();
        UnitData.OnLevelChange += LevelUp;
    }

    public void SetPosition()
    {
        transform.position = dm.map.GetElementAt(UnitData.coord.x, UnitData.coord.y).transform.position;
        dm.map.GetElementAt(UnitData.coord).unit = this;
        SetSortingOrder();
    }

    void SetSortingOrder(bool complete = true)
    {
        int baseOrder = 1000 - (10 * UnitData.coord.y);
        if (complete)
        {
            MySpriteRenderer.sortingOrder = baseOrder + (int)LayerOrder.Unit;
            if (shadow != null) shadow.sortingOrder = baseOrder;
            canvas.sortingOrder = baseOrder + 10 + (int)LayerOrder.Canvas;
            effectRenderer.sortingOrder = baseOrder + 10 + (int)LayerOrder.Canvas;
            
        }
        else
        {
            MySpriteRenderer.sortingOrder = baseOrder + (int)LayerOrder.DungeonObject;
            if (shadow != null) shadow.sortingOrder = baseOrder;
            canvas.sortingOrder = baseOrder + 10 + (int)LayerOrder.Canvas;
            effectRenderer.sortingOrder = baseOrder + 10 + (int)LayerOrder.Canvas;
        }
    }

    public virtual void Teleportation()
    {
        List<Coordinate> coords = new();
        for(int i=1; i<dm.map.arrSize.x - 1; i++)
        {
            for(int j=1; j<dm.map.arrSize.y - 1; j++)
            {
                if (dm.map.GetElementAt(i, j).IsReachableTile() && (i != UnitData.coord.x) && (j != UnitData.coord.y))
                {
                    coords.Add(new Coordinate(i, j));
                }
            }
        }
        if (coords.Count <= 0) return;
        dm.map.GetElementAt(UnitData.coord).unit = null;
        UnitData.coord = coords[Random.Range(0, coords.Count)];
        SetPosition();
        UpdateSightArea();
        CheckNewInSight();
    }

    public void StartTurn()
    {
        CheckNewInSight();
        if (bubble.gameObject.activeSelf)
        {
            if (dm.Player.TilesInSight.Contains(UnitData.coord))
                EndBubble();
        }
        if (UnitData.hpRegenCounter >= 10)
        {
            RecoverHp(UnitData.hpRegen.Total() * UnitData.hpRegenCounter / 10);
            UnitData.hpRegenCounter = UnitData.hpRegenCounter % 10;
        }
        if(UnitData.mpRegenCounter >= 10)
        {
            RecoverMp(UnitData.mpRegen.Total() * UnitData.mpRegenCounter / 10);
            UnitData.mpRegenCounter = UnitData.mpRegenCounter % 10;
        }
        if (isFollowingPath)
        {
            FollowPath();
        }
        else
        {
           DecideBehavior();
        }
    }
    protected virtual void DecideBehavior()
    {
        Controllable = true;
    }

    public void TrimTurn(int amount)
    {
        UnitData.UpdateBuffDurations(amount);
        UnitData.UpdateSkillRechargeLeft(amount);
        for(int i= UnitData.Buffs.Count-1; i>=0; i--)
        {
            if (UnitData.Buffs[i].durationLeft <= 0)
            {
                BuffDirector.RemoveBuff(UnitData.Buffs[i]);
            }
        }
        UnitData.TurnIndicator -= amount;
    }

    public virtual bool Move(Directions direction)
    {
        FlipSprite(direction);

        Coordinate dest = UnitData.coord.MovedCoordinate(direction, 1);

        if (dest.IsValidCoordForMap(dm.map))
        {
            if (dm.GetTileByCoordinate(dest).IsReachableTile())
            {
                Controllable = false;

                if (!dm.fogMap.GetElementAt(dest.x, dest.y).IsOn)
                {
                    EnableRenderers();
                }

                dm.map.GetElementAt(UnitData.coord.x, UnitData.coord.y).unit = null;
                UnitData.coord = dest;                   

                if (MySpriteRenderer.enabled)
                {
                    if ((direction == Directions.SE) || (direction == Directions.S) || (direction == Directions.SW))
                        SetSortingOrder(false);

                    isAnimationFinished = false;

                    MyAnimator.SetBool("Walk", true);

                    if (moveTween == null)
                    {
                        moveTween = transform.DOMove(dest.ToVector2(), walkDelay).SetRecyclable(true)
                        .SetEase(Ease.Linear)
                        .OnComplete(MoveAnimationEnd)
                        .SetAutoKill(false);
                    }
                    else
                    {
                        moveTween.ChangeValues(transform.position, dest.ToVector3(0), walkDelay).Restart();
                    }
                    EndMove();
                }
                else
                {
                    transform.position = dm.map.GetElementAt(dest.x, dest.y).transform.position;
                    SetSortingOrder();
                    EndMove();
                }
                return true;
            }
        }
        return false;
    }

    void MoveAnimationEnd()
    {
        if (!isFollowingPath || (path.Count <= 0))
            MyAnimator.SetBool("Walk", false);
        
        if (dm.fogMap.GetElementAt(UnitData.coord.x, UnitData.coord.y).IsOn)
        {
            DisableRenderers();
        }

        Tile tile = dm.map.GetElementAt(UnitData.coord);
        for (int i = 0; i < tile.dungeonObjects.Count; i++)
        {
            if (tile.dungeonObjects[i].IsInteractsWithCollision)
                tile.dungeonObjects[i].Activate(this);
        }

        isAnimationFinished = true;
        SetSortingOrder();
    }
    protected virtual void EndMove()
    {
        UpdateSightArea();
        CheckNewInSight();
        Tile tile = dm.map.GetElementAt(UnitData.coord.x, UnitData.coord.y);
        tile.unit = this;
        EndTurn(100m / UnitData.speed.Total());
    }


    public void UpdateCoordinateFromTransform()
    {
        UnitData.coord.Set((int)transform.position.x, (int)transform.position.y);
        dm.GetTileByCoordinate(UnitData.coord).unit = this;
        UpdateSightArea();
    }

    public bool IsTargetable(Tile tile)
    {
        return false;
    }

    public bool IsHostileUnit(Unit target)
    {
        switch (UnitData.team)
        {
            case Team.Neutral:
                return false;
            case Team.Chaotic:
                if (target.UnitData.team == Team.Neutral)
                    return false;
                else return true;
            case Team.Player:
                if ((target.UnitData.team == Team.Player) || (target.UnitData.team == Team.Neutral) || (target.UnitData.team == Team.Ally))
                    return false;
                else return true;
            case Team.Enemy:
                if ((target.UnitData.team == Team.Enemy) || (target.UnitData.team == Team.Neutral))
                    return false;
                else return true;
            case Team.Enemy2:
                if ((target.UnitData.team == Team.Enemy2) || (target.UnitData.team == Team.Neutral))
                    return false;
                else return true;
            default:
                throw new System.NotImplementedException();
        }
    }

    public virtual void GetDamage(AttackData attackData)
    {
        int hitChance = Mathf.Min((85 + 5 * (attackData.Attacker.UnitData.level - UnitData.level) + (attackData.Attacker.UnitData.acc.Total() - UnitData.eva.Total())));
        if ((Random.Range(0, 100) + 1) > hitChance)
        {
            if (MySpriteRenderer.enabled) {
                DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
                dt.SetMiss();
            }
        }
        else if (UnitData.AdditionalEffects.ContainsKey("BLOCK") && (Random.Range(0, 100) < UnitData.AdditionalEffects["BLOCK"][0]))
        {
            if (MySpriteRenderer.enabled)
            {
                DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
                dt.SetBlock();
            }
        }
        else
        {
            int attackDamage = attackData.AttackDamage - (int)(UnitData.def.Total() * (1 - attackData.Attacker.UnitData.pen.Total() * 0.01f));
            int magicAttackDamage = attackData.MagicAttackDamage - (int)(UnitData.mDef.Total() * (1 - attackData.Attacker.UnitData.mPen.Total() * 0.01f));

            if (attackDamage < 0) attackDamage = 0;
            if (magicAttackDamage < 0) magicAttackDamage = 0;
            int damage = (int)Mathf.Max(0, (attackDamage + magicAttackDamage) * ((100 - UnitData.dmgReduction.Total()) * 0.01f));
            int stolenHp = (int)(Mathf.Min(damage, UnitData.Hp) * 0.01f * attackData.Attacker.UnitData.lifeSteal.Total());
            int stolenMp = (int)(Mathf.Min(damage, UnitData.Hp) * 0.01f * attackData.Attacker.UnitData.lifeSteal.Total());

            UnitData.Hp -= damage;
            if (stolenHp > 0) attackData.Attacker.RecoverHp(stolenHp);
            if (stolenMp > 0) attackData.Attacker.RecoverMp(stolenMp);

            if (MySpriteRenderer.enabled)
            {
                if (canvas != null)
                {
                    DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
                    if (attackData.IsCritical)
                        dt.SetValue(damage, DamageType.Critical);
                    else dt.SetValue(damage, DamageType.Normal);
                }
                //MySpriteRenderer.DOColor(Color.red, 0.2f).OnComplete(() => { MySpriteRenderer.DOColor(Color.white, 0.2f); });
                spriteMaterial.DOFloat(1, "_FlashAmount", 0.2f).OnComplete(() => { spriteMaterial.DOFloat(0, "_FlashAmount", 0.2f); });
                hpBar.DOFade(0.5f, 0.2f).SetEase(Ease.OutCirc).OnComplete(() => { hpBar.DOFade(1, 0.2f).SetEase(Ease.OutCirc); });
            }
            isHitFinished = false;
        }

        if (UnitData.Hp <= 0)
        {
            StartDie();
        }
        else isHitFinished = true;
    }
    protected virtual void StartDie()
    {
        MySpriteRenderer.DOFade(0, 0.5f).OnComplete(() => { Destroy(gameObject); });
        IsDead = true;
        dm.GetTileByCoordinate(UnitData.coord).unit = null;
        if (hpBarBg != null) { hpBarBg.DOFade(0, 0.4f); }
        if (mpBar != null) { mpBar.DOFade(0, 0.4f); }
        if (mpBarBg != null) { mpBarBg.DOFade(0, 0.4f); }
        if (lvBg!=null) { lvBg.DOFade(0, 0.4f); }
        if(lvText!=null) { lvText.DOFade(0, 0.4f); }
        isHitFinished = true;
    }

    public void IncreaseExp(int amount)
    {
        if (canvas != null && MySpriteRenderer.enabled)
        {
            DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
            dt.SetExpValue(amount);
            UnitData.IncreaseExpValue(amount);
        }
    }
    public void LevelUp()
    {
        if (canvas != null && MySpriteRenderer.enabled)
        {
            DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
            dt.SetLevelUp();
        }
        if (lvText != null) lvText.text = (UnitData.level + 1).ToString();
    }

    public void RecoverHp(int amount)
    {
        amount = Mathf.Min(amount, UnitData.maxHp.Total() - UnitData.Hp);
        if (amount >= 1)
        {
            UnitData.Hp += amount;
            if (canvas != null && MySpriteRenderer.enabled)
            {
                DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
                dt.SetValue(amount, DamageType.Heal);
            }
        }
    }


    public void UpdateHpBar()
    {
        if (hpBar != null)
            hpBar.fillAmount = UnitData.Hp / (float)UnitData.maxHp.Total();
    }
    public void UpdateMpBar()
    {
        if (mpBar != null)
            mpBar.fillAmount = UnitData.Mp / (float)UnitData.maxMp.Total();
    }

    public void RecoverMp(int amount)
    {
        amount = Mathf.Min(amount, UnitData.maxMp.Total() - UnitData.Mp);
        UnitData.Mp += amount;
    }

     protected void RandomStep()
    {
        if (UnitData.speed.Total() <= 0) EndTurn(1);

        List<Directions> deck = new();
        for(int i=0; i<8; i++)
        {
            Coordinate dest = UnitData.coord.MovedCoordinate((Directions)i, 1);
            if (dest.IsValidCoordForMap(dm.map))
            {
                if(dm.GetTileByCoordinate(dest).IsReachableTile())
                    deck.Add((Directions)i);
            }
        }
        int max = deck.Count;
        if (max == 0)
            EndTurn(1);
        else
            Move(deck[Random.Range(0, max)]);
    }

    protected bool FindPath(Coordinate targetCoord)
    {
        AStar aStar = new(dm.map, UnitData.coord, targetCoord, dm.fogMap);
        if (aStar.Path.Count == 0)
            return false;
        else
        {
            path = aStar.Path;
            return true;
        }
    }
    protected virtual void FollowPath()
    {
        if (path.Count > 0)
        {
            isFollowingPath = true;
            if (Move(path.Pop()))
                return;
        }
        isFollowingPath = false;
        MyAnimator.SetBool("Walk", false);
        DecideBehavior();
    }
    protected Directions FollowTarget(Coordinate targetCoord)
    {
        AStar aStar = (UnitData.team == Team.Player) ? new(dm.map, UnitData.coord, targetCoord, dm.fogMap) : new(dm.map, UnitData.coord, targetCoord, null);
        if (aStar.Path.Count == 0)
            return Directions.NONE;
        else
            return aStar.Path.Pop();
    }

    public virtual void UpdateSightArea()
    {
        TilesInSight.Clear();
        int sight = UnitData.sight.Total();

        int wallMapSize = sight * 2 + 1;
        NativeArray<bool> wallMap = new(wallMapSize * wallMapSize, Allocator.TempJob);
        Coordinate origin = new(UnitData.coord.x - sight, UnitData.coord.y - sight);
        for (int i = 0; i < wallMapSize; i++)
        {
            for (int j = 0; j < wallMapSize; j++)
            {
                if (dm.IsValidIndexForMap(origin.x + i, origin.y + j) && dm.map.GetElementAt(origin.x + i, origin.y + j).IsBlockingSight())
                {
                    wallMap[j + i * wallMapSize] = true;
                }
                else wallMap[j + i * wallMapSize] = false;
            }
        }
        List<Coordinate> inRange = GlobalMethods.RangeByStep(UnitData.coord, sight);
        NativeArray<Coordinate> end = new(inRange.Count, Allocator.TempJob);
        NativeArray<bool> result = new(inRange.Count, Allocator.TempJob);
        int endCounter = 0;
        for (int i = 0; i < inRange.Count; i++)
        {
            if (dm.IsValidCoordForMap(inRange[i]))
            {
                if (Coordinate.InRange(UnitData.coord, inRange[i], sight + 0.5f))
                {
                    end[endCounter] = (inRange[i] - origin);
                    endCounter++;
                }
            }
        }

        SightCheckJob2 sightJob = new()
        {
            wallMap = wallMap,
            wallMapSize = wallMapSize,
            start = UnitData.coord - origin,
            end = end,
            result = result
        };

        JobHandle handle = sightJob.Schedule(endCounter, 1);
        handle.Complete();

        for(int i=0; i< endCounter; i++)
        {
            if (result[i])
            {
                TilesInSight.Add(end[i] + origin);
            }
        }

        wallMap.Dispose();
        end.Dispose();
        result.Dispose();
    }
    
    protected void CheckNewInSight()
    {
        List<Unit> detectedUnits = new();
        for (int i = 0; i < TilesInSight.Count; i++)
        {
            Tile tile = dm.GetTileByCoordinate(TilesInSight[i]);
            if ((tile.unit != null) && (tile.unit != this))
                detectedUnits.Add(tile.unit);
        }
        if (UnitsInSight.Count > 0)
        {
            for (int i = 0; i < detectedUnits.Count; i++)
            {
                if (!UnitsInSight.Contains(detectedUnits[i]))
                    FoundSomething = true;
            }
        }
        UnitsInSight = detectedUnits;
    }

    protected virtual void ResetSkillRange()
    {
        AvailableRange.Clear();
        UnavailableRange.Clear();
    }
    public virtual void EndSkill(decimal turnSpent)
    {
        EndTurn(turnSpent);
    }

    protected void EndTurn(decimal turnSpent)
    {
        FoundSomething = false;
        UnitData.TurnIndicator += turnSpent;
        Controllable = false;
        dm.EndTurn();
    }

    public void SkipTurn()
    {
        UnitData.TurnIndicator += 1;
        Controllable = false;
        ShowBubble("¡¤¡¤¡¤");
        dm.EndTurn();
    }

    public void FlipSprite(Directions direction)
    {
        float sign = Mathf.Sign(transform.localScale.x);
        if ((1 <= (int)direction) && ((int)direction <= 3))
        {
            transform.localScale = new Vector3(sign * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (canvas != null)
            {
                canvas.transform.localScale = new Vector3(sign * canvas.transform.localScale.x, canvas.transform.localScale.y, canvas.transform.localScale.z);
            }
        }
        else if ((5 <= (int)direction) && ((int)direction <= 7))
        {
            sign *= -1;
            transform.localScale = new Vector3(sign * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (canvas != null)
            {
                canvas.transform.localScale = new Vector3(sign * canvas.transform.localScale.x, canvas.transform.localScale.y, canvas.transform.localScale.z);
            }
        }
    }
    public void FlipSprite(Coordinate lookAt)
    {
        float sign = Mathf.Sign(transform.localScale.x);
        if (UnitData.coord == lookAt)
            return;
        else if ((lookAt.x - UnitData.coord.x) > 0)
        {
            transform.localScale = new Vector3(sign * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (canvas != null)
            {
                canvas.transform.localScale = new Vector3(sign * canvas.transform.localScale.x, canvas.transform.localScale.y, canvas.transform.localScale.z);
            }
        }
        else if ((lookAt.x - UnitData.coord.x) < 0)
        {
            sign *= -1;
            transform.localScale = new Vector3(sign * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            if (canvas != null)
            {
                canvas.transform.localScale = new Vector3(sign * canvas.transform.localScale.x, canvas.transform.localScale.y, canvas.transform.localScale.z);
            }
        }
    }

    protected void ShowBubble(string s)
    {
        bubbleText.text = s;
        if (bubble.gameObject.activeSelf)
        {
            bubble.DOKill();
            bubbleText.DOKill();
            bubble.color = Color.white;
            bubbleText.color = Color.black;
        }
        else
        {
            bubble.gameObject.SetActive(true);
        }
    }
    protected void EndBubble()
    {
        if (bubble.gameObject.activeSelf)
        {
            bubble.DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() => { bubble.gameObject.SetActive(false); bubble.color = Color.white; });
            bubbleText.DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() => { bubbleText.color = Color.black; });
        }
    }

    public void EnableRenderers()
    {
        MySpriteRenderer.enabled = true;
        MyAnimator.enabled = true;
        canvas.enabled = true;
        bubble.enabled = true;
        shadow.enabled = true;
    }
    public void DisableRenderers()
    {
        MySpriteRenderer.enabled = false;
        MyAnimator.enabled = false;
        canvas.enabled = false;
        bubble.enabled = false;
        shadow.enabled = false;
    }

    private void OnDestroy()
    {
        MySpriteRenderer.DOKill();
        transform.DOKill();
        moveTween.Kill();
        bubble.DOKill();
        bubbleText.DOKill();
        hpBar.DOKill();
        hpBarBg.DOKill();
        mpBar.DOKill();
        mpBarBg.DOKill();
        lvText.DOKill();
        lvBg.DOKill();
        spriteMaterial.DOKill();
    }
}
