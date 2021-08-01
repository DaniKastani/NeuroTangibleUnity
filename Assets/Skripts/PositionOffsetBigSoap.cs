using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace VRfreePluginUnity
{
    public class PositionOffsetBigSoap : MonoBehaviour
    {
        // Start is called before the first frame updates

        public GameObject tracekdObject;
        public Vector3 offset;
        private VRfreeTracker TrackerScript;

        void Start()
        {
            TrackerScript = tracekdObject.GetComponent<VRfreeTracker>();
        }

        // Update is called once per frame
        void Update()
        {
            this.gameObject.transform.position = TrackerScript.trackerPosition + offset;
            this.gameObject.transform.rotation = tracekdObject.transform.rotation;
        }
    }
}