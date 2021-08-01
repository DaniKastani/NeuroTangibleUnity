/*
 * When using a VRfree Head Module that is attached to the HMD, attach this script to the VR camera Transform in the scene as a position and orientation reference.
 * The VRfree position tracking needs at least one VRfreeCamera or VRfreeFixedHeadModulePosition script in the scene!
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRfreePluginUnity {     
    [ScriptOrder(-101)]
    public class VRfreeCamera : MonoBehaviour {
        public static VRfreeCamera Instance;
        void Start() {
            if(Instance == null && VRfreeFixedHeadModulePosition.Instance == null) {
                Instance = this;
                Debug.Log("Registering VRfreeCamera Instance.");
                VRfree.VRfreeAPI.Init();
                VRfree.VRfreeAPI.Start();
            } else {
                this.enabled = false;
                Debug.Log("Another VRfreeCamera or VRfreeFixedHeadModulePosition already active in scene, disabling.");
            }
        }

        private void OnDisable() {
            VRfree.VRfreeAPI.Shutdown();
        }

        void FixedUpdate() {
            VRfree.VRfreeAPI.UpdateCameraPose(HandData.Vector3ToVRfree(transform.position), HandData.QuaternionToVRfree(transform.rotation), false);
        }
    }
}
