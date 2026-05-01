using UnityEngine;
using GameConstants;
using GameConstants.Enumerations;
using Managers;

public class DeathBox : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAG_PLAYER))
        {
            GameController.Singleton.GameOver();
        }
    }
}
