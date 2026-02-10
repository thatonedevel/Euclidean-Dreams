using UnityEngine;
using GameConstants.Enumerations;
using System.Collections.Generic;

namespace LevelObjects
{
    public class ColliderFactory : MonoBehaviour
    {
        // used to generate collision once we switch to 2D & are not looking straight down
        
        [Header("References")]
        [SerializeField] private GameObject colliderPrefab;
        
        [Header("Debug Information")] [SerializeField]
        private Axes mainAxis = Axes.X;
        [SerializeField] private Axes secondaryAxis = Axes.Z;
        [SerializeField] private List<BoxCollider> generatedColliders = new();
        [SerializeField] private List<Collider> standardColliders = new();

        private const string ROOT_NAME = "LevelGeometry";
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            // subscribe to the dimension switch event
            PerspectiveSwitcher.OnDimensionsSwitched += DimensionSwitchHandler;
            CreateColliderPool();
        }

        private void OnDestroy()
        {
            PerspectiveSwitcher.OnDimensionsSwitched -= DimensionSwitchHandler;
        }

        private void DimensionSwitchHandler(Dimensions newDimension)
        {
            if (newDimension == Dimensions.SECOND && PerspectiveSwitcher.CurrentObservedAxis != Axes.Y)
            {
                // we generate the side on collision. note - objects must use a primitive collider shape for
                // this to function
            }
        }

        private void GenerateCollision()
        {
            
        }

        private void CreateColliderPool()
        {
            // create a pool of colliders on the scene startup which we can reuse for the whole level
            // get the scene's levelgeometry root & get all box collider children
            GameObject root = GameObject.Find(ROOT_NAME);
            
            standardColliders.AddRange(root.GetComponentsInChildren<BoxCollider>());
            
            // instantiate the needed amount of colliders
            for (int i = 0; i < standardColliders.Count; i++)
            {
                generatedColliders.Add(Instantiate(colliderPrefab).GetComponent<BoxCollider>());
                generatedColliders[i].enabled = false; // default state is disabled
            }
        }
    }
}
