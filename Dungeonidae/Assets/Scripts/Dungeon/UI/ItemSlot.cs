using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    protected InventoryUI inventoryUI;
    [SerializeField] protected Image icon;
    [SerializeField] TMP_Text amountText;
    public int ListIndex { get; protected set; } = -1;

    public void Init(InventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;
    }

    public virtual void SetItem(ItemData item, int listIndex)
    {
        icon.sprite = item.MySprite;
        ListIndex = listIndex;
        icon.gameObject.SetActive(true);

        if (item is MiscData misc)
        {
            amountText.text = misc.Amount.ToString();
            amountText.gameObject.SetActive(true);
        }
        else amountText.gameObject.SetActive(false);
    }

    public virtual void RemoveItem()
    {
        icon.gameObject.SetActive(false);
        ListIndex = -1;
    }

    public virtual void ItemSlotClicked()
    {
        if(ListIndex>=0)
            inventoryUI.SlotClicked(ListIndex);
    }
}
