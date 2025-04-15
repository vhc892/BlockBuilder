using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
    public bool pushToRight = true;
    public float pushSpeed = 2f;
    private BoxCollider boxCollider;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TargetObject"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = pushToRight ? Vector3.right : Vector3.left;

                rb.velocity = new Vector3(pushDirection.x * pushSpeed, rb.velocity.y, rb.velocity.z);
            }
        }
    }
}
