using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "VersionNumSO", menuName = "Scriptable Objects/VersionNumSO")]
    public class VersionNumSO : ScriptableObject
    {
        public string versionFormatted;
    
        private void Awake()
        {
            // set the version variable
            versionFormatted = "v" + Application.version;
        }
    }
}

