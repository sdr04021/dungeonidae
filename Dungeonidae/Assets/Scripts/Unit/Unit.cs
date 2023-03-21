using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

abstract public class Unit : MonoBehaviour
{
    protected DungeonManager dm;
    protected Tile[,] map;
    Coordinate _coord = new (0, 0);
    public Coordinate Coord => _coord;
    public bool Controllable { get; private set; } = false;
    public int UnitListIndex { get; private set; } = 0;
    public float TurnIndicator { get; private set; } = 0;
    public bool IsDead { get; private set; } = false;

    protected List<Coordinate> tilesInSight = new();
    protected bool isFollowingPath = false;
    protected Stack<Directions> path;
    public List<Unit> UnitsInSight { get; private set; } = new();
    protected bool foundSomething = false;
    bool isAnimationFinished = false;
    protected Unit chaseTarget;
    protected Coordinate chaseTargetRecentCoord;

    [SerializeField] BaseStats baseStats;
    public UnitData Data { get; private set; }
    public SpriteRenderer MySpriteRenderer { get; private set; }
    public Animator MyAnimator { get; private set; }
    public Canvas canvas;
    [SerializeField] Image hpBarBg;
    public Image hpBar;
    readonly WaitForSeconds shortDelay = new(0.1f);

    private void Awake()
    {
        MySpriteRenderer = GetComponent<SpriteRenderer>();
        MyAnimator = GetComponent<Animator>();
        //map = GameManager.Instance.dungeonManager.map;
        Data = new(baseStats);
    }

    protected virtual void Start()
    {
        UpdateHpBar();
    }

    public virtual void Init(DungeonManager dungeonManager, Coordinate c)
    {
        dm = dungeonManager;
        map = dm.map;
        SetStartPosition(c.x, c.y);
        UpdateSightArea();
    }

    public void SetStartPosition(int x, int y)
    {
        transform.position = map[x, y].transform.position;
        _coord.Set(x, y);
        map[x, y].unit = this;
    }
    public void SetUnitListIndex(int i)
    {
        UnitListIndex = i;
    }

    public virtual void StartTurn()
    {
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
        TurnIndicator -= amount;
    }

    public bool Move(Directions direction)
    {
        FlipSprite(direction);

        Coordinate dest = _coord.ToMovedCoordinate(direction, 1);

        if (dest.IsValidCoordForMap(map))
        {
            if (dm.IsReachable(dest))
            {
                Controllable = false;

                if (!dm.FogMap[dest.x, dest.y].IsOn)
                {
                    MySpriteRenderer.enabled = true;
                    if (canvas != null) canvas.enabled = true;
                }

                map[_coord.x, _coord.y].unit = null;
                _coord = dest;
                TurnIndicator += 100f / Data.speed.Total();

                if (MySpriteRenderer.enabled)
                {
                    MyAnimator.SetBool("Walk", true);
                    transform.DOMove(dest.ToVector2(), 0.2f)
                    .SetEase(Ease.Linear)
                    .OnComplete(EndBehavior);
                }
                else
                {
                    transform.position = map[dest.x, dest.y].transform.position;
                    EndBehavior();
                }
                return true;
            }
        }
        return false;
    }

    public void StartBasicAttack(Unit target)
    {
        if (!Controllable || Data.aspd.Total() <= 0) return;
        if (_coord.IsTargetInRange(target.Coord, Data.atkRange.Total())){
            Controllable = false;
            MyAnimator.SetBool("Attack", true);
            StartCoroutine(BasicAttack(target));
        }
    }
    public void EndBasicAttack()
    {
        MyAnimator.SetBool("Attack", false);
        isAnimationFinished = true;
    }
    IEnumerator BasicAttack(Unit target)
    {
        float animationLength = MyAnimator.GetCurrentAnimatorStateInfo(0).length;
        int damage = Data.atk.Total();
        yield return new WaitForSeconds(animationLength * 0.5f);
        if (damage < 0) damage = 0;
        target.GetDamage(new AttackData(this, AttackType.Atk, damage));
        TurnIndicator += 100f / Data.aspd.Total();
        while (!isAnimationFinished || !target.isAnimationFinished)
        {
            yield return shortDelay;
        }
        isAnimationFinished = false;
        target.isAnimationFinished = false;
        EndTurn();
    }

    public virtual void GetDamage(AttackData attackData)
    {
        int damage;
        switch (attackData.Type)
        {
            case AttackType.Atk:
                damage = attackData.Damage - Data.def.Total();
                break;
            case AttackType.MAtk:
                damage = attackData.Damage - Data.mDef.Total();
                break;
            default: damage = attackData.Damage; break;
        }
        Data.Hp -= damage;
        UpdateHpBar();
        if(canvas!= null)
        {
            DamageText dt = Instantiate(GameManager.Instance.damageTextPrefab, canvas.transform);
            dt.SetValue(damage);
        }
        MySpriteRenderer.DOColor(Color.red, 0.2f).OnComplete(ToDefaultColor);
        if (Data.Hp <= 0)
        {
            StartDie();
        }
        else isAnimationFinished = true;
    }
    protected virtual void StartDie()
    {
        MySpriteRenderer.DOFade(0, 1.1f).OnComplete(Die);
        if (hpBarBg != null) { hpBarBg.DOFade(0, 1f); }
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

    void UpdateHpBar()
    {
        if (hpBar != null)
            hpBar.fillAmount = Data.Hp / (float)Data.maxHp.Total();
        else
            dm.UpdatePlayerHpBar();
    }


   protected virtual void EndBehavior()
    {
        UpdateSightArea();
        if (isFollowingPath)
            MyAnimator.SetBool("Walk", false);
        if (dm.FogMap[_coord.x, _coord.y].IsOn)
        {
            MySpriteRenderer.enabled = false;
            if (canvas != null) canvas.enabled = false;
        }
        map[_coord.x, _coord.y].unit = this;
        EndTurn();
    }

    protected void RandomStep()
    {
        List<Directions> deck = new();
        for(int i=0; i<8; i++)
        {
            Coordinate dest = _coord.ToMovedCoordinate((Directions)i, 1);
            if (dest.IsValidCoordForMap(dm.map))
            {
                if(dm.IsReachable(dest))
                    deck.Add((Directions)i);
            }
        }
        int max = deck.Count;
        if (max == 0)
            EndTurn();
        else
            Move(deck[Random.Range(0, max)]);
    }

    protected bool FindPath(Coordinate targetCoord)
    {
        PathFinder pf = new();
        Directions dir = pf.FindPath(_coord, targetCoord, dm.map, dm.FogMap);

        if (dir==Directions.NONE)
        {
            return false;
        }
        else
        {
            path = pf.Path;
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
        PathFinder pf = new();
        Directions dir = pf.FindPath(_coord, targetCoord, dm.map, dm.FogMap);

        if (dir == Directions.NONE)
            return dir;
        else
        {
            return pf.Path.Pop();
        }
    }

    protected virtual void UpdateSightAreaOld()
    {
        int northBound = Mathf.Min(_coord.y + Data.sight.Total(), map.GetLength(1) - 1);
        int southBound = Mathf.Max(_coord.y - Data.sight.Total(), 0);
        int eastBound = Mathf.Min(_coord.x + Data.sight.Total(), map.GetLength(1) - 1);
        int westBound = Mathf.Max(_coord.x - Data.sight.Total(), 0);
        
        tilesInSight.Clear();

        RaycastHit2D[] hit;
        for (int i = 0; i <= eastBound - westBound; i++)
        {
            Vector2 dir = new Vector2(westBound + i, northBound) - (Vector2)transform.position;
            hit = Physics2D.RaycastAll(transform.position, dir, dir.magnitude, LayerMask.GetMask("Tile"));
            for(int j=0; j<hit.Length; j++)
            {
                Coordinate c = new Coordinate(hit[j].transform.position);
                tilesInSight.Add(c);
                if (map[tilesInSight[^1].x, tilesInSight[^1].y].type == TileType.Wall)
                    break;
            }
            dir = new Vector2(westBound + i, southBound) - (Vector2)transform.position;
            hit = Physics2D.RaycastAll(transform.position, dir, dir.magnitude, LayerMask.GetMask("Tile"));
            for (int j = 0; j < hit.Length; j++)
            {
                tilesInSight.Add(new Coordinate(hit[j].transform.position));
                if (map[tilesInSight[^1].x, tilesInSight[^1].y].type == TileType.Wall)
                    break;
            }
        }
        for (int i = 1; southBound + i < northBound; i++)
        {
            Vector2 dir = new Vector2(westBound, southBound + i) - (Vector2)transform.position;
            hit = Physics2D.RaycastAll(transform.position, dir, dir.magnitude, LayerMask.GetMask("Tile"));
            for (int j = 0; j < hit.Length; j++)
            {
                tilesInSight.Add(new Coordinate(hit[j].transform.position));
                if (map[tilesInSight[^1].x, tilesInSight[^1].y].type == TileType.Wall)
                    break;
            }
            dir = new Vector2(eastBound, southBound + i) - (Vector2)transform.position;
            hit = Physics2D.RaycastAll(transform.position, dir, dir.magnitude, LayerMask.GetMask("Tile"));
            for (int j = 0; j < hit.Length; j++)
            {
                tilesInSight.Add(new Coordinate(hit[j].transform.position));
                if (map[tilesInSight[^1].x, tilesInSight[^1].y].type == TileType.Wall)
                    break;
            }
        }
    }

    protected virtual void UpdateSightArea()
    {
        int northBound = Mathf.Min(_coord.y + Data.sight.Total(), map.GetLength(1) - 1);
        int southBound = Mathf.Max(_coord.y - Data.sight.Total(), 0);
        int eastBound = Mathf.Min(_coord.x + Data.sight.Total(), map.GetLength(1) - 1);
        int westBound = Mathf.Max(_coord.x - Data.sight.Total(), 0);

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
                    else if (map[(int)hit[k].transform.position.x, (int)hit[k].transform.position.y].type == TileType.Wall)
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

    void EndTurn()
    {
        Controllable = false;
        dm.EndTurn();
    }

    public void SkipTurn()
    {
        TurnIndicator += 1;
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
}
