using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class RaycastTriggerTarget : MonoBehaviour
{
    private List<RaycastCollision> activeCollisions = new();
    
    // public events for collisions
    public static event Action<RaycastCollision> RaycastTriggered;
    public static event Action<RaycastCollision> RaycastTriggerExit;

    private void OnRaycastTriggerEnter(RaycastCollision collision)
    {
        // check if we have this in our active collisions
        if (!activeCollisions.Contains(collision) &&
            !HasGameObject(collision.SourceGamemeObject))
        {
            // new collision occurred
            RaycastTriggered?.Invoke(collision);
        }
    }

    private void OnRaycastTriggerExit(RaycastCollision collision)
    {
        activeCollisions.Remove(collision);
        RaycastTriggerExit?.Invoke(collision);
    }

    private void FixedUpdate()
    {
        // update the list of collisions
        for (int i = activeCollisions.Count - 1; i >= 0; i--)
        {
            // since this is for top-down collisions, we just check if the x/z still falls in the collider bounds
            Bounds objectFakeBounds = activeCollisions[i].SourceGamemeObject.GetComponent<Collider>().bounds;
            objectFakeBounds.center = new Vector3(objectFakeBounds.center.x, 
                activeCollisions[i].Hit.collider.bounds.center.y, objectFakeBounds.center.z);
            
            // check for overlap
            if (!objectFakeBounds.Intersects(activeCollisions[i].Hit.collider.bounds))
            {
                // this counts as a collision exit
                var currentCollision =  activeCollisions[i];
                OnRaycastTriggerExit(currentCollision);
            }
        }
    }

    private bool HasGameObject(GameObject comp)
    {
        bool result = false;

        foreach (var collision in activeCollisions)
        {
            if (collision.SourceGamemeObject == comp)
            {
                result = true;
                break;
            }
        }
        
        return result;
    }

}

public struct RaycastCollision : IEquatable<RaycastCollision>
{
    // contains info about a collision from a raycast with extra info
    public RaycastHit Hit { get; private set; }
    public GameObject SourceGamemeObject { get; private set; }
    public Guid CollisionGuid { get; private set; }

    public RaycastCollision(RaycastHit hit, GameObject source)
    {
        Hit = hit;
        SourceGamemeObject = source;
        CollisionGuid = Guid.NewGuid();
    }
    
    public bool Equals(RaycastCollision other) => other.CollisionGuid == CollisionGuid;
}