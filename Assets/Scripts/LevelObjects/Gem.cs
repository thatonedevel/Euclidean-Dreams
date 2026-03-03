using System;
using Data;
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

        private void Awake()
        {
            LevelData.OnLevelInitComplete += LevelInitHandler;  
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(Constants.TAG_PLAYER))
            {
                OnGemCollected?.Invoke(gemNumber);
                Destroy(gameObject);
            }
        }

        private void LevelInitHandler(bool[] gemStatus)
        {
            if ((int)gemNumber < gemStatus.Length)
            {
                if (gemStatus[(int)gemNumber])
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
