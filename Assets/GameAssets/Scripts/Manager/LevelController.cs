using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Helper;

public class LevelController : MonoBehaviour
{
    public TargetPositionsData targetPositionsData;
    public Levels currentLevel;
    private List<Vector2> targetPositions;
    public List<GameObject> objectsToCheck;

    [Header("Level Settings")]
    [Tooltip("Number of touches required to trigger win check")]
    public int requiredTouches = 5; 
    private int shapesTouched = 0;

    [Header("Tutorial Setup")]
    public List<GameObject> tutorialShapes;

    private Dictionary<GameObject, Rigidbody> rigidbodyCache = new Dictionary<GameObject, Rigidbody>();
    private bool checkingShapes = false; // for check courotine

    private void Start()
    {
        GameEvents.LevelStart();
        LoadTargetPositions();
        CacheRigidbodies(); // save rb
    }

    private void LoadTargetPositions()
    {
        var levelData = targetPositionsData.GetLevelData(currentLevel);
        if (levelData != null)
        {
            targetPositions = new List<Vector2>(levelData.targetPositions);
            Debug.Log($" Loaded {targetPositions.Count} target positions for level: {currentLevel}");
        }
        else
        {
            Debug.LogError($" Level '{currentLevel}' not found in TargetPositionsData!");
        }
    }

    private void CacheRigidbodies()
    {
        rigidbodyCache.Clear();
        foreach (var obj in objectsToCheck)
        {
            if (obj != null)
            {
                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rigidbodyCache[obj] = rb;
                }
            }
        }
    }

    private void OnShapeTouched()
    {
        shapesTouched++;

        if (shapesTouched >= requiredTouches && !checkingShapes)
        {
            checkingShapes = true;
            Debug.Log(" All shapes have been touched, waiting for them to stop...");
            StartCoroutine(WaitForAllShapesToStop());
        }
    }

    private IEnumerator WaitForAllShapesToStop()
    {
        yield return new WaitForSeconds(1f);

        bool allStopped = false;
        while (!allStopped)
        {
            allStopped = true;
            foreach (var obj in rigidbodyCache.Keys)
            {
                Rigidbody rb = rigidbodyCache[obj];
                if (rb != null && rb.velocity.sqrMagnitude > 0.0025f)
                {
                    allStopped = false;
                    break;
                }
            }
            yield return null;
        }

        Debug.Log(" All shapes stopped. Checking win condition...");
        yield return new WaitForSeconds(1.5f);
        if (CheckWinCondition())
        {
            GameEvents.LevelComplete();
            UIManager.Instance.TurnOnWinPanel();
            Debug.Log(" Win!");
        }
        else
        {
            GameEvents.LevelLose();
        }
        checkingShapes = false;
    }

    private bool CheckWinCondition()
    {
        for (int i = 0; i < objectsToCheck.Count; i++)
        {
            Vector2 objectPosition = new Vector2(objectsToCheck[i].transform.position.x, objectsToCheck[i].transform.position.y);
            float distance = Vector2.Distance(objectPosition, targetPositions[i]);

            if (distance > 0.2f)
            {
                return false;
            }
        }

        Debug.Log("✅ You Win!");
        return true;
    }

    public void LevelLose()
    {
        Debug.Log("❌ You Lose!");
        UIManager.Instance.TurnOnLosePanel();
        StartCoroutine(HandleLevelLoss());
    }

    private IEnumerator HandleLevelLoss()
    {
        yield return new WaitForSeconds(2f);
        LevelManager.Instance.Restart();
    }

    private void OnEnable()
    {
        GameEvents.onShapeTouch += OnShapeTouched;
        GameEvents.onLevelLose += LevelLose;
    }

    private void OnDisable()
    {
        GameEvents.onShapeTouch -= OnShapeTouched;
        GameEvents.onLevelLose -= LevelLose;
    }
}
