using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffBase", menuName = "Scriptable Object/BuffBase")]
public class BuffBase : ScriptableObject
{
    public string key;
    public BuffType buffType;
    public Sprite sprite;
    public List<int> effectValues;  
}
