using UnityEngine;

public class RaycastTriggerTarget : MonoBehaviour
{
    private void OnRaycastTriggerEnter(RaycastCollision collision)
    {
        
    }

    private void OnRaycastTriggerExit(RaycastCollision collision)
    {
        
    }
}

public struct RaycastCollision
{
    // contains info about a collision from a raycast with extra info
    public RaycastHit hit;
    public GameObject gameObject;
    
}