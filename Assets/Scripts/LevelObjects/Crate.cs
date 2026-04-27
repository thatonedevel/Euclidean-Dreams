using System.Threading;
using UnityEditor;
using UnityEngine;

namespace LevelObjects
{
    public class Crate : MonoBehaviour
    {
        public bool IsHeld { get; private set; } = false;
        public bool CanHold { get; private set; } = false;

        [Header("Object References")] 
        [SerializeField] private Rigidbody crateRigidbody;

        private GameObject holder;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        public void PickUp(GameObject holder)
        {
            this.holder = holder;
            transform.SetParent(holder.transform);
            transform.localPosition = Vector3.zero;
            crateRigidbody.useGravity = false;
        }

        public void PickUp(GameObject holder, Vector3 localPosition)
        {
            this.holder = holder;
            transform.SetParent(holder.transform);
            transform.localPosition = localPosition;
            crateRigidbody.useGravity = false;
        }

        public void Release()
        {
            this.holder = null;
            transform.SetParent(null);
            crateRigidbody.useGravity = true;
        }

        private void OnValidate()
        {
            if (crateRigidbody == null)
                return;
            if (transform.parent == null)
            {
                PickUp(transform.parent.gameObject);
            }
        }
    }
}
