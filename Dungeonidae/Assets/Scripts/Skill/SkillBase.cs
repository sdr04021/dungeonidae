using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillBase", menuName = "Scriptable Object/SkillBase")]
public class SkillBase : ScriptableObject
{
    public string key;
    public SkillType type;
    public Sprite sprite;
    public int[] effectValues;
    public int[] mutableValueIndex;
    public bool needTarget;
}
