using GameConstants.Enumerations;
using LevelObjects;
using UnityEngine;

public class RailAdjuster : ColliderAdjuster
{
    private RailData restorationData;
    [SerializeField] private Rail ownedRail;

    protected override void Start()
    {
        base.Start();
        // set up the restoration data
        if (ownedRail == null)
            ownedRail = GetComponent<Rail>();
        
        restorationData = new RailData();
        restorationData.railStart = ownedRail.RailStart;
        restorationData.railEnd = ownedRail.RailEnd;
        restorationData.railLength = ownedRail.GetRailLength();
    }
    
    // use this to adjust the positioning of the rails as well as their collision
    protected override void DimensionSwitchHandler(Dimensions newDim)
    {
        base.DimensionSwitchHandler(newDim);
        // don't move the rail itself, but instead store and restore the rail's positions

        if (newDim == Dimensions.SECOND)
        {
            // restore the rail settings
            ownedRail.RemoveConnectedRail(restorationData);
        }
    }
}
public struct RailData
{
    public Vector3 railStart;
    public Vector3 railEnd;
    public float railLength;
}