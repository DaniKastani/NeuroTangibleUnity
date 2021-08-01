using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VRfreePluginUnity {
    public class VRfreeTracker : MonoBehaviour {

   //  bool isGrabbed;
    // MovablesCollisionHandler handler;


       [Header("Settings")]
        public int trackerId;
        public Transform center; // when tracking is lost, rotations will apply around this center
        public bool getAbsoluteData = false;
        public bool rotationOnly = false;
        public GameObject hideWhenTrackingLost;

        [Header("Output")]
        public Vector3 trackerPosition;
        public Quaternion trackerRotation;
        public bool isTrackerPositionValid;
        public byte buttonPressed;

        [Header("Events")]
        public UnityEvent buttonPressedEvent;
        public UnityEvent buttonReleasedEvent;

       

        // Start is called before the first frame update
        void Start() {

         //handler = gameObject.GetComponent<MovablesCollisionHandler>();

            if (center != null) {
                center.parent = transform;
                center.localRotation = Quaternion.identity;
            }
            isTrackerPositionValid = true;
            UpdateTracking(Vector3.zero, Quaternion.identity, isTrackerPositionValidNew: false);
        }

        public void setCenter(Transform center) {
            transform.parent = null;
            center.parent = transform;

            this.center = center;

            center.parent = null;
            transform.parent = center;
        }

        // Update is called once per frame
        void Update() {

       //  isGrabbed = handler.isGrabbed;
          
     //    if (isGrabbed == false)
      //     {
                VRfree.Vector3 outPos;
                VRfree.Quaternion outQuat;
                byte newButtonPressed;
                bool isTrackerPositionValidNew;
                if (getAbsoluteData)
                {
                    VRfree.VRfreeAPI.GetTrackerDataAbsolute(out outPos, out outQuat, out isTrackerPositionValidNew, out newButtonPressed, trackerId, false);
                }
                else
                {
                    VRfree.VRfreeAPI.GetTrackerData(out outPos, out outQuat, out isTrackerPositionValidNew, out newButtonPressed, trackerId);
                }

                if (buttonPressed == 0 && newButtonPressed != 0)
                {
                    buttonPressedEvent.Invoke();
                }
                else if (buttonPressed != 0 && newButtonPressed == 0)
                {
                    buttonReleasedEvent.Invoke();
                }
                buttonPressed = newButtonPressed;

                if (outQuat.x == 0 && outQuat.y == 0 && outQuat.z == 0 && outQuat.w == 0) outQuat = VRfree.Quaternion.identity;
                trackerPosition = HandData.Vector3FromVRfree(outPos);
                trackerRotation = HandData.QuaternionFromVRfree(outQuat);

                UpdateTracking(trackerPosition, trackerRotation, isTrackerPositionValidNew);
           // }
        }

        private void UpdateTracking(Vector3 trackerPosition, Quaternion trackerRotation, bool isTrackerPositionValidNew) {

          
                if (isTrackerPositionValidNew && !rotationOnly)
                {
                    if (!isTrackerPositionValid)
                    {
                        transform.parent = null;
                        if (center != null) center.parent = transform;
                        if (hideWhenTrackingLost != null)
                            hideWhenTrackingLost.SetActive(true);
                    }
                    transform.position = trackerPosition;
                    transform.rotation = trackerRotation;
                }
                else
                {
                    // isTrackerPositionValidNew is false
                    if (center != null)
                    {
                        // rotate around center
                        if (isTrackerPositionValid)
                        {
                            // just became invalid
                            center.parent = null;
                            transform.parent = center;
                            if (hideWhenTrackingLost != null)
                                hideWhenTrackingLost.SetActive(false);
                        }
                        center.rotation = trackerRotation;
                    }
                    else
                    {
                        transform.rotation = trackerRotation;
                    }
                }
                isTrackerPositionValid = isTrackerPositionValidNew;
        }
        



    }
}