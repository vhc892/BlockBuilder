using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.Collections.Generic;
using Helper;

public class TargetPositionsEditor : OdinEditorWindow
{
    [Title("Auto-Save Target Positions", Bold = true)]
    [InlineEditor(Expanded = true)]
    [SerializeField]
    private LevelData levelData;

    [InlineEditor(Expanded = true)]
    [SerializeField]
    private TargetPositionsData targetPositionsData;

    [Space(10)]
    [Title("Level Selection")]
    public Levels selectedLevel;

    [Space(10)]
    [Title("Objects to Save")]
    [ListDrawerSettings(ShowFoldout = true, DefaultExpandedState = true, DraggableItems = true)]
    [SerializeField] private List<GameObject> selectedObjects = new List<GameObject>();

    [MenuItem("Tools/Target Positions Editor")]
    public static void ShowWindow()
    {
        GetWindow<TargetPositionsEditor>("Target Positions Editor").Show();
    }

    [Button("Add Target Positions", ButtonSizes.Large), GUIColor(0.3f, 0.8f, 0.3f)]
    private void SaveTargetPositions()
    {
        if (targetPositionsData != null)
        {
            var levelData = targetPositionsData.GetLevelData(selectedLevel);
            if (levelData == null)
            {
                levelData = new TargetPositionsData.LevelData { levelName = selectedLevel };
                targetPositionsData.levels.Add(levelData);
            }

            foreach (var obj in selectedObjects)
            {
                if (obj != null)
                {
                    Vector2 newPosition = new Vector2(obj.transform.position.x, obj.transform.position.y);
                    levelData.targetPositions.Add(newPosition);
                    Debug.Log($"✅ Saved Position: {newPosition} for Level '{selectedLevel}' from '{obj.name}'");
                }
            }

            EditorUtility.SetDirty(targetPositionsData);
        }
        else
        {
            Debug.LogWarning("❌ Please assign TargetPositionsData!");
        }
    }

    [Button("Clear Target Positions", ButtonSizes.Large), GUIColor(0.9f, 0.3f, 0.3f)]
    private void ClearTargetPositions()
    {
        if (targetPositionsData != null)
        {
            var levelData = targetPositionsData.GetLevelData(selectedLevel);
            if (levelData != null)
            {
                levelData.targetPositions.Clear();
                EditorUtility.SetDirty(targetPositionsData);
                Debug.Log($"✅ Cleared Target Positions for Level '{selectedLevel}'");
            }
        }
        else
        {
            Debug.LogWarning("❌ Please assign TargetPositionsData!");
        }
    }
}
