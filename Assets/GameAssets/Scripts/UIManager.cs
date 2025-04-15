using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public FadeEffect fade;
    public Image levelImageUI;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        TurnOffAllPanels();
    }

    public void TurnOnWinPanel()
    {
        if (winPanel)
        {
            winPanel.SetActive(true);
        }
    }

    public void TurnOnLosePanel()
    {
        if (losePanel)
        {
            losePanel.SetActive(true);
        }
    }

    private void TurnOffAllPanels()
    {
        if (winPanel && winPanel.activeSelf) winPanel.SetActive(false);
        if (losePanel && losePanel.activeSelf) losePanel.SetActive(false);
    }

    public void FadeIn()
    {
        fade.FadeIn();
    }
    public void FadeOut()
    {
        fade.FadeOut();
    }
    public void StartTutorial()
    {
        LevelManager.Instance.Restart();
        TutorialManager.Instance.StartTutorial();
    }

    private void OnEnable()
    {
        GameEvents.onLevelStart += TurnOffAllPanels;
    }
    private void OnDisable()
    {
        GameEvents.onLevelStart -= TurnOffAllPanels;
    }
}
