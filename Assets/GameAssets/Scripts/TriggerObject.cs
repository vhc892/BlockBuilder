using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TargetObject"))
        {
            Debug.Log("Triggerenter");
            GameEvents.LevelLose();
        }
    }
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("TargetObject"))
    //    {
    //        Debug.Log("Trigger");
    //        GameEvents.LevelLose();
    //    }
    //}
}
