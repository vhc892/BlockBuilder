using System.Collections;
using UnityEngine;
using Helper;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public LevelData levelData;
    private GameObject currentLevelInstance;
    private int currentLevelIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            LoadGame();
        }
    }

    void Start()
    {
#if !UNITY_EDITOR
            Application.targetFrameRate = 60;
#endif
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (currentLevelInstance != null)
        {
            StartCoroutine(WaitForFadeOutAndLoadLevel(index));
        }
        else
        {
            StartCoroutine(LoadLevelAfterFade(index));
        }
    }

    private IEnumerator WaitForFadeOutAndLoadLevel(int index)
    {
        UIManager.Instance.FadeOut();
        yield return new WaitForSeconds(UIManager.Instance.fade.fadeDuration);

        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        yield return StartCoroutine(LoadLevelAfterFade(index));
    }

    private IEnumerator LoadLevelAfterFade(int index)
    {
        if (index < levelData.levelPrefabs.Count)
        {
            currentLevelIndex = index;
            LevelDataPrefabImage levelInfo = levelData.levelPrefabs[index];

            if (levelInfo.prefabs != null)
            {
                currentLevelInstance = Instantiate(levelInfo.prefabs, Vector3.zero, Quaternion.identity);
            }

            UpdateLevelUI(index);
        }
        else
        {
            Debug.Log("All levels completed!");
            yield break;
        }

        yield return new WaitForSeconds(0.1f);
        UIManager.Instance.FadeIn();
    }

    public void NextLevel()
    {
        if (currentLevelIndex + 1 < levelData.levelPrefabs.Count)
        {
            LoadLevel(currentLevelIndex + 1);
        }
        else
        {
            Debug.Log("Game Finished!");
        }
    }

    public void SkipLevel()
    {
        if (currentLevelIndex + 1 < levelData.levelPrefabs.Count)
        {
            SaveGame();
            LoadLevel(currentLevelIndex + 1);
        }
        else
        {
            Debug.Log("Game Finished!");
        }
    }

    public void Restart()
    {
        if(TutorialManager.Instance.isTutorialActive == true)
        {
            TutorialManager.Instance.isTutorialActive = false;
            LoadLevel(currentLevelIndex);
            TutorialManager.Instance.StartTutorial();
        }
        else
        {
            LoadLevel(currentLevelIndex);
        }
    }

    public LevelController GetCurrentLevelController()
    {
        if (currentLevelInstance != null)
        {
            return currentLevelInstance.GetComponent<LevelController>();
        }
        return null;
    }

    private void SaveGame()
    {
        GameData data = new GameData { currentLevelIndex = currentLevelIndex + 1 };
        SaveSystem.SaveGame(data);
    }

    private void LoadGame()
    {
        GameData data = SaveSystem.LoadGame();
        currentLevelIndex = data.currentLevelIndex;
    }

    private void UpdateLevelUI(int index)
    {
        if (UIManager.Instance.levelImageUI != null && index < levelData.levelPrefabs.Count)
        {
            UIManager.Instance.levelImageUI.sprite = levelData.levelPrefabs[index].levelImage;
        }
    }

    private void OnEnable()
    {
        GameEvents.onLevelComplete += SaveGame;
    }

    private void OnDisable()
    {
        GameEvents.onLevelComplete -= SaveGame;
    }
}
