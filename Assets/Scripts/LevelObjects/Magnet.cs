using UnityEngine;

namespace LevelObjects
{
    public class Magnet : Player.Hands
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        public void ToggleMagnet()
        {
            if (isPickedUp)
            {
                PickUpCrate(new Vector3(0, -2.5f, 0));
            }
            else
            {
                ReleaseCrate();
            }
        }
    }
}
