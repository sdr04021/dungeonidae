using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    bool isAccSelectMode = false;
    int accPlanToEquipIndex = -1;
    [SerializeField] GameObject itemInfoBg;
    [SerializeField] ItemInfo itemInfo;
    public bool IsEquipmentEnchantMode { get; private set; } = false;
    public bool IsArtifactEnchantMode { get; private set; } = false;
    [SerializeField] GameObject InventoryNotice;
    [SerializeField] TMP_Text InventoryNoticeText;

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
        CloseEquippedINfo();
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
        if (IsEquipmentEnchantMode || IsArtifactEnchantMode) CancelEnchantMode();
        if (isAccSelectMode) CancelArtifectSelectMode();
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
            if (dm.Player.UnitData.equipped[i] != null)
            {
                equippedSlots[i].SetItem(dm.Player.UnitData.equipped[i], i);
            }
            else equippedSlots[i].RemoveItem();
        }
    }

    public void UpdateEquipInventory()
    {
        List<EquipmentData> equips = dm.Player.UnitData.equipInventory;
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
        List<MiscData> miscs = dm.Player.UnitData.miscInventory;
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
            itemInfo.ShowItemInfo(dm.Player.UnitData.equipInventory[index], index);
        else if(miscSlotField.activeSelf)
            itemInfo.ShowItemInfo(dm.Player.UnitData.miscInventory[index], index);
    }

    public void CloseInfoBox()
    {
        itemInfo.gameObject.SetActive(false);
        itemInfoBg.SetActive(false);
        if (isAccSelectMode) CancelArtifectSelectMode();
    }

    public void EquippedSlotClicked(int index)
    {
        if (isAccSelectMode)
        {
            dm.Player.ExchangeEquipment(accPlanToEquipIndex, index);
            CancelArtifectSelectMode();
            dunUI.CloseMenuCanvas();
        }
        else
            equippedInfo.ShowItemInfo(dm.Player.UnitData.equipped[index], index);
    }
    public void CloseEquippedINfo()
    {
        equippedInfo.gameObject.SetActive(false);
    }

    public void EquipEquipment(int index)
    {
        CloseInfoBox();
        CloseEquippedINfo();

        EquipmentData equip = dm.Player.UnitData.equipInventory[index];

        if (equip.EquipmentType != EquipmentType.Artifact)
        {
            if (dm.Player.UnitData.equipped[(int)equip.EquipmentType] == null)
                dm.Player.EquipEquipment(index, (int)equip.EquipmentType);
            else
                dm.Player.ExchangeEquipment(index, (int)equip.EquipmentType);
            dunUI.CloseMenuCanvas();
        }
        else
        {
            for(int i=5; i<9; i++)
            {
                if (dm.Player.UnitData.equipped[i] == null)
                {
                    dm.Player.EquipEquipment(index, i);
                    dunUI.CloseMenuCanvas();
                    return;
                }
            }

            //¼±ÅÃ
            accPlanToEquipIndex = index;
            //accSelecterBg.SetActive(true);
            itemInfoBg.SetActive(true);
            isAccSelectMode = true;
            for (int i = 0; i < equipmentSlots.Count; i++)
            {
                if (i < GameManager.Instance.saveData.playerData.equipInventory.Count)
                {
                    if (GameManager.Instance.saveData.playerData.equipInventory[i].EquipmentType != EquipmentType.Artifact)
                    {
                        equipmentSlots[i].SetCurtain();
                    }
                }
                else equipmentSlots[i].SetCurtain();
            }
            for (int i = 0; i < 5; i++)
            {
                equippedSlots[i].SetCurtain();
            }
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
        CloseEquippedINfo();
        string key = GameManager.Instance.saveData.playerData.miscInventory[index].Key;
        if (key == "SCROLL_EQUIPMENT_ENCHANT")
            SetEnchantMode(false);
        else if (key == "SCROLL_ARTIFACT_ENCHANT")
            SetEnchantMode(true);
        else
        {
            dm.Player.UseItem(index);
            dunUI.CloseMenuCanvas();
        }
    }
    public void DropItem(int index, ItemSlotType itemSlotType)
    {
        if (equipmentSlotField.activeSelf)
        {
            dm.Player.DropEquip(index, itemSlotType);
        }
        else if (miscSlotField.activeSelf)
        {
            dm.Player.DropMisc(index);
        }
        dunUI.CloseMenuCanvas();
    }
    public void ThrowItem(int index, ItemSlotType itemSlotType)
    {
        if (equipmentSlotField.activeSelf)
        {
            dm.Player.PrepareThrowing(ItemType.Equipment, itemSlotType, index);
        }
        else if (miscSlotField.activeSelf)
        {
            dm.Player.PrepareThrowing(ItemType.Misc, itemSlotType, index);
        }

        //open cancel ui
        //

        dunUI.CloseMenuCanvas();
    }
    public void SetEnchantMode(bool isArtifact)
    {
        InventoryNotice.SetActive(true);
        Btn_EquipmentTabClicked();
        if (isArtifact)
        {
            IsArtifactEnchantMode = true;
            for(int i=0; i<equipmentSlots.Count; i++)
            {
                if (i < GameManager.Instance.saveData.playerData.equipInventory.Count)
                {
                    if (GameManager.Instance.saveData.playerData.equipInventory[i].EquipmentType != EquipmentType.Artifact)
                    {
                        equipmentSlots[i].SetCurtain();
                    }
                }
                else equipmentSlots[i].SetCurtain();
            }
            for(int i=0; i<5; i++)
            {
                equippedSlots[i].SetCurtain();
            }
        }
        else
        {
            IsEquipmentEnchantMode = true;
            for (int i = 0; i < equipmentSlots.Count; i++)
            {
                if (i < GameManager.Instance.saveData.playerData.equipInventory.Count)
                {
                    if (GameManager.Instance.saveData.playerData.equipInventory[i].EquipmentType == EquipmentType.Artifact)
                    {
                        equipmentSlots[i].SetCurtain();
                    }
                }
                else equipmentSlots[i].SetCurtain();
            }
            for (int i = 5; i < 9; i++)
            {
                equippedSlots[i].SetCurtain();
            }
        }
    }
    public void CancelEnchantMode()
    {
        InventoryNotice.SetActive(false);
        CloseInfoBox();
        CloseEquippedINfo();
        if (IsArtifactEnchantMode)
        {
            IsArtifactEnchantMode = false;
            for (int i = 0; i < equipmentSlots.Count; i++)
            {
                if (i < GameManager.Instance.saveData.playerData.equipInventory.Count)
                {
                    if (GameManager.Instance.saveData.playerData.equipInventory[i].EquipmentType != EquipmentType.Artifact)
                    {
                        equipmentSlots[i].RemoveCurtain();
                    }
                }
                else equipmentSlots[i].RemoveCurtain();
            }
            for (int i = 0; i < 5; i++)
            {
                equippedSlots[i].RemoveCurtain();
            }
        }
        else if (IsEquipmentEnchantMode)
        {
            IsEquipmentEnchantMode = false;
            for (int i = 0; i < equipmentSlots.Count; i++)
            {
                if (i < GameManager.Instance.saveData.playerData.equipInventory.Count)
                {
                    if (GameManager.Instance.saveData.playerData.equipInventory[i].EquipmentType == EquipmentType.Artifact)
                    {
                        equipmentSlots[i].RemoveCurtain();
                    }
                }
                else equipmentSlots[i].RemoveCurtain();
            }
            for (int i = 5; i < 9; i++)
            {
                equippedSlots[i].RemoveCurtain();
            }
        }
    }
    public void CancelArtifectSelectMode()
    {
        if (isAccSelectMode)
        {
            isAccSelectMode = false;
            InventoryNotice.SetActive(false);
            CloseInfoBox();
            CloseEquippedINfo();
            for (int i = 0; i < equipmentSlots.Count; i++)
            {
                if (i < GameManager.Instance.saveData.playerData.equipInventory.Count)
                {
                    if (GameManager.Instance.saveData.playerData.equipInventory[i].EquipmentType != EquipmentType.Artifact)
                    {
                        equipmentSlots[i].RemoveCurtain();
                    }
                }
                else equipmentSlots[i].RemoveCurtain();
            }
            for (int i = 0; i < 5; i++)
            {
                equippedSlots[i].RemoveCurtain();
            }
        }
    }
}
