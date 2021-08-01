using UnityEngine;
using System.Collections.Generic;
using VRfreeAPI = VRfree.VRfreeAPI;
using CalibrationAPI = VRfree.CalibrationAPI;

namespace VRfreePluginUnity
{
    //sets the Rotation and Position of the Gameobect to where the Handposition and rotation is

    public class GetWristRotation : MonoBehaviour
    {

        public GameObject WristTracker;

        private Vector3 currentEulerAngles;
        Quaternion newrotation;
        private Vector3 newpostition;

        // Start is called before the first frame update
        void Start()
        {
       
        }

        // Update is called once per frame
        void Update()
        {         
         newrotation = WristTracker.GetComponent<VRfreeGlove>().handData.wristRotation;
         newpostition = WristTracker.GetComponent<VRfreeGlove>().handData.wristPosition;

         this.gameObject.transform.position = newpostition;
         this.gameObject.transform.rotation = newrotation;
        
   

         
          //currentEulerAngles = new Vector3(ViveTracker.transform.eulerAngles.x, ViveTracker.transform.eulerAngles.y, ViveTracker.transform.eulerAngles.z);
          //this.gameObject.transform.eulerAngles = currentEulerAngles;
            

        }
    }
}
