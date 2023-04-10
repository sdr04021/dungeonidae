using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [JsonProperty] public int Seed { get; private set; }
    [JsonIgnore] public List<int> Seeds { get; private set; }
    [JsonProperty]
    public List<DungeonData> Floors { get; private set; } = new();
    public int currentFloor = 0;

    public UnitData playerData = null;

    public void SetSeed()
    {
        Seed = Random.Range(int.MinValue, int.MaxValue);
        GenerateSeedList();
    }

    public void GenerateSeedList()
    {
        System.Random random = new(Seed);
        Seeds = new();
        for (int i = 0; i < 100; i++)
        {
            Seeds.Add(random.Next());
        }
    }

    public DungeonData GetCurrentDungeonData()
    {
        return Floors[currentFloor];
    }

    public static void Save(SaveData saveData)
    {
        string data = JsonConvert.SerializeObject(saveData, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/save.json", data);
    }

    public static SaveData Load()
    {
        if (File.Exists(Application.dataPath + "/save.json"))
        {
            string data = File.ReadAllText(Application.dataPath + "/save.json");
            return JsonConvert.DeserializeObject<SaveData>(data);
        }
        else return null;
    }
}
