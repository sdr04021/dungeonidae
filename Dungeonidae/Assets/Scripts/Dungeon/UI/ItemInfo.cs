using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;

public class ItemInfo : MonoBehaviour
{
    InventoryUI inventoryUI;
    [SerializeField] GameObject background;
    [SerializeField] Image icon;
    [SerializeField] RectTransform content;
    [SerializeField] TMP_Text title;
    [SerializeField] TMP_Text mainText;
    [SerializeField] TMP_Text potentialText;
    [SerializeField] TMP_Text additionalText;
    [SerializeField] GameObject EquipButton;
    [SerializeField] GameObject UseButton;

    [SerializeField] ItemSlotType itemSlotType;

    HashSet<StatType> percentPointStats = new() { StatType.Pen, StatType.MPen, StatType.Proficiency,StatType.Cri,StatType.CriDmg,
            StatType.Aspd,StatType.LifeSteal,StatType.ManaSteal,StatType.Resist,StatType.CoolSpeed,StatType.Speed,StatType.DmgIncrease,StatType.DmgReduction };

    public int Index { get; private set; } = -1;

    public void Init(InventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;
    }

    public void SetEquipmentMode()
    {
        UseButton.SetActive(false);
        EquipButton.SetActive(true);
    }
    public void SetItemMode()
    {
        EquipButton.SetActive(false);
        UseButton.SetActive(true);
    }

    public void ShowItemInfo(ItemData item, int index)
    {
        icon.sprite = item.Sprite;
        Index = index;
        StringBuilder equipStatusString = new();
        if (item.GetType() == typeof(EquipmentData))
        {
            EquipmentData equip = (EquipmentData)item;
            title.text = inventoryUI.DunUI.GetEquipmentName(equip.Key);
            for(int i=0; i<equip.Stats.Count; i++)
            {
                equipStatusString.Append(inventoryUI.DunUI.StatTypeToString(equip.Stats[i].statType));
                equipStatusString.Append(" : ");
                if (equip.Stats[i].val < 0) equipStatusString.Append("-");
                else equipStatusString.Append("+");
                equipStatusString.Append(equip.Stats[i].val);
                if (equip.Stats[i].statUnit == StatUnit.Percent) equipStatusString.Append("%");
                else if (percentPointStats.Contains(equip.Stats[i].statType)) equipStatusString.Append("%P");
                equipStatusString.AppendLine();
            }
            StringBuilder equipPotentialString = new();
            for (int i = 0; i < equip.Potentials.Count; i++)
            {
                equipPotentialString.Append(inventoryUI.DunUI.StatTypeToString(equip.Potentials[i].statType));
                equipPotentialString.Append(" : ");
                if (equip.Potentials[i].val < 0) equipPotentialString.Append("-");
                else equipPotentialString.Append("+");
                equipPotentialString.Append(equip.Potentials[i].val);
                if (equip.Potentials[i].statUnit == StatUnit.Percent) equipPotentialString.Append("%");
                else if (percentPointStats.Contains(equip.Potentials[i].statType)) equipPotentialString.Append("%P");
                equipPotentialString.AppendLine();
            }
            for (int i = 0; i < 3 - equip.PotentialExp / 10; i++)
            {
                equipPotentialString.Append("???");
                equipPotentialString.AppendLine();
            }
            potentialText.text = equipPotentialString.ToString();
        }
        else if(item.GetType() == typeof(MiscData))
        {
            MiscData misc = (MiscData)item;
            title.text = inventoryUI.DunUI.GetItemName(misc.Key);
            equipStatusString.Append(inventoryUI.DunUI.GetItemDescription(misc.Key, misc.EffectValues));
        }
        mainText.text = equipStatusString.ToString();

        gameObject.SetActive(true);
    }

    public void Close()
    {
        potentialText.text = "";
        gameObject.SetActive(false);
        if(background != null)
            background.SetActive(false);
    }

    public void Btn_EquipCLick()
    {
        inventoryUI.EquipEquipment(Index);
    }
    public void Btn_UnequipClick()
    {
        inventoryUI.UnEquipEquipment(Index);
    }
    public void Btn_UseClick()
    {
        inventoryUI.UseItem(Index);
    }
    public void Btn_DropClick()
    {
        inventoryUI.DropItem(Index, itemSlotType);
    }
    public void Btn_ThrowClick()
    {
        inventoryUI.ThrowItem(Index, itemSlotType);
    }
}
