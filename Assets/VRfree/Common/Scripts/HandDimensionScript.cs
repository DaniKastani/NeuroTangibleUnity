/*
 * For this script to work correctly, all handtransforms should have a scale of (1, 1, 1) and have the bone of the rigged model as first child. 
 */
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VRfreePluginUnity {
    [System.Serializable]
    public struct HandDimensions {
        //public float handLength; //from hand joint to start of middle finger
        public float handWidth;
        public float thumb0Length;
        public float thumb1Length;
        public float thumb2Length;
        public float indexMetaLength;
        public float index0Length;
        public float index1Length;
        public float index2Length;
        public float middleMetaLength;
        public float middle0Length;
        public float middle1Length;
        public float middle2Length;
        public float ringMetaLength;
        public float ring0Length;
        public float ring1Length;
        public float ring2Length;
        public float pinkyMetaLength;
        public float pinky0Length;
        public float pinky1Length;
        public float pinky2Length;

        /*
         * finger going from 0 (thumb) to 4 (pinky finger), index going from -1 to 2
         */
        public float getPhalanxLength(int finger, int index) {
            if(finger == 0 && index == 0)
                return thumb0Length;
            else if(finger == 0 && index == 1)
                return thumb1Length;
            else if(finger == 0 && index == 2)
                return thumb2Length;
            else if(finger == 1 && index == -1)
                return indexMetaLength;
            else if(finger == 1 && index == 0)
                return index0Length;
            else if(finger == 1 && index == 1)
                return index1Length;
            else if(finger == 1 && index == 2)
                return index2Length;
            else if(finger == 2 && index == -1)
                return middleMetaLength;
            else if(finger == 2 && index == 0)
                return middle0Length;
            else if(finger == 2 && index == 1)
                return middle1Length;
            else if(finger == 2 && index == 2)
                return middle2Length;
            else if(finger == 3 && index == -1)
                return ringMetaLength;
            else if(finger == 3 && index == 0)
                return ring0Length;
            else if(finger == 3 && index == 1)
                return ring1Length;
            else if(finger == 3 && index == 2)
                return ring2Length;
            else if(finger == 4 && index == -1)
                return pinkyMetaLength;
            else if(finger == 4 && index == 0)
                return pinky0Length;
            else if(finger == 4 && index == 1)
                return pinky1Length;
            else if(finger == 4 && index == 2)
                return pinky2Length;
            else
                return 0;
        }

        /*
         * finger going from 0 (thumb) to 4 (pinky finger), index going from -1 to 2
         */
        public void setPhalanxLength(float length, int finger, int index) {
            if(finger == 0 && index == 0)
                thumb0Length = length;
            else if(finger == 0 && index == 1)
                thumb1Length = length;
            else if(finger == 0 && index == 2)
                thumb2Length = length;
            else if(finger == 1 && index == -1)
                indexMetaLength = length;
            else if(finger == 1 && index == 0)
                index0Length = length;
            else if(finger == 1 && index == 1)
                index1Length = length;
            else if(finger == 1 && index == 2)
                index2Length = length;
            else if(finger == 2 && index == -1)
                middleMetaLength = length;
            else if(finger == 2 && index == 0)
                middle0Length = length;
            else if(finger == 2 && index == 1)
                middle1Length = length;
            else if(finger == 2 && index == 2)
                middle2Length = length;
            else if(finger == 3 && index == -1)
                ringMetaLength = length;
            else if(finger == 3 && index == 0)
                ring0Length = length;
            else if(finger == 3 && index == 1)
                ring1Length = length;
            else if(finger == 3 && index == 2)
                ring2Length = length;
            else if(finger == 4 && index == -1)
                pinkyMetaLength = length;
            else if(finger == 4 && index == 0)
                pinky0Length = length;
            else if(finger == 4 && index == 1)
                pinky1Length = length;
            else if(finger == 4 && index == 2)
                pinky2Length = length;
        }
    }

    public class HandDimensionScript : MonoBehaviour {
        public bool loadHandDimensionsOnStartup = true;
        public List<HandController> handControllers = new List<HandController>();
        //public HandsTogetherCalibrationLegacy handsTogetherCalibration;

        public HandDimensions initialDimensions;
        public float initialWristToMiddleEndLength;

        public HandDimensions desiredDimensions;
        public float desiredWristToMiddleEndLength;

        private void Start() {
            foreach(HandController handController in handControllers)
                handController.init();
            if(loadHandDimensionsOnStartup)
                loadHandDimensions();
        }

        public void calculateInitialDimensions() {
            if(handControllers.Count < 1)
                return;
            calcuateInitialDimensions(handControllers[0]);
        }

        public void calcuateInitialDimensions(HandController handController) {
            handController.init();

            //initialDimensions.handLength = (handController.handMasterTransforms.handTransform.position - handController.handMasterTransforms.middle1Transform.position).magnitude;
            Vector3 handWidth = handController.handMasterTransforms.knucklesIndexTransform.localPosition - handController.handMasterTransforms.knucklesPinkyTransform.localPosition;
            handWidth.z = 0;
            initialDimensions.handWidth = handWidth.magnitude;

            initialDimensions.thumb0Length = handController.handMasterTransforms.thumb1Transform.localPosition.magnitude;
            initialDimensions.thumb1Length = handController.handMasterTransforms.thumb2Transform.localPosition.magnitude;
            initialDimensions.thumb2Length = handController.handMasterTransforms.thumb2EndTransform.localPosition.magnitude;

            if(handController.handMasterTransforms.indexMetaTransform)
                initialDimensions.indexMetaLength = handController.handMasterTransforms.handTransform.InverseTransformPoint(handController.handMasterTransforms.index0Transform.position).z;
            initialDimensions.index0Length = handController.handMasterTransforms.index1Transform.localPosition.magnitude;
            initialDimensions.index1Length = handController.handMasterTransforms.index2Transform.localPosition.magnitude;
            initialDimensions.index2Length = handController.handMasterTransforms.index2EndTransform.localPosition.magnitude;

            if(handController.handMasterTransforms.middleMetaTransform)
                initialDimensions.middleMetaLength = handController.handMasterTransforms.handTransform.InverseTransformPoint(handController.handMasterTransforms.middle0Transform.position).z;
            initialDimensions.middle0Length = handController.handMasterTransforms.middle1Transform.localPosition.magnitude;
            initialDimensions.middle1Length = handController.handMasterTransforms.middle2Transform.localPosition.magnitude;
            initialDimensions.middle2Length = handController.handMasterTransforms.middle2EndTransform.localPosition.magnitude;

            if(handController.handMasterTransforms.ringMetaTransform)
                initialDimensions.ringMetaLength = handController.handMasterTransforms.handTransform.InverseTransformPoint(handController.handMasterTransforms.ring0Transform.position).z;
            initialDimensions.ring0Length = handController.handMasterTransforms.ring1Transform.localPosition.magnitude;
            initialDimensions.ring1Length = handController.handMasterTransforms.ring2Transform.localPosition.magnitude;
            initialDimensions.ring2Length = handController.handMasterTransforms.ring2EndTransform.localPosition.magnitude;

            if(handController.handMasterTransforms.pinkyMetaTransform)
                initialDimensions.pinkyMetaLength = handController.handMasterTransforms.handTransform.InverseTransformPoint(handController.handMasterTransforms.pinky0Transform.position).z;
            initialDimensions.pinky0Length = handController.handMasterTransforms.pinky1Transform.localPosition.magnitude;
            initialDimensions.pinky1Length = handController.handMasterTransforms.pinky2Transform.localPosition.magnitude;
            initialDimensions.pinky2Length = handController.handMasterTransforms.pinky2EndTransform.localPosition.magnitude;

            initialWristToMiddleEndLength = initialDimensions.middleMetaLength + initialDimensions.middle0Length + initialDimensions.middle1Length + initialDimensions.middle2Length; //handController.handMasterTransforms.handTransform.lossyScale.z * (handController.handMasterTransforms.handTransform.position - handController.handMasterTransforms.middle3EndTransform.position).magnitude;

        }

        public void setIntialAsDesiredDimensions() {
            desiredDimensions = initialDimensions;
            desiredWristToMiddleEndLength = initialWristToMiddleEndLength;
        }

        public void applyDesiredHandDimensions() {

            foreach(HandController handController in handControllers) {
                // change location of handController.handMasterTransforms but apply scale to handController.handTransforms
                calcuateInitialDimensions(handController);
                float handLengthScale = desiredDimensions.middleMetaLength / initialDimensions.middleMetaLength;
                float widthScale = desiredDimensions.handWidth / initialDimensions.handWidth;
                if(handLengthScale != 1.0f || widthScale != 1.0f) {
                    Transform bone = handController.handTransforms.handTransform;
                    bone.localScale = new Vector3(widthScale * bone.localScale.x, widthScale * bone.localScale.y, handLengthScale * bone.localScale.z);
                    bone = handController.handTransforms.wristTransform;
                    bone.localScale = new Vector3(widthScale * bone.localScale.x, widthScale * bone.localScale.y, handLengthScale * bone.localScale.z);

                    Transform knucklePinky = handController.handMasterTransforms.knucklesPinkyTransform;
                    if(knucklePinky) knucklePinky.localPosition = new Vector3(widthScale * knucklePinky.localPosition.x, widthScale * knucklePinky.localPosition.y, handLengthScale * knucklePinky.localPosition.z);
                    Transform knuckleIndex = handController.handMasterTransforms.knucklesIndexTransform;
                    if(knuckleIndex) knuckleIndex.localPosition = new Vector3(widthScale * knuckleIndex.localPosition.x, widthScale * knuckleIndex.localPosition.y, handLengthScale * knuckleIndex.localPosition.z);

                    Transform thumb0 = handController.handMasterTransforms.getFingerTransform(0, 0);
                    thumb0.localPosition = new Vector3(widthScale * thumb0.localPosition.x, widthScale * thumb0.localPosition.y, handLengthScale * thumb0.localPosition.z);

                    for(int i = 1; i < 5; i++) {
                        Transform finger0 = handController.handMasterTransforms.getFingerTransform(i, 0);
                        finger0.parent = handController.handMasterTransforms.handTransform;
                        finger0.localPosition = new Vector3(widthScale * finger0.localPosition.x, widthScale * finger0.localPosition.y, handLengthScale * finger0.localPosition.z);
                    }
                }
                calcuateInitialDimensions(handController);
                for(int finger = 0; finger < 5; finger++) {
                    for(int index = -1; index < 3; index++) {
                        if(index == -1) {
                            if(finger == 0 || finger == 2)
                                continue;

                            float lengthScale = desiredDimensions.getPhalanxLength(finger, -1) / initialDimensions.getPhalanxLength(finger, -1);
                            if(lengthScale == 1.0f && handLengthScale == 1.0f)
                                continue;

                            Transform finger0 = handController.handMasterTransforms.getFingerTransform(finger, 0);
                            finger0.parent = handController.handMasterTransforms.handTransform;
                            finger0.localPosition = new Vector3(finger0.localPosition.x, finger0.localPosition.y, lengthScale * finger0.localPosition.z);

                            Transform meta = handController.handTransforms.getFingerTransform(finger, index);
                            if(meta == null)
                                continue;
                            meta.localScale = handController.glove.convertForSteamVrGlove ?
                                new Vector3(lengthScale * meta.localScale.x, meta.localScale.y, meta.localScale.z)
                                : new Vector3(meta.localScale.x, meta.localScale.y, lengthScale * meta.localScale.z);
                        } else {
                            float lengthScale = desiredDimensions.getPhalanxLength(finger, index) / initialDimensions.getPhalanxLength(finger, index);
                            Transform fingerTransform = handController.handTransforms.getFingerTransform(finger, index);
                            fingerTransform.localScale = handController.glove.convertForSteamVrGlove ?
                                new Vector3(lengthScale * fingerTransform.localScale.x, widthScale * fingerTransform.localScale.y, widthScale * fingerTransform.localScale.z)
                                : new Vector3(widthScale * fingerTransform.localScale.x, widthScale * fingerTransform.localScale.y, lengthScale * fingerTransform.localScale.z);
                            for(int j = 0; j < handController.handMasterTransforms.getFingerTransform(finger, index).childCount; j++) {
                                Transform child = handController.handMasterTransforms.getFingerTransform(finger, index).GetChild(j);
                                child.localPosition = handController.glove.convertForSteamVrGlove ?
                                    new Vector3(lengthScale * child.localPosition.x, widthScale * child.localPosition.y, widthScale * child.localPosition.z)
                                    : new Vector3(widthScale * child.localPosition.x, widthScale * child.localPosition.y, lengthScale * child.localPosition.z);
                            }
                        }
                    }
                }
                handController.moveHandTransforms();
            }
            //if(handsTogetherCalibration != null && handControllers.Count > 0) {
            //    // hand's width has changed, and therefore also the distance between the wrists when the hands are held together
            //    float widthScale = desiredDimensions.handWidth / initialDimensions.handWidth;
            //    handsTogetherCalibration.wristOffsetY = widthScale * handsTogetherCalibration.wristOffsetY;
            //}
        }

        public void applyDesiredWristToMiddleEndLength() {
            if(handControllers.Count < 1)
                return;
            calcuateInitialDimensions(handControllers[0]);
            float scale = desiredWristToMiddleEndLength/initialWristToMiddleEndLength;
            //desiredDimensions.handLength = initialDimensions.handLength * scale;
            desiredDimensions.handWidth = initialDimensions.handWidth * scale;
            for(int finger = 0; finger < 5; finger++) {
                for(int index = -1; index < 3; index++) {
                    desiredDimensions.setPhalanxLength(initialDimensions.getPhalanxLength(finger, index) * scale, finger, index);
                }
            }
            applyDesiredHandDimensions();
        }

        public void saveHandDimensions() {
            if(handControllers.Count < 1)
                return;

            calcuateInitialDimensions(handControllers[0]);

            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Sensoryx\\";
            string filename = path + "HandDimensions";
            foreach(HandController handController in handControllers) {
                if(!handController.glove.isRightHand) {
                    filename += "L";
                    break;
                }
            }
            foreach(HandController handController in handControllers) {
                if(handController.glove.isRightHand) {
                    filename += "R";
                    break;
                }
            }
            filename += ".json";
            string contents = JsonUtility.ToJson(initialDimensions);
            Directory.CreateDirectory(path);
            File.WriteAllText(filename, contents);
            Debug.Log("Saved HandDimensions " + filename + ":\n" + contents);
        }

        public void loadHandDimensions() {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData) + "\\Sensoryx\\";
            string filename = path + "HandDimensions";
            foreach(HandController handController in handControllers) {
                if(!handController.glove.isRightHand) {
                    filename += "L";
                    break;
                }
            }
            foreach(HandController handController in handControllers) {
                if(handController.glove.isRightHand) {
                    filename += "R";
                    break;
                }
            }
            filename += ".json";
            try {
                desiredDimensions = JsonUtility.FromJson<HandDimensions>(File.ReadAllText(filename));

                // applyDesiredHandDimensions is iterative and doesn't really work well with big changes, therefore call multiple times
                for(int i = 0; i < 5; i++)
                    applyDesiredHandDimensions();
            } catch {
                Debug.Log("couldn't load hand dimensions");
            }
        }
    }
}