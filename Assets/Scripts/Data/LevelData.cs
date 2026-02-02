using GameConstants.Enumerations;
using UnityEngine;

public class LevelData : MonoBehaviour
{
    // class used to store data about the current level
    // stores collectible status & level clear time

    // all the data we want to read
    public float levelClearTime { get; private set; } = -1;
    public bool[] gemCollectionStatus = new bool[3];
    [SerializeField] private string stageName;

    // internal data
    private float levelStartTime;

    public string GetStageName() => stageName;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // subscribe to gem collected event
        Gem.OnGemCollected += GemCollectedHandler;
        levelStartTime = Time.time;
    }

    private void OnDestroy()
    {
        // unsubscribe from events
        Gem.OnGemCollected -= GemCollectedHandler;
    }

    public void ResetLevelTime()
    {
        // resets the level start time to current time

    }

    public void ResetLevelData()
    {

    }

    public void GemCollectedHandler(GemOrders num)
    {
        // gemorders is an enum so we can cast directly to an int to get the index
        gemCollectionStatus[(int)num] = true;
    }

    public void StopLevelTimer()
    {
        // call this when we want to get the stage's clear time
        levelClearTime = Mathf.Round(Time.time - levelStartTime);
    }
}
