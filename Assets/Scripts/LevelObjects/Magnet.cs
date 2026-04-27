using UnityEngine;

namespace LevelObjects
{
    public class Magnet : Player.Hands
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public void PickupExternal()
        {
            // pick up the "observed" crate
            PickUpCrate(new Vector3(0, -2.5f, 0));
            observedCrate = null;
        }

        public override void ReleaseCrate()
        {
            observedCrate = heldCrate;
            base.ReleaseCrate();
        }
    }
}
