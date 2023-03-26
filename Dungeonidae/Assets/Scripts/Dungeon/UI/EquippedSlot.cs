using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSlot : ItemSlot
{
    [SerializeField] Sprite equipmentTypeIcon;

    public override void SetItem(ItemData item, int listIndex)
    {
        icon.sprite = item.MySprite;
        ListIndex = listIndex;
        icon.gameObject.SetActive(true);
    }

    public override void RemoveItem()
    {
        icon.sprite = equipmentTypeIcon;
        ListIndex = -1;
    }

    public override void ItemSlotClicked()
    {
        if (ListIndex >= 0)
            inventoryUI.EquippedSlotClicked(ListIndex);
    }
}
