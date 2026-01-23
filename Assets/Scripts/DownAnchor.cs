using UnityEngine;

public class DownAnchor : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        // make sure that we have this object always face upwards in world space
        // ie x / z acis rotation = 0
        transform.localEulerAngles.Set(-transform.parent.localEulerAngles.x, transform.localEulerAngles.y, transform.eulerAngles.z);
    }
}
