using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using System;
using DG.Tweening;
using Unity.Mathematics;

public class DungeonUIManager : MonoBehaviour
{
    [SerializeField] DungeonManager dm;

    [SerializeField] Canvas gameCanvas;

    [SerializeField] GameObject curtain;
    [SerializeField] TMP_Text curtainFloorText;
    [SerializeField] Image loadingImage;

    [SerializeField] TMP_Text lvText;
    [SerializeField] Image hpBar;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Image barrierBar;
    [SerializeField] TMP_Text barrierText;
    [SerializeField] Image mpBar;
    [SerializeField] TMP_Text mpText;
    [SerializeField] Image expBar;
    [SerializeField] TMP_Text expText;

    [SerializeField] RectTransform buffIconField;
    [SerializeField] BuffIcon buffIconPrefab;
    List<BuffIcon> buffIcons = new();

    [SerializeField] ShortcutButton[] skillShortcutButtons;
    bool shortCutPressed = false;

    [SerializeField] Image interactIcon;

    [SerializeField] Canvas menuCanvas;
    public enum Menu { Status, Inventory, Ability, Skill, Soulstone }
    Menu currentMenu = Menu.Status;

    [SerializeField] StatusUI statusUI;
    [SerializeField] TMP_Text classText;

    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] AbilityUI abilityUI;

    [SerializeField] SkillUI skillUI;

    readonly LocalizedStringTable tableStatusText = new("Status Text");
    readonly LocalizedStringTable tableEquipment = new("Equipment Text");
    readonly LocalizedStringTable tableMiscItem = new("Misc Item Text");
    readonly LocalizedStringTable tableAbility = new("Ability");
    readonly LocalizedStringTable tableSkill = new("Skill Text");

    public bool isMapLoadComplete = false;

    private void Awake()
    {
        if (!Application.isMobilePlatform)
        {
            //gameCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(960, 540);
            //menuCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(960, 540);
        }
    }

    public void Init()
    {
        UpdateLevelText();
        dm.Player.UnitData.OnLevelChange += UpdateLevelText;
        UpdateHpBar();
        dm.Player.UnitData.OnHpValueChange += UpdateHpBar;  
        UpdateBarrierBar();
        dm.Player.UnitData.OnBarrierValueChange += UpdateBarrierBar;
        UpdateMpBar();
        dm.Player.UnitData.OnMpValueChange += UpdateMpBar;
        UpdateExpBar();
        dm.Player.UnitData.OnExpValueChange += UpdateExpBar;
        UpdateBuffIcons();
        dm.Player.UnitData.OnBuffListChange += UpdateBuffIcons;
        UpdateBuffIconDurations();
        dm.Player.UnitData.OnBuffDurationChange += UpdateBuffIconDurations;
        UpdateBuffIconStacks();
        dm.Player.UnitData.OnBuffStackChange += UpdateBuffIconStacks;
        SetSkillIcons();
        dm.Player.UnitData.OnSkillChange += SetSkillIcons;
        UpdateSkillIconRechargeLeft();
        dm.Player.UnitData.OnSkillRechargeChange += UpdateSkillIconRechargeLeft;
        dm.Player.UnitData.OnSkillChange += UpdateSkillIconRechargeLeft;

        dm.Player.UnitData.OnTurnChange += UpdateInteractIcon;

        inventoryUI.Init();
        abilityUI.Init();
    }
    
    public void ShowFloorCurtain(int floor)
    {
        curtain.SetActive(true);
        isMapLoadComplete = false;
        curtainFloorText.text = "B" + (floor + 1).ToString() + "F";
        curtainFloorText.DOFade(1, 0.5f).OnComplete(() => { StartCoroutine(HideFloorCurtain()); });
        loadingImage.rectTransform.DORotate(new Vector3(0, 0, 360), 1, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
    }
    IEnumerator HideFloorCurtain()
    {
        while (!isMapLoadComplete)
        {
            yield return Constants.ZeroPointOne;
        }
        curtainFloorText.DOFade(0, 0.5f).OnComplete(() => {
            Camera.main.transform.position = new Vector3(dm.Player.transform.position.x, dm.Player.transform.position.y, Camera.main.transform.position.z);
            curtain.SetActive(false);
        });
        loadingImage.rectTransform.DOKill();
    }

    void UpdateLevelText()
    {
        lvText.text = "LV " + (dm.Player.UnitData.level + 1).ToString();
    }

    void UpdateHpBar()
    {
        int hp = dm.Player.UnitData.Hp;
        int maxHp = dm.Player.UnitData.maxHp.Total();
        hpBar.fillAmount = hp / (float)maxHp;
        hpText.text = hp.ToString() + "/" + maxHp.ToString();
    }
    void UpdateBarrierBar()
    {
        int barrier = dm.Player.UnitData.Barrier;
        if (barrier <= 0)
        {
            barrierText.gameObject.SetActive(false);
            barrierBar.fillAmount = 0;
        }
        else
        {
            int maxHp = dm.Player.UnitData.maxHp.Total();
            barrierBar.fillAmount = barrier / (float)maxHp;
            barrierText.gameObject.SetActive(true);
            barrierText.text = barrier.ToString();
        }
    }
    void UpdateMpBar()
    {
        int mp = dm.Player.UnitData.Mp;
        int maxMp = dm.Player.UnitData.maxMp.Total();
        mpBar.fillAmount = mp / (float)maxMp;
        mpText.text = mp.ToString() + "/" + maxMp.ToString();
    }

    void UpdateExpBar()
    {
        int exp = dm.Player.UnitData.exp;
        int maxExp = dm.Player.UnitData.maxExp;
        expBar.fillAmount = exp / (float)maxExp;
        expText.text = exp.ToString() + "/" + maxExp.ToString() +"(" + Mathf.Round(exp/(float)maxExp*100) + "%)";
    }

    void UpdateBuffIcons()
    {
        List<BuffData> buffs = dm.Player.UnitData.Buffs;
        for (int i = 0; i < buffs.Count; i++)
        {
            if (i < buffIcons.Count)
            {
                buffIcons[i].SetBuff(buffs[i]);
            }
            else
            {
                BuffIcon temp = Instantiate(buffIconPrefab, buffIconField);
                temp.Init(this);
                temp.SetBuff(buffs[i]);
                buffIcons.Add(temp);
            }
        }
        for (int i = buffIcons.Count - 1; i >= 0; i--)
        {
            if (i >= buffs.Count)
            {
                Destroy(buffIcons[i].gameObject);
                buffIcons.RemoveAt(i);
            }
        }
    }

    void UpdateBuffIconDurations()
    {
        List<BuffData> buffs = dm.Player.UnitData.Buffs;
        for (int i = 0; i < buffs.Count; i++)
        {
            buffIcons[i].SetDuration(buffs[i]);
        }
    }

    void UpdateBuffIconStacks()
    {
        List<BuffData> buffs = dm.Player.UnitData.Buffs;
        for (int i = 0; i < buffs.Count; i++)
        {
            buffIcons[i].SetStack(buffs[i]);
        }
    }

    void SetSkillIcons()
    {
        for(int i=0; i<skillShortcutButtons.Length; i++)
        {
            if (dm.Player.UnitData.currentSkills[i]==null)
                skillShortcutButtons[i].icon.gameObject.SetActive(false);
            else
            {
                skillShortcutButtons[i].icon.sprite = GameManager.Instance.GetSkillBase(dm.Player.UnitData.currentSkills[i]).Sprite;
                skillShortcutButtons[i].icon.gameObject.SetActive(true);
            }
        }
    }
    void UpdateSkillIconRechargeLeft()
    {
        for (int i = 0; i < skillShortcutButtons.Length; i++)
        {
            string skill = dm.Player.UnitData.currentSkills[i];
            int rechargeLeft = dm.Player.UnitData.skillRechargeLeft[i];
            if ((skill == null) || (rechargeLeft > 0))
            {
                skillShortcutButtons[i].curtain.gameObject.SetActive(true);
                skillShortcutButtons[i].coolDownText.gameObject.SetActive(true);
                skillShortcutButtons[i].coolDownText.text = rechargeLeft.ToString();
            }
            else
            {
                skillShortcutButtons[i].curtain.gameObject.SetActive(false);
                skillShortcutButtons[i].coolDownText.gameObject.SetActive(false);
            }
        }
    }

    public void Btn_SkillShortcutPointerDown(int index)
    {
        if (shortCutPressed) return;
        shortCutPressed = true;

        if (dm.Player.Controllable)
        {
            dm.Player.StartSkill(index);
        }
        else if (dm.Player.IsSkillMode && dm.Player.CurrentSkill.Key == dm.Player.UnitData.currentSkills[index])
        {
            dm.Player.SkillOnCurrentTargeting();
        }

    }
    public void Btn_SkillShortcutPointerUp()
    {
        shortCutPressed = false;
        if (dm.Player.IsSkillMode && (dm.Player.AvailableRange.Count == 0))
        {
            dm.Player.CancelSkill();
        }
    }
    public void Btn_ActionShortCutPointerDown(int index)
    {
        if (shortCutPressed) return;
        shortCutPressed = true;
        if (!dm.Player.Controllable) return;
        switch(index)
        {
            case 0:
                if (dm.Player.IsSkillMode && dm.Player.CurrentSkill == dm.Player.BasicAttack)
                    dm.Player.SkillOnCurrentTargeting();
                else
                    dm.Player.StartBasicAttack();
                break;
            case 1:
                dm.Player.SkipTurn();
                break;
            case 2:
                dm.Player.Interact();
                break;
            case 3:
                break;
            default:
                throw new System.NotImplementedException();
        }
    }
    public void Btn_ActionShortCutPointerUp(int index)
    {
        if (index == 0)
        {
            if (dm.Player.IsSkillMode && dm.Player.CurrentSkill == dm.Player.BasicAttack && (dm.Player.AvailableRange.Count == 0))
            {
                dm.Player.CancelSkill();
            }
        }
        shortCutPressed = false;
    }
    public void UpdateInteractIcon()
    {
        Tile tile = dm.map.GetElementAt(dm.Player.UnitData.coord);
        if(tile.items.Count > 0)
        {
            interactIcon.gameObject.SetActive(true);
            //interactIcon.sprite = tile.items.Peek().data.Sprite;
            ItemObject topItem = tile.items.Peek();
            interactIcon.sprite = topItem.DataType == typeof(MiscData) ? GameManager.Instance.GetSprite(SpriteAssetType.Misc, topItem.Data.Key) : GameManager.Instance.GetSprite(SpriteAssetType.Equipment, topItem.Data.Key);
            return;
        }
        else if(tile.dungeonObjects.Count > 0)
        {
            if (tile.dungeonObjects[^1].IsInteractable)
            {
                interactIcon.gameObject.SetActive(true);
                interactIcon.sprite = tile.dungeonObjects[^1].SpriteRenderer.sprite;
                return;
            }
        }
        interactIcon.gameObject.SetActive(false);
    }

    public void UpdateMenuUI(Menu menu)
    {
        switch (menu)
        {
            case Menu.Status:
                statusUI.UpdateStatus();
                break;
            case Menu.Inventory:
                inventoryUI.Refresh();
                break;
            case Menu.Ability:
                abilityUI.Refresh();
                break;
            case Menu.Skill:
                skillUI.Refresh();
                break;
        }
    }

    public void OpenMenuCanvas()
    {
        if(dm.Player.Controllable)
        {
            menuCanvas.gameObject.SetActive(true);
            UpdateMenuUI(currentMenu);
        }
    }
    public void CloseMenuCanvas()
    {
        menuCanvas.gameObject.SetActive(false);
    }

    GameObject MenuToGameObject(Menu menu)
    {
        return menu switch
        {
            Menu.Status => statusUI.gameObject,
            Menu.Inventory => inventoryUI.gameObject,
            Menu.Ability => abilityUI.gameObject,
            Menu.Skill => skillUI.gameObject,
            _ => null,
        };
    }

    public void TabButtonClick(int menu)
    {
        MenuToGameObject(currentMenu).SetActive(false);

        UpdateMenuUI((Menu)menu);
        MenuToGameObject((Menu)menu).SetActive(true);

        currentMenu = (Menu)menu;
    }

    public string StatTypeToString(StatType stat)
    {
        return stat switch
        {
            StatType.Atk => tableStatusText.GetTable().GetEntry("ATTACK").GetLocalizedString(),
            StatType.MAtk => tableStatusText.GetTable().GetEntry("MAGIC_ATTACK").GetLocalizedString(),
            StatType.AtkRange => tableStatusText.GetTable().GetEntry("ATTACK_RANGE").GetLocalizedString(),
            StatType.Pen => tableStatusText.GetTable().GetEntry("ARMOR_PENETRATION").GetLocalizedString(),
            StatType.MPen => tableStatusText.GetTable().GetEntry("MAGIC_PENETRATION").GetLocalizedString(),
            StatType.Acc => tableStatusText.GetTable().GetEntry("ACCURACY").GetLocalizedString(),
            StatType.Aspd => tableStatusText.GetTable().GetEntry("ATTACK_SPEED").GetLocalizedString(),
            StatType.Cri => tableStatusText.GetTable().GetEntry("CRITICAL_RATE").GetLocalizedString(),
            StatType.CriDmg => tableStatusText.GetTable().GetEntry("CRITICAL_DAMAGE").GetLocalizedString(),
            StatType.Proficiency => tableStatusText.GetTable().GetEntry("PROFICIENCY").GetLocalizedString(),
            StatType.LifeSteal => tableStatusText.GetTable().GetEntry("LIFE_STEAL").GetLocalizedString(),
            StatType.ManaSteal => tableStatusText.GetTable().GetEntry("MANA_STEAL").GetLocalizedString(),
            StatType.DmgIncrease => tableStatusText.GetTable().GetEntry("DAMAGE_INCREASE").GetLocalizedString(),
            StatType.MaxHp => "HP",
            StatType.MaxMp => "MP",
            StatType.MaxHunger => tableStatusText.GetTable().GetEntry("HUNGER").GetLocalizedString(),
            StatType.Def => tableStatusText.GetTable().GetEntry("DEFENSE").GetLocalizedString(),
            StatType.MDef => tableStatusText.GetTable().GetEntry("MAGIC_DEFENSE").GetLocalizedString(),
            StatType.Eva => tableStatusText.GetTable().GetEntry("EVASION").GetLocalizedString(),
            StatType.CoolSpeed => tableStatusText.GetTable().GetEntry("COOLDOWN_SPEED").GetLocalizedString(),
            StatType.Resist => tableStatusText.GetTable().GetEntry("RESIST").GetLocalizedString(),
            StatType.DmgReduction => tableStatusText.GetTable().GetEntry("DAMAGE_REDUCTION").GetLocalizedString(),
            StatType.Sight => tableStatusText.GetTable().GetEntry("SIGHT").GetLocalizedString(),
            StatType.Instinct => tableStatusText.GetTable().GetEntry("INSTINCT").GetLocalizedString(),
            StatType.SearchRange => tableStatusText.GetTable().GetEntry("SEARCH_RANGE").GetLocalizedString(),
            StatType.HpRegen => tableStatusText.GetTable().GetEntry("HEALTH_REGENERATION").GetLocalizedString(),
            StatType.MpRegen => tableStatusText.GetTable().GetEntry("MANA_REGENERATION").GetLocalizedString(),
            StatType.Speed => tableStatusText.GetTable().GetEntry("MOVE_SPEED").GetLocalizedString(),
            StatType.Stealth => tableStatusText.GetTable().GetEntry("STEALTH").GetLocalizedString(),
            _ => null,
        };
    }

    public string GetEquipmentName(string key)
    {
        return tableEquipment.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }
    public string GetEquipmentAbilitiy(string key)
    {
        return tableEquipment.GetTable().GetEntry("ABILITY_" + key).GetLocalizedString();
    }
    public string GetEquipmentAbilitiy(string key, List<int> values)
    {
        return tableEquipment.GetTable().GetEntry("ABILITY_" + key).GetLocalizedString(values);
    }
    public string GetItemName(string key)
    {
        return tableMiscItem.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }

    public string GetItemDescription(string key, int[] values)
    {
        if (values.Length > 0)
            return tableMiscItem.GetTable().GetEntry(key + "_DESC").GetLocalizedString(values);
        else
            return tableMiscItem.GetTable().GetEntry(key + "_DESC").GetLocalizedString();
    }

    public string GetAbilityName(string key)
    {
        return tableAbility.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }
    public string GetAbilityDescription(string key)
    {
        return tableAbility.GetTable().GetEntry(key + "_DESC").GetLocalizedString();
    }
    public string GetAbilityEffect(string key, int[] values)
    {
        return tableAbility.GetTable().GetEntry(key + "_EFFECT").GetLocalizedString(values);
    }
    public string GetSkillName(string key)
    {
        return tableSkill.GetTable().GetEntry(key + "_NAME").GetLocalizedString();
    }
    public string GetSkillDescription(string key, int[] values)
    {
        return tableSkill.GetTable().GetEntry(key + "_DESC").GetLocalizedString(values);
    }
}
