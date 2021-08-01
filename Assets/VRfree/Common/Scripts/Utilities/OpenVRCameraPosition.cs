using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
#if !UNITY_ANDROID
using Valve.VR;
#endif

[DefaultExecutionOrder(-10000)]
public class OpenVRCameraPosition : MonoBehaviour {
    public Transform fallbackPosition;
    public float predictionTime = 0;
#if !UNITY_ANDROID
    private TrackedDevicePose_t[] poseArr = new TrackedDevicePose_t[1];
#endif
    public bool usingOpenVR = false;

    void Start() {
        usingOpenVR = XRSettings.loadedDeviceName == "OpenVR";
        Debug.Log(XRSettings.loadedDeviceName);
    }

    void Update() {
        UpdatePose();
    }

    void FixedUpdate() {
        UpdatePose();
    }

    void UpdatePose() {
#if !UNITY_ANDROID
        if (usingOpenVR) {
            Valve.VR.OpenVR.System.GetDeviceToAbsoluteTrackingPose(Valve.VR.OpenVR.Compositor.GetTrackingSpace(), predictionTime, poseArr);
            OVR_Utils.RigidTransform pose = new OVR_Utils.RigidTransform(poseArr[0].mDeviceToAbsoluteTracking);

            transform.localPosition = pose.pos;
            transform.localRotation = pose.rot;
        } else {
#endif
            transform.position = fallbackPosition.position;
            transform.rotation = fallbackPosition.rotation;
#if !UNITY_ANDROID
        }
#endif
    }
}