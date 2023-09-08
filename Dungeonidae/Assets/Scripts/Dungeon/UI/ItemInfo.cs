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
    [SerializeField] GameObject enchantButton;
    [SerializeField] TMP_Text enchantButtonText;

    ItemSlotType itemSlotType;

    public int Index { get; private set; } = -1;

    public void Init(InventoryUI inventoryUI)
    {
        this.inventoryUI = inventoryUI;
    }

    void SetEquipmentMode()
    {
        UseButton.SetActive(false);
        EquipButton.SetActive(true);
    }
    void SetItemMode()
    {
        EquipButton.SetActive(false);
        UseButton.SetActive(true);
    }

    public void ShowItemInfo(ItemData item, int index, ItemSlotType itemSlotType)
    {
        mainText.text = "";
        Index = index;
        this.itemSlotType = itemSlotType;
        StringBuilder itemString = new();
        if (item.GetType() == typeof(EquipmentData))
        {
            SetEquipmentMode();
            EquipmentData equip = (EquipmentData)item;
            StringBuilder titleString = new();
            if (equip.Enchant > 0)
            {
                titleString.Append("(+");
                titleString.Append(equip.Enchant);
                titleString.Append(") ");
            }
            titleString.Append(inventoryUI.DunUI.GetEquipmentName(equip.Key));
            title.text = titleString.ToString();
            icon.sprite = GameManager.Instance.GetSprite(SpriteAssetType.Equipment, equip.Key);
            for(int i=0; i<equip.Stats.Count; i++)
            {
                itemString.Append(inventoryUI.DunUI.StatTypeToString(equip.Stats[i].statType));
                itemString.Append(" : ");
                if (equip.Stats[i].val > 0) itemString.Append("+");
                if (equip.Stats[i].bonus > 0)
                    itemString.Append("<color=#FFEA00>");
                itemString.Append(equip.Stats[i].val + equip.Stats[i].bonus);
                if (equip.Stats[i].bonus > 0)
                {
                    itemString.Append("<color=#FFFFFF>");
                    itemString.Append("(");
                    itemString.Append(equip.Stats[i].val);
                    itemString.Append("+");
                    itemString.Append(equip.Stats[i].bonus);
                    itemString.Append(")");
                }
                if (equip.Stats[i].statUnit == StatUnit.Percent) itemString.Append("%");
                else if (Constants.PercentPointStats.Contains(equip.Stats[i].statType)) itemString.Append("%P");
                itemString.AppendLine();
            }
            //itemString.AppendLine();
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
            EquipmentBase equipBase = GameManager.Instance.GetEquipmentBase(equip.Key);
            if (equipBase.abilities?.Length > 0)
            {
                //itemString.AppendLine();
                itemString.Append("<color=#00BFFF>");
                for (int i = 0; i < equipBase.abilities.Length; i++)
                {
                    itemString.Append("¡¤ ");
                    if (equipBase.abilities[i].vals?.Count > 0)
                    {
                        if (i == 0)
                        {
                            List<int> temp = new();
                            for (int j = 0; j < equipBase.abilities[i].vals.Count; j++)
                            {
                                temp.Add(equipBase.abilities[i].vals[j] + equip.EquipmentAblitiyBonus[j]);
                            }
                            itemString.Append(inventoryUI.DunUI.GetEquipmentAbilitiy(equipBase.abilities[i].key.ToString(), temp));
                        }
                        else
                            itemString.Append(inventoryUI.DunUI.GetEquipmentAbilitiy(equipBase.abilities[i].key.ToString(), equipBase.abilities[i].vals));
                    }
                    else
                    {
                        itemString.Append(inventoryUI.DunUI.GetEquipmentAbilitiy(equipBase.abilities[i].key.ToString()));
                    }
                    itemString.AppendLine();
                }
                itemString.Append("<color=#FFFFFF>");
            }
        }
        else if(item.GetType() == typeof(MiscData))
        {
            SetItemMode();
            MiscData misc = (MiscData)item;
            title.text = inventoryUI.DunUI.GetItemName(misc.Key);
            icon.sprite = GameManager.Instance.GetSprite(SpriteAssetType.Misc, misc.Key);
            itemString.Append(inventoryUI.DunUI.GetItemDescription(misc.Key, GameManager.Instance.GetMiscBase(misc.Key).EffectValues));
            if(GameManager.Instance.GetMiscBase(misc.Key).IsUsable) UseButton.SetActive(true);
            else UseButton.SetActive(false);
        }
        mainText.text = itemString.ToString();

        if(inventoryUI.IsArtifactEnchantMode || inventoryUI.IsEquipmentEnchantMode)
        {
            enchantButton.SetActive(true);
        }
        else enchantButton.SetActive(false);

        gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(content);
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
    public void Btn_EnchantClick()
    {
        if (itemSlotType == ItemSlotType.Equipped)
        {
            GameManager.Instance.saveData.playerData.equipped[Index].EnchantEquipment();
        }
        else if(itemSlotType == ItemSlotType.Item)
        {
            GameManager.Instance.saveData.playerData.equipInventory[Index].EnchantEquipment();
        }
        if(inventoryUI.IsEquipmentEnchantMode)
            GameManager.Instance.saveData.playerData.RemoveOneMisc("SCROLL_EQUIPMENT_ENCHANT");
        else if(inventoryUI.IsArtifactEnchantMode)
            GameManager.Instance.saveData.playerData.RemoveOneMisc("SCROLL_ARTIFACT_ENCHANT");
        inventoryUI.CancelEnchantMode();
        inventoryUI.Refresh();
    }
}
