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

    readonly LocalizedStringTable tableStatusText = new("Status Text");
    readonly LocalizedStringTable tableEquipmentName = new("Equipment Name");
    readonly LocalizedStringTable tableItemName = new("Item Name");
    readonly LocalizedStringTable tableItemDescription = new("Item Description");

    public void Init()
    {
        UpdateLevelText();
        UpdateHpBar();
        UpdateMpBar();
        UpdateExpBar();


        inventoryUI.Init();
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
        }
    }

    public void OpenMenuCanvas()
    {
        menuCanvas.SetActive(true);
        UpdateMenuUI(currentMenu);
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
        switch (stat)
        {
            case StatType.Atk:
                return tableStatusText.GetTable().GetEntry("ATTACK").GetLocalizedString();
            case StatType.MAtk:
                return tableStatusText.GetTable().GetEntry("MAGIC_ATTACK").GetLocalizedString();
            case StatType.AtkRange:
                return tableStatusText.GetTable().GetEntry("ATTACK_RANGE").GetLocalizedString();
            case StatType.Pen:
                return tableStatusText.GetTable().GetEntry("ARMOR_PENETRATION").GetLocalizedString();
            case StatType.MPen:
                return tableStatusText.GetTable().GetEntry("MAGIC_PENETRATION").GetLocalizedString();
            case StatType.Acc:
                return tableStatusText.GetTable().GetEntry("ACCURACY").GetLocalizedString();
            case StatType.Aspd:
                return tableStatusText.GetTable().GetEntry("ATTACK_SPEED").GetLocalizedString();
            case StatType.Cri:
                return tableStatusText.GetTable().GetEntry("CRITICAL_RATE").GetLocalizedString();
            case StatType.CriDmg:
                return tableStatusText.GetTable().GetEntry("CRITICAL_DAMAGE").GetLocalizedString();
            case StatType.Proficiency:
                return tableStatusText.GetTable().GetEntry("PROFICIENCY").GetLocalizedString();
            case StatType.LifeSteal:
                return tableStatusText.GetTable().GetEntry("LIFE_STEAL").GetLocalizedString();
            case StatType.ManaSteal:
                return tableStatusText.GetTable().GetEntry("MANA_STEAL").GetLocalizedString();
            case StatType.DmgIncrease:
                return tableStatusText.GetTable().GetEntry("DAMAGE_INCREASE").GetLocalizedString();
            case StatType.HP:
                return "HP";
            case StatType.Mp:
                return "MP";
            case StatType.Hunger:
                return tableStatusText.GetTable().GetEntry("HUNGER").GetLocalizedString();
            case StatType.Def:
                return tableStatusText.GetTable().GetEntry("DEFENSE").GetLocalizedString();
            case StatType.MDef:
                return tableStatusText.GetTable().GetEntry("MAGIC_DEFENSE").GetLocalizedString();
            case StatType.Eva:
                return tableStatusText.GetTable().GetEntry("EVASION").GetLocalizedString();
            case StatType.Block:
                return tableStatusText.GetTable().GetEntry("BLOCK").GetLocalizedString();
            case StatType.Resist:
                return tableStatusText.GetTable().GetEntry("RESIST").GetLocalizedString();
            case StatType.DmgReduction:
                return tableStatusText.GetTable().GetEntry("DAMAGE_REDUCTION").GetLocalizedString();
            case StatType.Sight:
                return tableStatusText.GetTable().GetEntry("SIGHT").GetLocalizedString();
            case StatType.Instinct:
                return tableStatusText.GetTable().GetEntry("INSTINCT").GetLocalizedString();
            case StatType.SearchRange:
                return tableStatusText.GetTable().GetEntry("SEARCH_RANGE").GetLocalizedString();
            case StatType.HpRegen:
                return tableStatusText.GetTable().GetEntry("HEALTH_REGENERATION").GetLocalizedString();
            case StatType.MpRegen:
                return tableStatusText.GetTable().GetEntry("MANA_REGENERATION").GetLocalizedString();
            case StatType.Speed:
                return tableStatusText.GetTable().GetEntry("MOVE_SPEED").GetLocalizedString();
            default: return null;
        }
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
}
