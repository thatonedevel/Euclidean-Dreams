using UnityEngine;

namespace LevelObjects
{
    public class Magnet : MonoBehaviour
    {
        [SerializeField] private Crate observedCrate;
        private bool isCrateHeld = false;

        public void ToggleMagnet()
        {
            if (!isCrateHeld)
            {
                observedCrate.PickUp(gameObject, new Vector3(0, -2.4f, 0));
                isCrateHeld = true;
            }
            else
            {
                observedCrate.Release();
                isCrateHeld = false;
            }
        }
    }
}
