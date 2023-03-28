using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;

public class DungeonUIManager : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    public Player Player { get; private set; }

    [SerializeField] TMP_Text lvText;
    [SerializeField] Image hpBar;
    [SerializeField] TMP_Text hpText;
    [SerializeField] Image mpBar;
    [SerializeField] TMP_Text mpText;
    [SerializeField] Image expBar;
    [SerializeField] TMP_Text expText;

    [SerializeField] GameObject menuCanvas;
    public enum Menu { Status, Inventory, Ability, Skill, Soulstone }
    Menu currentMenu = Menu.Status;

    [SerializeField] StatusUI statusUI;
    [SerializeField] TMP_Text classText;

    [SerializeField] InventoryUI inventoryUI;

    [SerializeField] AbilityUI abilityUI;

    readonly LocalizedStringTable tableStatusText = new("Status Text");
    readonly LocalizedStringTable tableEquipmentName = new("Equipment Name");
    readonly LocalizedStringTable tableItemName = new("Item Name");
    readonly LocalizedStringTable tableItemDescription = new("Item Description");
    readonly LocalizedStringTable tableAbility = new("Ability");

    public void Init()
    {
        UpdateLevelText();
        UpdateHpBar();
        UpdateMpBar();
        UpdateExpBar();

        inventoryUI.Init();
        abilityUI.Init();
    }

    public void UpdateLevelText()
    {
        lvText.text = "LV " + (dm.Player.UnitData.Level + 1).ToString();
    }

    public void UpdateHpBar()
    {
        int hp = dm.Player.UnitData.Hp;
        int maxHp = dm.Player.UnitData.MaxHp.Total();
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
        int exp = dm.Player.UnitData.Exp;
        int maxExp = dm.Player.UnitData.MaxExp;
        expBar.fillAmount = exp / (float)maxExp;
        expText.text = exp.ToString() + "/" + maxExp.ToString() +"(" + Mathf.Round(exp/(float)maxExp*100) + "%)";
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
        }
    }

    public void OpenMenuCanvas()
    {
        if(dm.Player.Controllable)
        {
            menuCanvas.SetActive(true);
            UpdateMenuUI(currentMenu);
        }
    }
    public void CloseMenuCanvas()
    {
        menuCanvas.SetActive(false);
    }

    GameObject MenuToGameObject(Menu menu)
    {
        switch (menu)
        {
            case Menu.Status:
                return statusUI.gameObject;
            case Menu.Inventory:
                return inventoryUI.gameObject;
            case Menu.Ability:
                return abilityUI.gameObject;
            default: return null;
        }
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
            StatType.HP => "HP",
            StatType.Mp => "MP",
            StatType.Hunger => tableStatusText.GetTable().GetEntry("HUNGER").GetLocalizedString(),
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
        return tableAbility.GetTable().GetEntry(key + "_DESCRIPTION").GetLocalizedString();
    }
    public string GetAbilityEffect(string key, int[] values)
    {
        return tableAbility.GetTable().GetEntry(key + "_EFFECT").GetLocalizedString(values);
    }
}
