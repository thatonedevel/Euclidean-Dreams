using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class RaycastTriggerTarget : MonoBehaviour
{
    private List<RaycastCollision> activeCollisions = new();
    
    private void OnRaycastTriggerEnter(RaycastCollision collision)
    {
        // check if we have this in our active collisions
        if (!activeCollisions.Contains(collision))
        {
            // new collision occurred
        }
    }

    private void OnRaycastTriggerExit(RaycastCollision collision)
    {
        
    }
}

public struct RaycastCollision : IEquatable<RaycastCollision>
{
    // contains info about a collision from a raycast with extra info
    public RaycastHit hit { get; private set; }
    public GameObject gameObject { get; private set; }
    public Guid collisionGuid { get; private set; }

    public RaycastCollision(RaycastHit hit, GameObject source)
    {
        this.hit = hit;
        gameObject = source;
        collisionGuid = Guid.NewGuid();
    }
    
    public bool Equals(RaycastCollision other) => other.collisionGuid == collisionGuid;
}