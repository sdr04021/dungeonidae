using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using System;

public class DungeonUIManager : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    public Player Player { get; private set; }

    [SerializeField] Canvas gameCanvas;

    [SerializeField] TMP_Text lvText;
    [SerializeField] Image hpBar;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Image mpBar;
    [SerializeField] TMP_Text mpText;
    [SerializeField] Image expBar;
    [SerializeField] TMP_Text expText;

    [SerializeField] RectTransform buffIconField;
    [SerializeField] BuffIcon buffIconPrefab;
    List<BuffIcon> buffIcons = new();

    [SerializeField] ShortcutButton[] skillShortcutButtons;
    bool shortCutPressed = false;

    [SerializeField] Canvas menuCanvas;
    public enum Menu { Status, Inventory, Ability, Skill, Soulstone }
    Menu currentMenu = Menu.Status;

    [SerializeField] StatusUI statusUI;
    [SerializeField] TMP_Text classText;

    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] AbilityUI abilityUI;

    [SerializeField] SkillUI skillUI;

    readonly LocalizedStringTable tableStatusText = new("Status Text");
    readonly LocalizedStringTable tableEquipmentName = new("Equipment Name");
    readonly LocalizedStringTable tableItemName = new("Item Name");
    readonly LocalizedStringTable tableItemDescription = new("Item Description");
    readonly LocalizedStringTable tableAbility = new("Ability");
    readonly LocalizedStringTable tableSkill = new("Skill Text");

    private void Awake()
    {
        if (!Application.isMobilePlatform)
        {
            gameCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(2560, 1440);
            menuCanvas.GetComponent<CanvasScaler>().referenceResolution = new Vector2(2560, 1440);
        }
    }

    public void Init()
    {
        UpdateLevelText();
        dm.Player.UnitData.OnLevelChanged += UpdateLevelText;
        UpdateHpBar();
        dm.Player.UnitData.OnHpValueChanged += UpdateHpBar;
        UpdateMpBar();
        dm.Player.UnitData.OnMpValueChanged += UpdateMpBar;
        UpdateExpBar();
        dm.Player.UnitData.OnExpValueChanged += UpdateExpBar;
        UpdateBuffIcons();
        dm.Player.UnitData.OnBuffListChanged += UpdateBuffIcons;
        UpdateBuffIconDurations();
        dm.Player.UnitData.OnBuffDurationChanged += UpdateBuffIconDurations;
        SetSkillIcons();
        dm.Player.UnitData.OnSkillChanged += SetSkillIcons;
        UpdateSkillIconCooldowns();
        dm.Player.UnitData.OnSkillCurrentCooldownChanged += UpdateSkillIconCooldowns;
        dm.Player.UnitData.OnSkillChanged += UpdateSkillIconCooldowns;

        inventoryUI.Init();
        abilityUI.Init();
    }

    public void UpdateLevelText()
    {
        lvText.text = "LV " + (dm.Player.UnitData.level + 1).ToString();
    }

    public void UpdateHpBar()
    {
        int hp = dm.Player.UnitData.Hp;
        int maxHp = dm.Player.UnitData.maxHp.Total();
        hpBar.fillAmount = hp / (float)maxHp;
        hpText.text = hp.ToString() + "/" + maxHp.ToString();
    }

    public void UpdateMpBar()
    {
        int mp = dm.Player.UnitData.Mp;
        int maxMp = dm.Player.UnitData.maxMp.Total();
        mpBar.fillAmount = mp / (float)maxMp;
        mpText.text = mp.ToString() + "/" + maxMp.ToString();
    }

    public void UpdateExpBar()
    {
        int exp = dm.Player.UnitData.exp;
        int maxExp = dm.Player.UnitData.maxExp;
        expBar.fillAmount = exp / (float)maxExp;
        expText.text = exp.ToString() + "/" + maxExp.ToString() +"(" + Mathf.Round(exp/(float)maxExp*100) + "%)";
    }

    public void UpdateBuffIcons()
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

    public void UpdateBuffIconDurations()
    {
        List<BuffData> buffs = dm.Player.UnitData.Buffs;
        for (int i = 0; i < buffs.Count; i++)
        {
            buffIcons[i].SetDuration(buffs[i]);
        }
    }

    void SetSkillIcons()
    {
        for(int i=0; i<skillShortcutButtons.Length; i++)
        {
            if (dm.Player.UnitData.Skills[i]==null)
                skillShortcutButtons[i].icon.gameObject.SetActive(false);
            else
            {
                skillShortcutButtons[i].icon.sprite = dm.Player.UnitData.Skills[i].Sprite;
                skillShortcutButtons[i].icon.gameObject.SetActive(true);
            }
        }
    }
    void UpdateSkillIconCooldowns()
    {
        for (int i = 0; i < skillShortcutButtons.Length; i++)
        {
            SkillData skill = dm.Player.UnitData.Skills[i];
            if ((skill == null) || (skill.currentCoolDown <= 0))
            {
                skillShortcutButtons[i].curtain.gameObject.SetActive(false);
                skillShortcutButtons[i].coolDownText.gameObject.SetActive(false);
            }
            else
            {
                skillShortcutButtons[i].curtain.gameObject.SetActive(true);
                skillShortcutButtons[i].coolDownText.gameObject.SetActive(true);
                skillShortcutButtons[i].coolDownText.text = skill.currentCoolDown.ToString();
            }
        }
    }

    public void Btn_SkillShortcutPointerDown(int index)
    {
        if (shortCutPressed) return;
        shortCutPressed = true;
        if (dm.Player.Controllable)
        {
            dm.Player.PrepareSkill(index);
        }
        else if (dm.Player.IsSkillMode)
        {
            dm.Player.AutoSkill();
        }
    }
    public void Btn_SkillShortcutPointerUp()
    {
        if (dm.Player.IsSkillMode && (dm.Player.skill.AvailableTilesInRange.Count == 0))
        {
            dm.Player.CancelSkill();
        }
        shortCutPressed = false;
    }
    public void Btn_ActionShortCutPointerDown(int index)
    {
        if (shortCutPressed) return;
        shortCutPressed = true;
        if (!dm.Player.Controllable) return;
        switch(index)
        {
            case 0:
                if (dm.Player.IsBasicAttackMode)
                    dm.Player.AutoBasicAttack();
                else
                    dm.Player.PrepareBasicAttack();
                break;
            case 1:
                dm.Player.SkipTurn();
                break;
            case 2:
                dm.Player.LootItem();
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
            if (dm.Player.IsBasicAttackMode && (dm.Player.BasicAttack.AvailableTilesInRange.Count == 0))
            {
                dm.Player.CancelBasicAttack();
            }
        }
        shortCutPressed = false;
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
            StatType.Block => tableStatusText.GetTable().GetEntry("BLOCK").GetLocalizedString(),
            StatType.Resist => tableStatusText.GetTable().GetEntry("RESIST").GetLocalizedString(),
            StatType.DmgReduction => tableStatusText.GetTable().GetEntry("DAMAGE_REDUCTION").GetLocalizedString(),
            StatType.Sight => tableStatusText.GetTable().GetEntry("SIGHT").GetLocalizedString(),
            StatType.Instinct => tableStatusText.GetTable().GetEntry("INSTINCT").GetLocalizedString(),
            StatType.SearchRange => tableStatusText.GetTable().GetEntry("SEARCH_RANGE").GetLocalizedString(),
            StatType.HpRegen => tableStatusText.GetTable().GetEntry("HEALTH_REGENERATION").GetLocalizedString(),
            StatType.MpRegen => tableStatusText.GetTable().GetEntry("MANA_REGENERATION").GetLocalizedString(),
            StatType.Speed => tableStatusText.GetTable().GetEntry("MOVE_SPEED").GetLocalizedString(),
            _ => null,
        };
    }

    public string GetEquipmentName(string key)
    {
        return tableEquipmentName.GetTable().GetEntry(key).GetLocalizedString();
    }

    public string GetItemName(string key)
    {
        return tableItemName.GetTable().GetEntry(key).GetLocalizedString();
    }

    public string GetItemDescription(string key, int[] values)
    {
        if (values.Length > 0)
            return tableItemDescription.GetTable().GetEntry(key).GetLocalizedString(values);
        else
            return tableItemDescription.GetTable().GetEntry(key).GetLocalizedString();
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
    public string GetSkillDescription(string key)
    {
        return tableSkill.GetTable().GetEntry(key + "_DESC").GetLocalizedString();
    }
    public string GetSkillEffect(string key, int[] values)
    {
        return tableSkill.GetTable().GetEntry(key + "_EFFECT").GetLocalizedString(values);
    }
}
