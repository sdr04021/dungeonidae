using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EquippedSlot : ItemSlot
{
    [SerializeField] Sprite equipmentTypeIcon;

    public override void SetItem(ItemData item, int listIndex)
    {
        icon.sprite = GameManager.Instance.GetSprite(SpriteAssetType.Equipment, item.Key);
        ListIndex = listIndex;
        icon.gameObject.SetActive(true);

        EquipmentData equip = (EquipmentData)item;
        if (equip.Enchant > 0)
        {
            enchantText.text = "+" + equip.Enchant.ToString();
            enchantText.gameObject.SetActive(true);
        }
        else enchantText.gameObject.SetActive(false);
    }

    public override void RemoveItem()
    {
        icon.sprite = equipmentTypeIcon;
        ListIndex = -1;
        enchantText.gameObject.SetActive(false);
    }

    public override void ItemSlotClicked()
    {
        if (ListIndex >= 0)
            inventoryUI.EquippedSlotClicked(ListIndex);
    }
}
