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
        private Crate heldCrate;
        private bool canPickUp = false;
        private bool isPickedUp = false;
        private Crate tempCrate;
        
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
                heldCrate = tempCrate;
                tempCrate = null;
                heldCrate.PickUp(gameObject, (Vector3.forward * 2) + new Vector3(0, 0.5f, 0)); // pick up crate & put it in front of character
                isPickedUp = true;
            }
            else if (pickupAction.WasPressedThisFrame() && isPickedUp)
            {
                heldCrate.Release();
                heldCrate = null;
                isPickedUp = false;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(Constants.TAG_CRATES))
            {
                Debug.Log("crate  found");
                canPickUp = true;
                tempCrate = other.GetComponent<Crate>();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(Constants.TAG_CRATES))
            {
                Debug.Log("crate is gone");
                canPickUp = false;
                tempCrate = null;
            }
        }
    }
}
