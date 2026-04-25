using UnityEngine;

[ExecuteInEditMode]
public class Rail : MonoBehaviour
{
    // Represents a rail that can have an attached object move across it
    [Header("Rail Settings")] [SerializeField]
    private float railLength = 1; // implicitly determine start/end points

    public Vector3 RailStart { get; private set; }
    public Vector3 RailEnd { get; private set; }

    [Header("Rail Mesh")]
    [SerializeField] private GameObject railMesh;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        RailStart = transform.position;
        RailEnd = transform.position + (transform.forward * railLength);
        railMesh = transform.GetChild(0).gameObject;
    }

    Vector3 GetPointOnRail(float t)
    {
        // takes in a value t and returns the position of the object on the rail
        
        return Vector3.Lerp(RailStart, RailEnd, t);
    }

    private void OnValidate()
    {
        // use this to adjust the length of the mesh
        railMesh.transform.localScale = new Vector3(1, 1, railLength);
        // length also = distance from centre
        // dist = 1/2 of length
        
        railMesh.transform.localPosition = new Vector3(0, 0, (railLength/2) - 0.5f);
    }
}
