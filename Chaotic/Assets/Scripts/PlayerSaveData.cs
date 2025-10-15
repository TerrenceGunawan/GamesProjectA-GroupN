using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerSaveData
{
    public string sceneName;
    public float sanity;
    public float[] position;        // store as array for JSON
    public float[] rotation;
    public List<string> inventory;
    public List<string> completedPuzzles;
    public List<string> completedObjects;
    public List<string> completedPhoneTwice;
    public List<ItemTransformData> itemTransforms = new List<ItemTransformData>();
}
