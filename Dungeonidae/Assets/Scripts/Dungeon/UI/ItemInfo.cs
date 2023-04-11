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
    [SerializeField] TMP_Text additionalText;
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
        icon.sprite = item.Sprite;
        Index = index;
        StringBuilder equipStatusString = new();
        if (item is EquipmentData equip)
        {
            title.text = inventoryUI.DunUI.GetEquipmentName(equip.Key);
            foreach (var pair in equip.Stats)
            {
                equipStatusString.Append(inventoryUI.DunUI.StatTypeToString(pair.Key));
                equipStatusString.Append(" : +");
                equipStatusString.Append(pair.Value);
                equipStatusString.AppendLine();
            }
        }
        else if(item is MiscData misc)
        {
            title.text = inventoryUI.DunUI.GetItemName(misc.Key);
            equipStatusString.Append(inventoryUI.DunUI.GetItemDescription(misc.Key, misc.EffectValues));
        }
        mainText.text = equipStatusString.ToString();

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
