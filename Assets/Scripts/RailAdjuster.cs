using GameConstants.Enumerations;
using LevelObjects;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Pipeline.Utilities;
using UnityEngine;

public class RailAdjuster : ColliderAdjuster
{
    private RailData restorationData;
    [SerializeField] private Rail ownedRail;
    [SerializeField] private float connectionThreshold;
    
    private const string TAG_RAIL = "Rail";

    protected override void Start()
    {
        base.Start();
        // set up the restoration data
        if (ownedRail == null)
            ownedRail = GetComponent<Rail>();
        
        restorationData = new RailData
        {
            railStart = ownedRail.RailStart,
            railEnd = ownedRail.RailEnd,
            railLength = ownedRail.GetRailLength()
        };
    }
    
    // use this to adjust the positioning of the rails as well as their collision
    protected override void DimensionSwitchHandler(Dimensions newDim)
    {
        base.DimensionSwitchHandler(newDim);
        // don't move the rail itself, but instead store and restore the rail's positions

        if (newDim == Dimensions.THIRD)
        {
            // restore the rail settings
            ownedRail.RemoveConnectedRail(restorationData);
        }
        else
        {
            // connect the rails
            FindConnectingRail();
        }
    }

    private bool FindConnectingRail()
    {
        HashSet<Rail> checkedRails = new HashSet<Rail>();
        // finds a rail object that our own rail can attach to
        // returns true/false based on success
        bool foundRail = false;
        var axisToCheck = PerspectiveSwitcher.CurrentObservedAxis;

        GameObject[] tmp = GameObject.FindGameObjectsWithTag(TAG_RAIL);

        Rail[] railsInScene = new Rail[tmp.Length];
        
        for (int i = 0; i <  railsInScene.Length; i++) railsInScene[i] = tmp[i].GetComponent<Rail>();
        
        int[] subIndices = Enumerable.Range(0, railsInScene.Length).ToArray(); // as explained by alexn (2021)

        for (int i = 0; i < railsInScene.Length; i++)
        {
            // check this rail against the other rails in the scene
            for (int j = 0; j < subIndices.Length; j++)
            {
                if (i == j) continue;
                
                // check if a connection can be made between these rails. first connection breaks the loop
                var state = CanRailsConnect(railsInScene[i], railsInScene[j]);
            }
        }
        
        return foundRail;
    }

    private RailConnectionState CanRailsConnect(Rail railA, Rail railB)
    {
        // check if the start / end points
        RailConnectionState conState = RailConnectionState.NO_CONNECTION;
        
        // cancel the axis we need to ignore here
        Vector3 aStart = CancelAxis(railA.RailStart, PerspectiveSwitcher.CurrentObservedAxis);
        Vector3 bStart = CancelAxis(railB.RailStart, PerspectiveSwitcher.CurrentObservedAxis);
        
        Vector3 aEnd = CancelAxis(railA.RailEnd, PerspectiveSwitcher.CurrentObservedAxis);
        Vector3 bEnd = CancelAxis(railB.RailEnd, PerspectiveSwitcher.CurrentObservedAxis);
        
        // check if back of a matches front of b
        if (Vector3.Distance(aEnd, bStart) <= connectionThreshold)
        {
            // connect the rails, a first
            conState = RailConnectionState.A_FIRST;
        }
        else if (Vector3.Distance(aStart, bEnd) <= connectionThreshold)
        {
            conState = RailConnectionState.B_FIRST;
        }
        return  conState;
    }

    private Vector3 CancelAxis(Vector3 startV, Axes axisToIgnore)
    {
        Vector3 mod = startV;
        
        switch (axisToIgnore)
        {
            case  Axes.X:
                mod.x = 0;
                break;
            case Axes.Y:
                mod.y = 0;
                break;
            case  Axes.Z:
                mod.z = 0;
                break;
        }

        return mod;
    }

    private enum RailConnectionState
    {
        A_FIRST,
        B_FIRST,
        NO_CONNECTION
    }

    private struct RailConnection
    {
        public Rail firstRail;
        public Rail secondRail;
        public RailConnectionState connectionType;
    }
}


public struct RailData
{
    public Vector3 railStart;
    public Vector3 railEnd;
    public float railLength;
}