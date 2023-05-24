using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ForClass
{
    public List<string> list;
}

[System.Serializable]
public class ForTier
{
    public List<ForClass> forClass;
}

[CreateAssetMenu(fileName = "StringData", menuName = "Scriptable Object/StringData")]
public class StringData : ScriptableObject
{
    [field:SerializeField] public List<string> Monsters { get; private set; }
    [field: Header("Equipment Drop Table")] [field:SerializeField] public List<ForTier> EquipTier { get; private set; }
    [field:SerializeField] public List<string> MiscItems { get; private set; }
}
