using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] DungeonManager dm;
    [SerializeField] DungeonUIManager dunUI;
    public DungeonUIManager DunUI { get => dunUI; }

    [SerializeField] EquippedSlot[] equippedSlots = new EquippedSlot[9];
    [SerializeField] Image equipmentTabButton;
    [SerializeField] GameObject equipmentSlotField;
    [SerializeField] List<ItemSlot> equipmentSlots = new();
    [SerializeField] Image miscTabButton;
    [SerializeField] GameObject miscSlotField;
    [SerializeField] List<ItemSlot> miscSlots = new();

    [SerializeField] ItemInfo equippedInfo;
    [SerializeField] GameObject accSelecterBg;
    bool isAccSelectMode = false;
    int accPlanToEquipIndex = -1;
    [SerializeField] GameObject itemInfoBg;
    [SerializeField] ItemInfo itemInfo;

    Color halfTransparentWhite = new(1, 1, 1, 0.5f);

    public void Init()
    {
        for(int i=0; i<9; i++)
        {
            equippedSlots[i].Init(this);
        }

        for(int i=0; i< equipmentSlots.Count; i++)
        {
            equipmentSlots[i].Init(this);
        }

        for(int i=0; i<miscSlots.Count; i++)
        {
            miscSlots[i].Init(this);
        }
        
        equippedInfo.Init(this);
        itemInfo.Init(this);
    }
    
    public void Refresh()
    {
        UpdateEquippedSlots();
        UpdateEquipInventory();
        UpdateMiscInventory();
        CloseInfoBox();
        CloseEquippedINfo();
    }

    public void Btn_EquipmentTabClicked()
    {
        CloseInfoBox();
        UpdateEquipInventory();
        itemInfo.SetEquipmentMode();
        equipmentTabButton.color = Color.white;
        miscTabButton.color = halfTransparentWhite;
        miscSlotField.SetActive(false);
        equipmentSlotField.SetActive(true);
    }
    public void Btn_ItemTabClicked()
    {
        CloseInfoBox();
        CloseEquippedINfo();
        UpdateMiscInventory();
        itemInfo.SetItemMode();
        miscTabButton.color = Color.white;
        equipmentTabButton.color = halfTransparentWhite;
        miscSlotField.SetActive(true);
        equipmentSlotField.SetActive(false);
    }

    public void UpdateEquippedSlots()
    {
        for(int i=0; i<9; i++)
        {
            if (dm.Player.PlayerData.equipped[i] != null)
            {
                equippedSlots[i].SetItem(dm.Player.PlayerData.equipped[i], i);
            }
            else equippedSlots[i].RemoveItem();
        }
    }

    public void UpdateEquipInventory()
    {
        List<EquipmentData> equips = dm.Player.PlayerData.equipInventory;
        for(int i=0; i< equipmentSlots.Count; i++)
        {
            if (i < equips.Count)
            {
                equipmentSlots[i].SetItem(equips[i], i);
            }
            else
            {
                equipmentSlots[i].RemoveItem();
            }

        }
    }

    public void UpdateMiscInventory()
    {
        List<MiscData> miscs = dm.Player.PlayerData.miscInventory;
        for(int i=0; i< miscSlots.Count; i++)
        {
            if(i < miscs.Count)
            {
                miscSlots[i].SetItem(miscs[i], i);
            }
            else
            {
                miscSlots[i].RemoveItem();
            }
        }
    }

    public void SlotClicked(int index)
    {
        itemInfoBg.SetActive(true);
        if (equipmentSlotField.activeSelf)
            itemInfo.ShowItemInfo(dm.Player.PlayerData.equipInventory[index], index);
        else if(miscSlotField.activeSelf)
            itemInfo.ShowItemInfo(dm.Player.PlayerData.miscInventory[index], index);
    }

    public void CloseInfoBox()
    {
        itemInfo.gameObject.SetActive(false);
        itemInfoBg.SetActive(false);
    }

    public void EquippedSlotClicked(int index)
    {
        if (isAccSelectMode)
        {
            dm.Player.ExchangeEquipment(accPlanToEquipIndex, index);
            accSelecterBg.SetActive(false);
            isAccSelectMode = false;
            dunUI.CloseMenuCanvas();
        }
        else
            equippedInfo.ShowItemInfo(dm.Player.PlayerData.equipped[index], index);
    }
    public void CloseEquippedINfo()
    {
        equippedInfo.gameObject.SetActive(false);
    }

    public void EquipEquipment(int index)
    {
        CloseInfoBox();
        CloseEquippedINfo();

        EquipmentData equip = dm.Player.PlayerData.equipInventory[index];

        if (equip.EquipmentType != EquipmentType.Accessory)
        {
            if (dm.Player.PlayerData.equipped[(int)equip.EquipmentType] == null)
                dm.Player.EquipEquipment(index, (int)equip.EquipmentType);
            else
                dm.Player.ExchangeEquipment(index, (int)equip.EquipmentType);
            dunUI.CloseMenuCanvas();
        }
        else
        {
            for(int i=5; i<9; i++)
            {
                if (dm.Player.PlayerData.equipped[i] == null)
                {
                    dm.Player.EquipEquipment(index, i);
                    dunUI.CloseMenuCanvas();
                    return;
                }
            }

            //¼±ÅÃ
            accPlanToEquipIndex = index;
            accSelecterBg.SetActive(true);
            itemInfoBg.SetActive(true);
            isAccSelectMode = true;
        }
    }
    public void UnEquipEquipment(int index)
    {
        if (dm.Player.UnequipEquipment(index))
        {
            UpdateEquippedSlots();
            UpdateEquipInventory();
            dunUI.CloseMenuCanvas();
        }
        else
        {
            
        } 
    }
    public void UseItem(int index)
    {
        CloseInfoBox();
        dm.Player.UseItem(index);
        dunUI.CloseMenuCanvas();
    }
    public void DropItem(int index)
    {
        if (equipmentSlotField.activeSelf)
        {
            dm.Player.DropEquip(index);
        }
        else if (miscSlotField.activeSelf)
        {
            dm.Player.DropMisc(index);
        }
        dunUI.CloseMenuCanvas();
    }
    public void ThrowItem(int index)
    {
        if (equipmentSlotField.activeSelf)
        {
            dm.Player.PrepareThrowing(ItemType.Equipment, index);
        }
        else if (miscSlotField.activeSelf)
        {
            dm.Player.PrepareThrowing(ItemType.Misc, index);
        }

        //open cancel ui
        //

        dunUI.CloseMenuCanvas();
    }
}
