using UnityEngine;
using GameConstants.Enumerations;
using System.Collections.Generic;
using System.Linq;

namespace LevelObjects
{
    public class ColliderFactory : MonoBehaviour
    {
        // used to generate collision once we switch to 2D & are not looking straight down
        
        [Header("Factory Settings")]
        [SerializeField] private GameObject colliderPrefab;
        [SerializeField] private float colliderDepth = 5.0f;
        [SerializeField] private GameObject playerCharacter;
        
        [Header("Debug Information")] [SerializeField]
        private Axes mainAxis = Axes.X;
        [SerializeField] private Axes secondaryAxis = Axes.Z;
        [SerializeField] private List<BoxCollider> generatedColliders = new();
        [SerializeField] private List<Collider> standardColliders = new();

        private const string ROOT_NAME = "Geometry";
        
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
                OpenColliderPool();
            }
            else if (newDimension == Dimensions.THIRD)
            {
                DisableAllGeneratedColliders();
            }
        }

        private void OpenColliderPool()
        {
            // gets colliders from the pool and places them in the needed positions
            if (PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D.Count > 0)
            {
                // update the standard colliders list to have all the colliders of the detected geometry
                
                // check we have detected geometry
                print("Opening the collider pool");
                // go through the generated collision & calculate the new bounds for it
                
                for (int i = 0; i < PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D.Count; i++)
                {
                    // get the generated collider of the same index & adjust bounds
                    BoxCollider currentCol = generatedColliders[i];
                    
                    // check the axis we're looking down
                    if (PerspectiveSwitcher.CurrentObservedAxis == Axes.X)
                    {
                        // set base position using the player's x axis value
                        currentCol.transform.position = new Vector3(
                            playerCharacter.transform.position.x,
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].center.y,
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].center.z
                            );

                        currentCol.size = new Vector3(
                            colliderDepth,
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].size.y,
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].size.z
                        );
                    }
                    else if (PerspectiveSwitcher.CurrentObservedAxis == Axes.Z)
                    {
                        // set base position using the player's z axis value
                        currentCol.transform.position = new Vector3(
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].center.x,
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].center.y,
                            playerCharacter.transform.position.z
                            );
                        
                        // set bounds of the collider to use collision depth on z axis
                        currentCol.size = new Vector3(
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].size.x,
                            PerspectiveSwitcher.CurrentVisibleCollisionGeometryIn2D[i].size.y,
                            colliderDepth
                            );
                    }
                    
                    // we've set the bounds of the collider as needed yay
                    // just enable it now
                    currentCol.enabled = true;
                }
            }
        }

        private void CreateColliderPool()
        {
            print("Populating collider pool");
            // create a pool of colliders on the scene startup which we can reuse for the whole level
            // get the scene's levelgeometry root & get all box collider children
            GameObject root = GameObject.Find(ROOT_NAME);
            
            standardColliders.AddRange(root.GetComponentsInChildren<BoxCollider>());
            
            // instantiate the needed amount of colliders
            for (int i = 0; i < standardColliders.Count; i++)
            {
                generatedColliders.Add(Instantiate(colliderPrefab).GetComponent<BoxCollider>());
                generatedColliders[i].enabled = false; // default state is disabled
                generatedColliders[i].transform.SetParent(transform);
            }
        }

        private void DisableAllGeneratedColliders()
        {
            foreach (var collider in generatedColliders)
            {
                collider.enabled = false;
            }
        }
    }
}
