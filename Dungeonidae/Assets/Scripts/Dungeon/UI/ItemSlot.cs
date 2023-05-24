using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    protected InventoryUI inventoryUI;
    Button thisButton;
    [SerializeField] protected Image icon;
    [SerializeField] TMP_Text amountText;
    [SerializeField] protected TMP_Text enchantText;
    public int ListIndex { get; protected set; } = -1;
    [SerializeField] GameObject curtain;

    private void Awake()
    {
        thisButton = GetComponent<Button>();
    }

    public void Init(InventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;
    }

    public virtual void SetItem(ItemData item, int listIndex)
    {
        ListIndex = listIndex;
        icon.gameObject.SetActive(true);

        if (item.GetType() == typeof(MiscData))
        {
            MiscData misc = (MiscData)item;
            icon.sprite = GameManager.Instance.GetSprite(SpriteAssetType.Misc, misc.Key);
            amountText.text = misc.Amount.ToString();
            amountText.gameObject.SetActive(true);
        }
        else if(item.GetType() == typeof(EquipmentData))
        {
            EquipmentData equip = (EquipmentData)item;
            icon.sprite = GameManager.Instance.GetSprite(SpriteAssetType.Equipment, item.Key);
            if (equip.Enchant > 0)
            {
                enchantText.text = "+" + equip.Enchant.ToString();
                enchantText.gameObject.SetActive(true);
            }
            else enchantText.gameObject.SetActive(false);
            amountText.gameObject.SetActive(false);
        }
    }

    public virtual void RemoveItem()
    {
        icon.gameObject.SetActive(false);
        amountText.gameObject.SetActive(false);
        enchantText.gameObject.SetActive(false);
        ListIndex = -1;
    }

    public virtual void ItemSlotClicked()
    {
        if(ListIndex>=0)
            inventoryUI.SlotClicked(ListIndex);
    }

    public void SetCurtain()
    {
        curtain.gameObject.SetActive(true);
        thisButton.interactable = false;
    }
    public void RemoveCurtain()
    {
        curtain.gameObject.SetActive(false);
        thisButton.interactable = true;
    }
}
