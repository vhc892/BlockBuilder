using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Helper;

[CreateAssetMenu(fileName = "NewTargetPositions", menuName = "Game/Target Positions Data")]
public class TargetPositionsData : ScriptableObject
{
    [System.Serializable]
    public class LevelData
    {
        public Levels levelName;
        public List<Vector2> targetPositions = new List<Vector2>();
    }

    public List<LevelData> levels = new List<LevelData>();

    // Tìm LevelData theo enum Levels thay vì string
    public LevelData GetLevelData(Levels level)
    {
        return levels.Find(l => l.levelName == level);
    }
}
