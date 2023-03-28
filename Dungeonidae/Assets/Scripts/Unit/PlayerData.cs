using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData
{
    public int maxEquip = 20;
    public List<EquipmentData> equipInventory = new();
    public int maxMisc = 20;
    public List<MiscData> miscInventory = new();

    public EquipmentData[] equipped = new EquipmentData[9];

    [SerializeField] List<AbilityData> abilities = new();
    public List<AbilityData> Abilities { get => abilities; }
    public Dictionary<string,int> AbilityNameToIndex { get; private set; } = new Dictionary<string,int>();

    public int abilityPoint = 0;

    public bool AddEquipment(EquipmentData equip)
    {
        if (equipInventory.Count >= maxEquip)
            return false;
        else
        {
            equipInventory.Add(equip);
            return true;
        }
    }

    public bool AddMisc(MiscData misc)
    {
        for (int i = miscInventory.Count - 1; i >= 0; i--)
        {
            if ((miscInventory[i].Key == misc.Key) && (misc.Amount < misc.MaxStack))
            {
                if (misc.Amount <= (miscInventory[i].AmountLeft))
                {
                    miscInventory[i].AddAmount(misc.Amount);
                    return true;
                }
                else
                {
                    misc.AddAmount(-miscInventory[i].AmountLeft);
                    miscInventory[i].AddAmount(miscInventory[i].AmountLeft);
                    break;
                }
            }
        }

        if (miscInventory.Count >= maxMisc)
            return false;
        else
        {
            miscInventory.Add(misc);
            return true;
        }
    }

    public void RemoveOneMisc(int index)
    {
        miscInventory[index].AddAmount(-1);
        if (miscInventory[index].Amount<=0)
            miscInventory.RemoveAt(index);
    }

    public void AddAbility(AbilityData ability)
    {
        abilities.Add(ability);
        AbilityNameToIndex.Add(ability.Key, abilities.Count - 1);
    }
}
