/*
 * The VRfreeGlove class is the base for the use of the VRfree glove in Unity. It handles all calls to the dll (Windows) 
 * or aar (Android), fetches the hand data, and can apply it to a hand model if the corresponding transforms are
 * set in handTransforms.
 */
using UnityEngine;
using System.Collections.Generic;
using VRfreeAPI = VRfree.VRfreeAPI;
using CalibrationAPI = VRfree.CalibrationAPI;

namespace VRfreePluginUnity {
    [ScriptOrder(-100)]
    public class VRfreeGlove : MonoBehaviour {
        //editor input
        /* use this flag to tell this instance which data to fetch */
        public bool isRightHand;
        /* when true, disconnects the VRfree device and stops the corresponding thread when the app is paused
         * as opposed to when it is destroyed. Recommended on Android, since apps are not destroyed when exiting with
         * the home button, and continuing to be connected can unnecessarily drain the battery of the phone	 */
        public bool disconnectOnPause = false;

        public bool allowOnlyHeadModuleUSB = true;

        /* The native bone orientation from the VRfree dll assumes all bones are pointed in z direction, 
         * with the y axis pointing towards the outside of the hand. If convertForSteamVrGlove is enabled, 
         * bones will be rotated to match the orientations given by the SteamVR skeleton by Valve. */
        public bool convertForSteamVrGlove = false;

        // set these to apply and visualize the hand data received from the VRfree glove
        public HandTransforms handTransforms;
        public Transform trackerTransform;
       public Transform externalTracker;
        public Vector3 offset1;
        private Vector3 currentEulerAngles;
        private Quaternion quaternionEullerAngles;

        private Vector3 newWristPosition;


        public HashSet<IPoseReceiver> poseReceivers = new HashSet<IPoseReceiver>();

        //editor output
        public string deviceStatus;
        public string deviceError;
        public uint cameraTimestamp;
        public int timeSinceLastLeftHandData;
        public int timeSinceLastRightHandData;
        public bool isWristPositionValid;
        public static bool isPaused = true;

        //internally used variables to get data from the driver
        public HandData handData;
        public HandData displayHandData; // hand data to be displayed, differs from handData e.g. during calibration
        public VRfree.HandAngles handAngles;

        //internally used variables to calibrate the hands
        public bool showingCalibrationPose = false;

        //variables to save GC alloc
        private Vector3 tempUnityVector;
        private VRfree.Vector3 tempVRfreeVector;
        private Quaternion tempUnityQuaternion;
        private VRfree.Quaternion tempVRfreeQuaternion;



        //constructor
        protected VRfreeGlove() {
            isPaused = true;
        }

        virtual protected void startVRfree() {
            VRfreeAPI.Init();
            if(allowOnlyHeadModuleUSB) {
                VRfree.CalibrationAPI.SetAllowedDevices(new List<VRfree.VRfreePID>() { VRfree.VRfreePID.Head });
            }
            VRfreeAPI.Start();
        }

        virtual public void calibrateTargetHandData(HandData target) {
            if(convertForSteamVrGlove)
                target.convertFromOpenVrQuaternions(isRightHand);
            CalibrationAPI.CalibratePose(isRightHand, HandData.HandDataToVRfree(target));
        }

        virtual public void calibrateAdditionalTargetHandData(HandData target) {
            if(convertForSteamVrGlove)
                target.convertFromOpenVrQuaternions(isRightHand);
            CalibrationAPI.CalibratePose(isRightHand, HandData.HandDataToVRfree(target), false);
        }

        virtual protected void getHandData() {
            HandData.HandDataFromVRfree(VRfreeAPI.GetHandData(isRightHand), ref handData);

            if(convertForSteamVrGlove) {
                handData.convertToOpenVrQuaternions(isRightHand);
            }
        }

        public Vector3 GetWristTrackerPosition() {
            return HandData.Vector3FromVRfree(VRfreeAPI.GetWristTrackerPosition(isRightHand));
           
        }

        virtual protected void getHandAngles() {
            handAngles = VRfreeAPI.GetHandAngles(isRightHand);
        }

        virtual public void setHeadModuleAndNeckOffset(Vector3 headModuleOffset, Quaternion hmdToHeadModuleRotation, Vector3 neckOffset) {
            CalibrationAPI.SetHeadModuleAndNeckOffset(HandData.Vector3ToVRfree(headModuleOffset), HandData.QuaternionToVRfree(hmdToHeadModuleRotation), HandData.Vector3ToVRfree(neckOffset));
        }

        virtual public void setWristOffset(Vector3 offset) {
            if(convertForSteamVrGlove) {
                offset = new Quaternion(0, 0, 0.7071068f, 0.7071068f) * offset;
            }
            CalibrationAPI.SetWristOffset(isRightHand, HandData.Vector3ToVRfree(offset));
        }

        virtual protected void release() {
            StopAllCoroutines();
            VRfreeAPI.Shutdown();
        }

        //start-up function
        public void Start() {
            if(VRfreeCamera.Instance == null && VRfreeFixedHeadModulePosition.Instance == null) {
                throw new System.Exception("scene needs eiter a VRfreeCamera or VRfreeFixedHeadModulePosition for VRfreeGlove to work!");
            }

            handAngles = new VRfree.HandAngles().init();
            isPaused = false;
        }

        public void OnEnable() {
            startVRfree();
        }

        void OnApplicationFocus(bool hasFocus) {
            if(disconnectOnPause) {
                if(isPaused && hasFocus) {
                    isPaused = false;
                    Debug.Log("VRfreeGlove application gained focus");
                    startVRfree();
                } else if(!isPaused && !hasFocus) {
                    isPaused = true;
                    Debug.Log("VRfreeGlove application lost focus");
                    release();
                }
            }
        }

        void OnApplicationPause(bool pauseStatus) {
            if(disconnectOnPause) {
                if(isPaused && !pauseStatus) {
                    isPaused = false;
                    Debug.Log("VRfreeGlove application unpaused");
                    startVRfree();
                } else if(!isPaused && pauseStatus) {
                    isPaused = true;
                    Debug.Log("VRfreeGlove application paused");
                    release();
                }
            }
        }

        //shut-down function
        protected void OnDisable() {
            Debug.Log("VRfreeGlove OnDisable");
            //release the VRfree library to indicate that this instance does not use it anymore
            release();
        }

        public void showCalibrationPose(HandData calibrationPose) {
            if(Application.isPlaying)
                showingCalibrationPose = true;
            displayHandData = calibrationPose;
            handTransforms.applyHandData(displayHandData);
            foreach(IPoseReceiver pr in poseReceivers)
                pr.OnPoseChanged();
        }

        public void stopShowingCalibrationPose() {
            showingCalibrationPose = false;
        }

        //update the output
        protected void FixedUpdate() {
            //print a readable status in the editor
            UpdateStatus();

            //get the current hand data
            getHandData();
            getHandAngles();

            //assign the data to the output
            cameraTimestamp = handData.cameraTimestamp;
            timeSinceLastLeftHandData = handData.timeSinceLastLeftHandData;
            timeSinceLastRightHandData = handData.timeSinceLastRightHandData;
            isWristPositionValid = handData.isWristPositionValid;

            // don't change displayHandData when showing calibration pose
            if(!showingCalibrationPose) {
                if (externalTracker != null) {

                //the WristPosition is now the ViveTrackerposition
                handData.wristPosition = externalTracker.position + offset1;
                   

                //Getting the Trackers Rotation

                //with EulerAngles
                //currentEulerAngles = new Vector3(externalTracker.transform.rotation.eulerAngles.x, externalTracker.transform.rotation.eulerAngles.y, externalTracker.transform.rotation.eulerAngles.z);
                //quaternionEullerAngles.eulerAngles = currentEulerAngles;
                 //setting the wrist rotation to the Trackers Rotation
                 //handData.wristRotation = quaternionEullerAngles;

                //with normal rotation info
                 handData.wristRotation = externalTracker.rotation;


                 Debug.Log(handData.wristRotation);
                    Debug.Log(externalTracker.rotation);
                }
                displayHandData = handData;

            }



            handTransforms.applyHandData(displayHandData);

            if(trackerTransform != null && isWristPositionValid)
                 trackerTransform.position = HandData.Vector3FromVRfree(VRfree.VRfreeAPI.GetWristTrackerPosition(isRightHand));


            foreach(IPoseReceiver poseReceiver in poseReceivers) {
                poseReceiver.OnPoseChanged();
            }
        }

        void UpdateStatus() {
            VRfree.StatusCode code = VRfreeAPI.StatusCode();
            deviceStatus = VRfreeStatusCode.statusCodeToString(code);

            deviceError = VRfreeStatusCode.statusCodeToErrorString(code);
        }

        public void findHandTransformsInChildren() {
            string rl = isRightHand ? "_r" : "_l";
            handTransforms.wristTransform = transform.FindDeepChild(VRfree.HandBoneTransforms.BoneIndex.LowerArm.ToString());
            if(handTransforms.wristTransform == null) handTransforms.wristTransform = transform.FindDeepChild("root" + rl);

            handTransforms.handTransform = transform.FindDeepChild(VRfree.HandBoneTransforms.BoneIndex.Hand.ToString());
            if(handTransforms.handTransform == null) handTransforms.handTransform = transform.FindDeepChild("hand" + rl);
            string[] fingers = { "finger_thumb_", "finger_index_", "finger_middle_", "finger_ring_", "finger_pinky_" };
            VRfree.HandBoneTransforms.BoneIndex[] fingerBoneIndices = { VRfree.HandBoneTransforms.BoneIndex.Thumb0, VRfree.HandBoneTransforms.BoneIndex.IndexFinger0,
                    VRfree.HandBoneTransforms.BoneIndex.MiddleFinger0, VRfree.HandBoneTransforms.BoneIndex.RingFinger0, VRfree.HandBoneTransforms.BoneIndex.PinkyFinger0 };
            for(int f = 0; f < 5; f++) {
                for(int i = 0; i < 5; i++) {
                    handTransforms.setFingerTransform(f, i, transform.FindDeepChild(((VRfree.HandBoneTransforms.BoneIndex)((int)fingerBoneIndices[f] + i)).ToString()));
                    if (handTransforms.getFingerTransform(f, i) == null) handTransforms.setFingerTransform(f, i, transform.FindDeepChild(fingers[f] + i + rl));
                }
            }
        }

        public void clearHandTransforms() {
            handTransforms.wristTransform = null;
            handTransforms.handTransform = null;
            for(int f = 0; f < 5; f++) {
                for(int i = 0; i < 5; i++) {
                    handTransforms.setFingerTransform(f, i, null);
                }
            }
        }
    }

    public static class TransformDeepChildExtension {
        //Breadth-first search
        public static Transform FindDeepChild(this Transform aParent, string aName) {
            var result = aParent.Find(aName);
            if(result != null)
                return result;
            foreach(Transform child in aParent) {
                result = child.FindDeepChild(aName);
                if(result != null)
                    return result;
            }
            return null;
        }
    }
}