/*
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CreatePrefab
{
    [MenuItem("Custom/CreatePrefab")]
    static void CreatePrefabs()
    {
        /*
        DungeonManager dm = GameObject.Find("DungeonManager").GetComponent<DungeonManager>();
        dm.map = new();
        for (int i = 0; i < 100; i++)
        {
            Arr2DRow<Tile> temp = new();
            for (int j = 0; j < 100; j++)
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Prefabs/Tile.prefab"));
                prefab.name = "Tile(" + i.ToString() + ", " + j.ToString() + ")";
                prefab.transform.localPosition = new Vector3(i, j, 0);
                temp.y.Add(prefab.GetComponent<Tile>());
            }
            dm.map.x.Add(temp);
        }
        */
        /*
        DungeonManager dm = GameObject.Find("DungeonManager").GetComponent<DungeonManager>();
        dm.fogMap = new();
        for (int i = 0; i < 100; i++)
        {
            Arr2DRow<Fog> temp = new();
            for (int j = 0; j < 100; j++)
            {
                GameObject prefab = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<Object>("Assets/Prefabs/Fog.prefab"));
                prefab.name = "Fog(" + i.ToString() + ", " + j.ToString() + ")";
                prefab.transform.localPosition = new Vector3(i, j, 0);
                temp.y.Add(prefab.GetComponent<Fog>());
            }
            dm.fogMap.x.Add(temp);
        }
    }

}
*/