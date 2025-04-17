using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class Shape : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private Rigidbody rb;
    private Renderer objRenderer;
    private Color originalColor;
    public bool hasTouched = false; // hasTouched = display
    private bool canTouchAgain = false;
    private bool isInteractable = true;
    private static bool gameLost = false;
    private Outline outline;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        objRenderer = GetComponent<Renderer>();
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        gameLost = false;
        if (!hasTouched)
        {
            SetUpShape();
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Default");
            canTouchAgain = true;
        }
        if(outline != null)
        {
            outline.enabled = false;
        }
    }

    private void SetUpShape()
    {
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        if (objRenderer != null)
        {
            originalColor = objRenderer.material.color;
            Color transparentColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            objRenderer.material.color = transparentColor;
        }
    }

    public void SetInteractable(bool state)
    {
        isInteractable = state;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isInteractable) return;
        if (outline != null)
        {
            outline.enabled = true;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isInteractable) return;
        if (outline != null)
        {
            outline.enabled = false;
        }
        if (!hasTouched)
        {
            Debug.Log("Touch detected, object falling...");
            gameObject.layer = LayerMask.NameToLayer("Default");
            hasTouched = true;

            if (rb != null)
            {
                EnablePhysics();
                StartCoroutine(EnableSecondTouch());
            }

            if (objRenderer != null)
            {
                Color opaqueColor = new Color(originalColor.r, originalColor.g, originalColor.b, 1f);
                objRenderer.material.color = opaqueColor;
            }
        }
        else if (canTouchAgain)
        {
            Debug.Log("Object touched again, applying force to Z axis...");
            gameObject.layer = LayerMask.NameToLayer("Shape");
            rb.constraints = RigidbodyConstraints.None;
            rb.AddForce(new Vector3(0, 0, 150f));

            Vector3 randomTorque = new Vector3(
                Random.Range(-200f, 200f),
                Random.Range(-200f, 200f),
                Random.Range(-200f, 200f)
            );
            rb.AddTorque(randomTorque);

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
    private void EnablePhysics()
    {
        rb.isKinematic = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }


    private IEnumerator EnableSecondTouch()
    {
        yield return new WaitForSeconds(1f);
        canTouchAgain = true;
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }
}
