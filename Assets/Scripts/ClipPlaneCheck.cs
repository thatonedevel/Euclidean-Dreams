using UnityEngine;

public class ClipPlaneCheck : MonoBehaviour
{
    [SerializeField] private Camera levelCam;
    [SerializeField] private MeshRenderer parentRenderer;
    [SerializeField] private MeshRenderer crateRenderer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var res = GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(levelCam), parentRenderer.bounds);
    }
}
