using UnityEngine;
using UnityEngine.InputSystem;
using GameConstants;
using GameConstants.Enumerations;
using LevelObjects;
using Managers;

namespace Player
{
    public class Hands : MonoBehaviour
    {
        private InputAction pickupAction;
        protected Crate heldCrate;
        protected bool canPickUp = false;
        protected bool isPickedUp = false;
        [SerializeField] protected Crate observedCrate;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            pickupAction = InputSystem.actions.FindAction(Constants.ACTION_INTERACT);
        }

        // Update is called once per frame
        void Update()
        {
            // check for pickup
            if (pickupAction.WasPressedThisFrame() && !isPickedUp && canPickUp)
            {
                PickUpCrate(transform.position + (Vector3.up * 2.5f));
            }
            else if (pickupAction.WasPressedThisFrame() && isPickedUp)
            {
                observedCrate = heldCrate;
                ReleaseCrate();
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(Constants.TAG_CRATES))
            {
                Debug.Log("crate  found");
                canPickUp = true;
                observedCrate = other.GetComponent<Crate>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.TAG_CRATES))
            {
                Debug.Log("crate is gone");
                canPickUp = false;
                observedCrate = null;
            }
        }

        protected void ReleaseCrate()
        {
            heldCrate.Release();
            observedCrate = heldCrate;
            heldCrate =  null;
            isPickedUp = false;
        }
        
        protected void PickUpCrate(Vector3 localPickupPos)
        {
            heldCrate = observedCrate;
            heldCrate.PickUp(gameObject, localPickupPos);
            isPickedUp = true;
            observedCrate = null;
        }
    }
}
