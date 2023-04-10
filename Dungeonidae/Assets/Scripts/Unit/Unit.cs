using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[System.Serializable]
abstract public class Unit : MonoBehaviour
{
    protected DungeonManager dm;

    public bool Controllable { get; protected set; } = false;
    public bool IsDead { get; private set; } = false;

    protected List<Coordinate> tilesInSight = new();
    protected bool isFollowingPath = false;
    protected Stack<Directions> path;
    public List<Unit> UnitsInSight { get; private set; } = new();
    protected bool foundSomething = false;
    public bool isAnimationFinished;

    [SerializeField] UnitBase unitBase;
    public UnitData UnitData { get; private set; }
    public SpriteRenderer MySpriteRenderer { get; private set; }
    public Animator MyAnimator { get; private set; }
    [field: SerializeField] public Animator EffectAnimator { get; private set; }
    public Canvas canvas;
    [SerializeField] Image hpBarBg;
    public Image hpBar;
    public BasicAttack BasicAttack { get; private set; }

    protected ItemEffectDirector itemEffectDirector;
    public BuffDirector BuffDirector { get; private set; }

    public Skill skill { get; protected set; }

    protected virtual void Awake()
    {
        MySpriteRenderer = GetComponent<SpriteRenderer>();
        MyAnimator = GetComponent<Animator>();
        //map = GameManager.Instance.dungeonManager.map;
        UnitData = new(unitBase);
        UnitData.SetOwner(this);
        itemEffectDirector = new ItemEffectDirector(this);
        BuffDirector = new BuffDirector(this);
    }

    protected virtual void Start()
    {
        UpdateHpBar();
        UnitData.OnHpValueChanged += new UnitData.EventHandler(UpdateHpBar);
    }

    public void SetUnitData(DungeonManager dungeonManager, UnitData unitData)
    {
        UnitData = unitData;
        UnitData.SetOwner(this);
        dm = dungeonManager;
        BasicAttack = new BasicAttack(this, dm, null);
        SetStartPosition(unitData.coord.x, unitData.coord.y);
        UnitData.OnLevelChanged += LevelUp;
        UnitData.SetOwner(this);
    }

    public virtual void Init(DungeonManager dungeonManager, Coordinate c)
    {
        dm = dungeonManager;
        BasicAttack = new BasicAttack(this, dm, null);
        SetStartPosition(c.x, c.y);
        //UpdateSightArea();
        UnitData.OnLevelChanged += LevelUp;
        UnitData.SetOwner(this);
    }

    public void SetStartPosition(int x, int y)
    {
        transform.position = dm.Map[x, y].transform.position;
        UnitData.coord.Set(x, y);
        dm.Map[x, y].unit = this;
    }
    public void SetUnitListIndex(int i)
    {
        UnitData.unitListIndex = i;
    }

    public virtual void StartTurn()
    {
        UpdateSightArea();
        CheckNewInSight();
        if (isFollowingPath)
        {
            FollowPath();
        }
        else
        {
            Controllable = true;
        }
    }

    public void TrimTurn(int amount)
    {
        UnitData.ReduceSkillCurrentCooldowns(amount);
        UnitData.UpdateBuffDurations(amount);
        for(int i= UnitData.Buffs.Count-1; i>=0; i--)
        {
            if (UnitData.Buffs[i].durationLeft <= 0)
            {
                BuffDirector.RemoveBuff(UnitData.Buffs[i]);
            }
        }
        UnitData.turnIndicator -= amount;
    }

    public bool Move(Directions direction)
    {
        FlipSprite(direction);

        Coordinate dest = UnitData.coord.ToMovedCoordinate(direction, 1);

        if (dest.IsValidCoordForMap(dm.Map))
        {
            if (dm.GetTileByCoordinate(dest).IsReachableTile())
            {
                Controllable = false;

                if (!dm.FogMap[dest.x, dest.y].FogData.IsOn)
                {
                    MySpriteRenderer.enabled = true;
                    if (canvas != null) canvas.enabled = true;
                }

                dm.Map[UnitData.coord.x, UnitData.coord.y].unit = null;
                UnitData.coord = dest;

                if (MySpriteRenderer.enabled)
                {
                    float walkDelay = 0.15f;

                    MyAnimator.SetBool("Walk", true);
                    transform.DOMove(dest.ToVector2(), walkDelay)
                    .SetEase(Ease.Linear)
                    .OnComplete(EndMove);
                }
                else
                {
                    transform.position = dm.Map[dest.x, dest.y].transform.position;
                    EndMove();
                }
                return true;
            }
        }
        return false;
    }
    protected virtual void EndMove()
    {
        UpdateSightArea();
        if (isFollowingPath)
            MyAnimator.SetBool("Walk", false);
        if (dm.FogMap[UnitData.coord.x, UnitData.coord.y].FogData.IsOn)
        {
            MySpriteRenderer.enabled = false;
            if (canvas != null) canvas.enabled = false;
        }
        dm.Map[UnitData.coord.x, UnitData.coord.y].unit = this;
        EndTurn(100f / UnitData.speed.Total());
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
            case Team.Free:
                return true;
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

    public void EndBasicAttack()
    {
        MyAnimator.SetBool("Attack", false);
        isAnimationFinished = true;
    }

    public virtual void GetDamage(AttackData attackData)
    {
        int attackDamage = attackData.AttackDamage - UnitData.def.Total();
        int magicAttackDamage = attackData.MagicAttackDamage - UnitData.mDef.Total();

        if (attackDamage < 0) attackDamage = 0;
        if(magicAttackDamage < 0) magicAttackDamage = 0;
        int damage = attackDamage + magicAttackDamage;

        UnitData.Hp -= damage;
        if(canvas!= null)
        {
            DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
            dt.SetValue(damage, DamageType.Normal);
        }
        MySpriteRenderer.DOColor(Color.red, 0.2f).OnComplete(ToDefaultColor);
        if (UnitData.Hp <= 0)
        {
            StartDie();
        }
        else isAnimationFinished = true;
    }
    protected virtual void StartDie()
    {
        MySpriteRenderer.DOFade(0, 0.5f).OnComplete(Die);
        dm.GetTileByCoordinate(UnitData.coord).unit = null;
        if (hpBarBg != null) { hpBarBg.DOFade(0, 0.4f); }
    }
    void Die()
    {
        isAnimationFinished = true;
        IsDead = true;
        gameObject.SetActive(false);
    }
    void ToDefaultColor()
    {
        MySpriteRenderer.DOColor(Color.white, 0.2f);
    }

    public void IncreaseExp(int amount)
    {
        if (canvas != null)
        {
            DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
            dt.SetExpValue(amount);
            UnitData.IncreaseExpValue(amount);
        }
    }
    public void LevelUp()
    {
        if (canvas != null)
        {
            DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
            dt.SetLevelUp();
        }
    }

    public void RecoverHp(int amount)
    {
        amount = Mathf.Min(amount, UnitData.maxHp.Total() - UnitData.Hp);
        UnitData.Hp += amount;
        if (canvas != null)
        {
            DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
            dt.SetValue(amount, DamageType.Heal);
        }
    }


    public void UpdateHpBar()
    {
        if (hpBar != null)
            hpBar.fillAmount = UnitData.Hp / (float)UnitData.maxHp.Total();
    }

    protected void RandomStep()
    {
        List<Directions> deck = new();
        for(int i=0; i<8; i++)
        {
            Coordinate dest = UnitData.coord.ToMovedCoordinate((Directions)i, 1);
            if (dest.IsValidCoordForMap(dm.Map))
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
        AStar aStar = new(dm.Map, UnitData.coord, targetCoord, dm.FogMap);
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
        StartTurn();
    }
    protected Directions FollowTarget(Coordinate targetCoord)
    {
        AStar aStar = new(dm.Map, UnitData.coord, targetCoord, dm.FogMap);
        if (aStar.Path.Count == 0)
            return Directions.NONE;
        else
            return aStar.Path.Pop();
    }

    protected virtual void UpdateSightArea()
    {
        int northBound = Mathf.Min(UnitData.coord.y + UnitData.sight.Total(), dm.Map.GetLength(0) - 1);
        int southBound = Mathf.Max(UnitData.coord.y - UnitData.sight.Total(), 0);
        int eastBound = Mathf.Min(UnitData.coord.x + UnitData.sight.Total(), dm.Map.GetLength(1) - 1);
        int westBound = Mathf.Max(UnitData.coord.x - UnitData.sight.Total(), 0);

        tilesInSight.Clear();

        RaycastHit2D[] hit;

        for(int i=westBound; i<=eastBound; i++)
        {
            for(int j=southBound; j<=northBound; j++)
            {
                Vector2 dir = new Vector2(i, j) - (Vector2)transform.position;
                hit = Physics2D.RaycastAll(transform.position, dir, dir.magnitude, LayerMask.GetMask("Tile"));
                for(int k=0; k<hit.Length; k++)
                {
                    if (k == hit.Length - 1)
                    {
                        Coordinate c = new(hit[k].transform.position);
                        tilesInSight.Add(c);
                    }
                    else if (dm.Map[(int)hit[k].transform.position.x, (int)hit[k].transform.position.y].TileData.tileType == TileType.Wall)
                        break;
                }
            }
        }
    }

    void CheckNewInSight()
    {
        List<Unit> detectedUnits = new();
        for (int i = 0; i < tilesInSight.Count; i++)
        {
            Tile tile = dm.GetTileByCoordinate(tilesInSight[i]);
            if (tile.unit != null)
                detectedUnits.Add(tile.unit);
        }
        if (UnitsInSight.Count > 0)
        {
            for (int i = 0; i < detectedUnits.Count; i++)
            {
                if (!UnitsInSight.Contains(detectedUnits[i]))
                    foundSomething = true;
            }
        }
        UnitsInSight = detectedUnits;
    }

    public void EndSkill(float turnSpent)
    {
        skill = null;
        EndTurn(turnSpent);
    }

    protected void EndTurn(float turnSpent)
    {
        UnitData.turnIndicator += turnSpent;
        Controllable = false;
        dm.EndTurn();
    }

    public void SkipTurn()
    {
        UnitData.turnIndicator += 1;
        Controllable = false;
        dm.EndTurn();
    }

    protected void FlipSprite(Directions direction)
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
    protected void FlipSprite(Coordinate lookAt)
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

    private void OnDestroy()
    {
        MySpriteRenderer.DOKill();
        transform.DOKill();
    }
}
