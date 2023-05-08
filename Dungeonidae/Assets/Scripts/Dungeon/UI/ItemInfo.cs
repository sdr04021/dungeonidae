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
    [SerializeField] GameObject EquipButton;
    [SerializeField] GameObject UseButton;

    [SerializeField] ItemSlotType itemSlotType;

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
        mainText.text = "";
        icon.sprite = item.Sprite;
        Index = index;
        StringBuilder itemString = new();
        if (item.GetType() == typeof(EquipmentData))
        {
            EquipmentData equip = (EquipmentData)item;
            title.text = inventoryUI.DunUI.GetEquipmentName(equip.Key);
            for(int i=0; i<equip.Stats.Count; i++)
            {
                itemString.Append(inventoryUI.DunUI.StatTypeToString(equip.Stats[i].statType));
                itemString.Append(" : ");
                if (equip.Stats[i].val > 0) itemString.Append("+");
                itemString.Append(equip.Stats[i].val);
                if (equip.Stats[i].statUnit == StatUnit.Percent) itemString.Append("%");
                else if (Constants.PercentPointStats.Contains(equip.Stats[i].statType)) itemString.Append("%P");
                itemString.AppendLine();
            }
            itemString.AppendLine();
            itemString.Append("<color=#87CEEB>");
            for (int i = 0; i < equip.Potentials.Count; i++)
            {
                itemString.Append(inventoryUI.DunUI.StatTypeToString(equip.Potentials[i].statType));
                itemString.Append(" : ");
                if (equip.Potentials[i].val > 0) itemString.Append("+");
                itemString.Append(equip.Potentials[i].val);
                if (equip.Potentials[i].statUnit == StatUnit.Percent) itemString.Append("%");
                else if (Constants.PercentPointStats.Contains(equip.Potentials[i].statType)) itemString.Append("%P");
                itemString.AppendLine();
            }
            for (int i = 0; i < 3 - equip.PotentialExp / 10; i++)
            {
                itemString.Append("???");
                itemString.AppendLine();
            }
        }
        else if(item.GetType() == typeof(MiscData))
        {
            MiscData misc = (MiscData)item;
            title.text = inventoryUI.DunUI.GetItemName(misc.Key);
            itemString.Append(inventoryUI.DunUI.GetItemDescription(misc.Key, misc.EffectValues));
        }
        mainText.text = itemString.ToString();

        gameObject.SetActive(true);
    }

    public void Close()
    {
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
