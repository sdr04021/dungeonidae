using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InClass
{
    public List<InTier> tier;
}

[System.Serializable]
public class InTier
{
    public List<string> list;
}

[CreateAssetMenu(fileName = "StringData", menuName = "Scriptable Object/StringData")]
public class StringData : ScriptableObject
{
    [field:SerializeField] public List<string> Monsters { get; private set; }
    [field: Header("Item Drop Table")]
    [field: SerializeField] public List<InClass> Hats_Class { get; private set; }
    [field:SerializeField] public List<InClass> Weapons_Class { get; private set; }
    [field: SerializeField] public List<InClass> Armors_Class { get; private set; }
    [field: SerializeField] public List<InClass> Subs_Class { get; private set; }
    [field: SerializeField] public List<InClass> Shoes_Class { get; private set; }
    [field:SerializeField] public List<string> Artifacts { get; private set; }
    [field: SerializeField] public List<string> MiscItems { get; private set; }

    public List<List<string>> GetEquipKeyList(bool hats, bool weapons, bool armors, bool subs, bool shoes)
    {
        List<List<string>> result = new();

        for(int i=0; i<1; i++)
        {
            List<string> temp = new();
            if (hats) temp.AddRange(Hats_Class[0].tier[i].list);
            if (weapons) temp.AddRange(Weapons_Class[0].tier[i].list);
            if (armors) temp.AddRange(Armors_Class[0].tier[i].list);
            if (subs) temp.AddRange(Subs_Class[0].tier[i].list);
            if (shoes) temp.AddRange(Shoes_Class[0].tier[i].list);
            result.Add(temp);
        }
        return result;
    }
}
