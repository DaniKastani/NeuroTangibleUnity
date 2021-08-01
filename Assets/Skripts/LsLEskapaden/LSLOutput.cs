using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class LSLOutput : MonoBehaviour
{
    private StreamOutlet outlet;
    private float[] currentSample;
    private string[] startstream;
    private string[] collisionmarker;
    private string[] endstream;

    public string StreamName = "Unity.ExampleStream";
    public string StreamType = "Unity.StreamType";
    public string StreamId = "MyStreamID-Unity1234";

    int collisioncount = 0;


    // Start is called before the first frame update
    void Start()
    {
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, Time.fixedDeltaTime * 1000,  channel_format_t.cf_float32);
         XMLElement chans = streamInfo.desc().append_child("channel");
        chans.append_child("channel").append_child_value("label", "marker");
        outlet = new  StreamOutlet(streamInfo);

        startstream = new string[1];
        collisionmarker = new string[1];
        endstream = new string[1];


        //stream when its starts
        startstream[0] = "and so it beginns";
        outlet.push_sample(startstream);
        Debug.Log(startstream[0]);
    }


    void OnCollisionEnter(Collision collision)
    {

        //define what object should not influence the count. Like other gameobjects
        if (!(collision.collider.name == "Table"))
        {
            collisioncount++;
        }

        if (collisioncount == 1)
        {
            collisionmarker[0] = "collison detected";
            outlet.push_sample(collisionmarker);

            Debug.Log(collisionmarker[0]);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (!(collision.collider.name == "Table"))
        {
            collisioncount--;
        }
    }

    private void OnApplicationQuit()
    {
        //stream when it ends
        endstream[0] = "Its over. Its done.";
        outlet.push_sample(endstream);
        Debug.Log(endstream[0]);
        
    }



    // Update is called once per frame
    /*  void FixedUpdate()
      {

          if (collisioncount == 1)
          {
              collisionmarker[0] = "collison detected";
              outlet.push_sample(collisionmarker);

              Debug.Log(collisionmarker[0]);
          }

          /*
          Vector3 pos = gameObject.transform.position;
          currentSample[0] = pos.x;
          currentSample[1] = pos.y;
          currentSample[2] = pos.z;
          outlet.push_sample(currentSample);
      }*/


}