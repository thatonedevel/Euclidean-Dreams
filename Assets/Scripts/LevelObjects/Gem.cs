using System;
using UnityEngine;
using GameConstants;
using GameConstants.Enumerations;

namespace LevelObjects
{
    public class Gem : MonoBehaviour
    {
        // behaviour for the gem collectible
        public GemOrders gemNumber;
    
        public static event Action<GemOrders> OnGemCollected;
    
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.TAG_PLAYER))
            {
                OnGemCollected?.Invoke(gemNumber);
                Destroy(gameObject);
            }
        }
    }
}
