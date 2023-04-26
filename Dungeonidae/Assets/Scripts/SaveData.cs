using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    [JsonProperty] public int Seed { get; private set; }
    [JsonIgnore] public List<int> FloorSeeds { get; private set; }
    [JsonIgnore] public System.Random Rand { get; private set; }
    [JsonProperty]
    public List<DungeonData> Floors { get; private set; } = new();
    public int currentFloor = 0;

    [HideInInspector] public UnitData playerData = null;
    [JsonIgnore] public List<int> MonsterLayout { get; private set; } = new();

    public void SetSeed()
    {
        Seed = Random.Range(int.MinValue, int.MaxValue);
    }
    public void SetRand()
    {
        Rand = new(Seed);
        FloorSeeds = new();
    }
    public void UpdateFloorSeeds()
    {
        for(int i=0; i<Floors.Count; i++)
        {
            if (i >= FloorSeeds.Count)
            {
                FloorSeeds.Add(Rand.Next());
            }
        }
    }
    public void SetMonstersLayout()
    {
        System.Random rand = new(Seed);
        List<int> deck = new();
        for (int i = 0; i < GameManager.Instance.StringData.Monsters.Count; i++)
        {
            deck.Add(i);
        }

        for (int i = 0; i < GameManager.Instance.StringData.Monsters.Count; i++)
        {
            if (i == 0)
            {
                MonsterLayout.Add(deck[0]);
                deck.RemoveAt(0);
            }
            else
            {
                int pick = rand.Next(0, deck.Count);
                MonsterLayout.Add(deck[pick]);
                deck.RemoveAt(pick);
            }
        }
    }
    public void SetMonsterLayout()
    {

    }


    public DungeonData GetCurrentDungeonData()
    {
        return Floors[currentFloor];
    }

    static JsonSerializerSettings serializerSettings = new() { TypeNameHandling = TypeNameHandling.Auto };
    public static void Save(SaveData saveData)
    {
        string data = JsonConvert.SerializeObject(saveData, Formatting.Indented, serializerSettings);
        File.WriteAllText(Application.dataPath + "/save.json", data);
    }

    public static SaveData Load()
    {
        if (File.Exists(Application.dataPath + "/save.json"))
        {
            string data = File.ReadAllText(Application.dataPath + "/save.json");
            return JsonConvert.DeserializeObject<SaveData>(data, serializerSettings);
        }
        else return null;
    }
}
