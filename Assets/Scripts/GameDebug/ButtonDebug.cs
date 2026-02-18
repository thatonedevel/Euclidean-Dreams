using UnityEngine;

namespace GameDebug
{
    public class ButtonDebug : MonoBehaviour
    {
        // debug methods to make sure our buttons work

        public void ButtonPressedDebug()
        {
            print("button was pressed");
        }
        
        public void PressureButtonPressedDebug()
        {
            print("pressure button is pressed");
        }

        public void PressureButtonReleasedDebug()
        {
            print("pressure button was released");
        }
    }
}
