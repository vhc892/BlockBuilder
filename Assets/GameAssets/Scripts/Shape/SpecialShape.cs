using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class SpecialShape : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Rigidbody parentRb;
    private Renderer objRenderer;
    private Color originalColor;
    private bool hasTouched = false;
    private bool canTouchAgain = false;
    private static bool gameLost = false;
    private bool isInteractable = true;
    private Outline outline;

    private Transform parentTransform;

    void Awake()
    {
        parentRb = GetComponentInParent<Rigidbody>();
        outline = GetComponent<Outline>();
        objRenderer = GetComponent<Renderer>();
        parentTransform = transform.parent;
    }

    private void Start()
    {
        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;
            Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.55f);
            objRenderer.material.color = transparentColor;
        }
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void SetInteractable(bool state)
    {
        isInteractable = state;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        TurnOnAllChildOutLine();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable || parentRb == null) return;
        TurnOffAllChildOutLine();
        if (!hasTouched)
        {
            ChangeAllChildLayers("Default");
            hasTouched = true;
            parentRb.isKinematic = false;
            StartCoroutine(EnableSecondTouch());
            ChangeAllChildColors(originalColor);
        }
        else if (canTouchAgain)
        {
            ChangeAllChildLayers("Shape");
            parentRb.constraints = RigidbodyConstraints.None;
            parentRb.AddForce(new Vector3(0, 0, 150f));

            Vector3 randomTorque = new Vector3(
                Random.Range(-200f, 200f),
                Random.Range(-200f, 200f),
                Random.Range(-200f, 200f)
            );

            parentRb.AddTorque(randomTorque);
            StartCoroutine(DestroyAfterDelay());
            if (CompareTag("TargetObject"))
            {
                Debug.Log("❌ Player touched TargetObject - Lose!");
                GameEvents.LevelLose();
                gameLost = true;
            }
        }
        TutorialManager.Instance.OnShapeTouched(gameObject);
        if (!gameLost)
        {
            GameEvents.ShapeTouch();
        }
    }

    private void ChangeAllChildColors(Color newColor)
    {
        if (parentTransform == null) return;

        Renderer[] childRenderers = parentTransform.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in childRenderers)
        {
            renderer.material.color = newColor;
        }
    }

    private void ChangeAllChildLayers(string layerName)
    {
        if (parentTransform == null) return;

        int layerID = LayerMask.NameToLayer(layerName);

        foreach (Transform child in parentTransform.GetComponentsInChildren<Transform>())
        {
            child.gameObject.layer = layerID;
        }
    }

    private void TurnOnAllChildOutLine()
    {
        if (!isInteractable) return;
        if (parentTransform == null) return;

        Outline[] childOutlines = parentTransform.GetComponentsInChildren<Outline>();
        foreach (Outline childOutline in childOutlines)
        {
            childOutline.enabled = true;
        }
    }

    private void TurnOffAllChildOutLine()
    {
        if (parentTransform == null) return;

        Outline[] childOutlines = parentTransform.GetComponentsInChildren<Outline>();
        foreach (Outline childOutline in childOutlines)
        {
            childOutline.enabled = false;
        }
    }

    private IEnumerator EnableSecondTouch()
    {
        yield return new WaitForSeconds(1f);
        canTouchAgain = true;
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        parentRb.velocity = Vector3.zero;
        parentRb.angularVelocity = Vector3.zero;
        parentRb.isKinematic = true;
    }
}
