using UnityEngine;

namespace LevelObjects
{
    public class Crate : MonoBehaviour
    {
        public bool IsHeld { get; private set; } = false;

        [Header("Object References")] 
        [SerializeField] private Rigidbody crateRigidbody;

        private GameObject holder;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            if (transform.parent != null && holder == null)
            {
                PickUp(transform.parent.gameObject);
            }
        }
        
        
        public void PickUp(GameObject holder)
        {
            this.holder = holder;
            transform.SetParent(holder.transform);
            transform.localPosition = Vector3.zero;
            crateRigidbody.useGravity = false;
            crateRigidbody.isKinematic = true;
        }

        public void PickUp(GameObject holder, Vector3 localPosition)
        {
            this.holder = holder;
            transform.SetParent(holder.transform);
            transform.localPosition = localPosition;
            crateRigidbody.useGravity = false;
            crateRigidbody.isKinematic = true;
        }

        public void Release()
        {
            this.holder = null;
            transform.SetParent(null);
            crateRigidbody.useGravity = true;
            crateRigidbody.isKinematic = false;
        }

        private void OnValidate()
        {
            if (crateRigidbody == null)
                return;
            if (transform.parent != null)
            {
                PickUp(transform.parent.gameObject);
            }
        }
    }
}
