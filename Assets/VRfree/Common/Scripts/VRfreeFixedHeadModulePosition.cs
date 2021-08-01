/*
 * When using a VRfree Head Module that is in a stationary position (fixed to the environment), attach this script to the VR camera Transform in the scene as a position and orientation reference.
 * The VRfree position tracking needs at least one VRfreeCamera or VRfreeFixedHeadModulePosition script in the scene!
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRfreePluginUnity {
    public class VRfreeFixedHeadModulePosition : MonoBehaviour {
        public static VRfreeFixedHeadModulePosition Instance;

        void Start() {
            if (Instance == null && VRfreeCamera.Instance == null) {
                Instance = this;
                Debug.Log("Registering VRfreeFixedHeadModulePosition Instance.");
                VRfree.VRfreeAPI.Init();
            } else {
                this.enabled = false;
                Debug.Log("Another VRfreeFixedHeadModulePosition or VRfreeCamera already active in scene, disabling.");
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
