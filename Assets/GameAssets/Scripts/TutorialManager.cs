using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    public GameObject handImage;
    private LevelController currentLevelController;
    private int currentTutorialIndex = 0;
    private Camera mainCamera;
    public bool isTutorialActive = false;

    private Coroutine outlineCoroutine;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void StartTutorial()
    {
        if (isTutorialActive) return;

        isTutorialActive = true;
        currentTutorialIndex = 0;
        StartCoroutine(WaitForRestartAndStartTutorial());
    }

    private IEnumerator WaitForRestartAndStartTutorial()
    {
        // wait end fade
        yield return new WaitForSeconds(UIManager.Instance.fade.fadeDuration);

        currentLevelController = FindObjectOfType<LevelController>();

        if (currentLevelController == null || currentLevelController.tutorialShapes.Count == 0)
        {
            Debug.LogWarning("No tutorial shapes found in the level!");
            isTutorialActive = false;
            yield break;
        }

        // can't touch all shape
        SetAllShapesInteractable(false);

        // can touch 1
        EnableNextShape();
    }

    // show hand
    private void ShowHandAtCurrentShape()
    {
        if (currentTutorialIndex >= currentLevelController.tutorialShapes.Count) return;

        GameObject targetShape = currentLevelController.tutorialShapes[currentTutorialIndex];
        if (targetShape == null || handImage == null) return;

        Vector3 worldPosition = targetShape.transform.position;
        worldPosition.y -= 0.7f;
        worldPosition.x += 0.3f;

        handImage.transform.position = mainCamera.WorldToScreenPoint(worldPosition);
        handImage.SetActive(true);
    }

    private void EnableNextShape()
    {
        if (currentTutorialIndex < currentLevelController.tutorialShapes.Count)
        {
            GameObject targetShape = currentLevelController.tutorialShapes[currentTutorialIndex];

            SetAllShapesInteractable(false);

            //normal shape
            Shape shapeComponent = targetShape.GetComponent<Shape>();
            if (shapeComponent != null)
            {
                shapeComponent.SetInteractable(true);
            }
            else
            {
                // special shape
                SpecialShape[] specialShapeComponent = targetShape.GetComponentsInChildren<SpecialShape>();
                if (specialShapeComponent.Length > 0)
                {
                    foreach (var specialShape in specialShapeComponent)
                    {
                        specialShape.SetInteractable(true);
                    }
                }
            }

            ShowHandAtCurrentShape();
            StartOutlineEffect(targetShape);
        }
    }

    public void OnShapeTouched(GameObject touchedShape)
    {
        if (!isTutorialActive) return;

        if (currentTutorialIndex < currentLevelController.tutorialShapes.Count)
        {
            GameObject targetShape = currentLevelController.tutorialShapes[currentTutorialIndex];

            if (touchedShape == targetShape ||
                touchedShape.transform.parent == targetShape.transform)
            {
                Shape shapeComponent = targetShape.GetComponent<Shape>();
                if (shapeComponent != null)
                {
                    shapeComponent.SetInteractable(false);
                }
                else
                {
                    SpecialShape specialShapeComponent = targetShape.GetComponentInChildren<SpecialShape>();
                    if (specialShapeComponent != null)
                    {
                        specialShapeComponent.SetInteractable(false);
                    }
                }

                if (currentTutorialIndex == currentLevelController.tutorialShapes.Count - 1)
                {
                    StopOutlineEffect(targetShape);
                    EndTutorial();
                    return;
                }

                currentTutorialIndex++;
                EnableNextShape();
            }
        }
    }

    private void EndTutorial()
    {
        isTutorialActive = false;
        if (handImage != null)
        {
            handImage.SetActive(false);
        }
        Debug.Log("✅ Tutorial Completed!");
    }

    private void SetAllShapesInteractable(bool state)
    {
        Shape[] allShapes = FindObjectsOfType<Shape>();
        foreach (Shape shape in allShapes)
        {
            if (shape != null)
            {
                shape.SetInteractable(state);
            }
        }

        SpecialShape[] allSpecialShapes = FindObjectsOfType<SpecialShape>();
        foreach (SpecialShape specialShape in allSpecialShapes)
        {
            if (specialShape != null)
            {
                specialShape.SetInteractable(state);
            }
        }
    }

    private void StartOutlineEffect(GameObject shape)
    {
        if (outlineCoroutine != null)
        {
            StopCoroutine(outlineCoroutine);
        }
        outlineCoroutine = StartCoroutine(ToggleOutlineEffect(shape));
    }

    //blink blink
    private IEnumerator ToggleOutlineEffect(GameObject shape)
    {
        Outline outline = shape.GetComponent<Outline>(); 

        if (outline != null) 
        {
            while (shape != null)
            {
                outline.enabled = !outline.enabled; 
                yield return new WaitForSeconds(1f);
            }
        }
        else 
        {
            Transform parentTransform = shape.transform;
            if (parentTransform == null) yield break;

            Outline[] childOutlines = parentTransform.GetComponentsInChildren<Outline>();

            while (shape != null)
            {
                foreach (Outline childOutline in childOutlines)
                {
                    if (childOutline != null)
                    {
                        childOutline.enabled = !childOutline.enabled;
                    }
                }
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void StopOutlineEffect(GameObject shape)
    {
        if (outlineCoroutine != null)
        {
            StopCoroutine(outlineCoroutine);
        }
        //if (shape != null)
        //{
        //    Outline outline = shape.GetComponent<Outline>();
        //    if (outline != null)
        //    {
        //        outline.enabled = false;
        //    }
        //}
    }
}
