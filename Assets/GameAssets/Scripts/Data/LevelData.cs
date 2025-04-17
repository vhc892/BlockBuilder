using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using Helper;


[CreateAssetMenu(fileName = "NewLevelData", menuName = "Game/Level Data")]
public class LevelData : ScriptableObject
{
    [TableList]
    public List<LevelDataPrefabImage> levelPrefabs;
    //public GameObject[] levelPrefabs;
    //public Sprite[] levelImages;
}
