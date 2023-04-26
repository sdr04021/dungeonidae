using Mono.Collections.Generic;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StringData", menuName = "Scriptable Object/StringData")]
public class StringData : ScriptableObject
{
    [field:SerializeField] public List<string> Monsters { get; private set; }
}
