using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class BuffData
{
    [field: SerializeField]
    public string Key { get; private set; }

    [field: SerializeField]
    public int MaxDuration { get; private set; }    
    public int durationLeft = 0;

    public BuffData(BuffBase buff, int maxDuration)
    {       
        Key = buff.key;
        MaxDuration = maxDuration;
        durationLeft = maxDuration;
    }
}
