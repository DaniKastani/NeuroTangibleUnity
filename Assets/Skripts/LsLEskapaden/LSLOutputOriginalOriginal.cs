using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class LSLOutputOriginalOriginal : MonoBehaviour
{
    private StreamOutlet outlet;
    private float[] currentSample;

    public string StreamName = "Unity.ExampleStream";
    public string StreamType = "Unity.StreamType";
    public string StreamId = "MyStreamID-Unity1234";

    int collisioncount = 0;
    bool istouched = false;

    // Start is called before the first frame update
    void Start()
    {
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 3, Time.fixedDeltaTime * 1000, channel_format_t.cf_float32);
        XMLElement chans = streamInfo.desc().append_child("channels");
        chans.append_child("channel").append_child_value("label", "X");
        chans.append_child("channel").append_child_value("label", "Y");
        chans.append_child("channel").append_child_value("label", "Z");
        outlet = new StreamOutlet(streamInfo);
        currentSample = new float[3];
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
            istouched = true;
            Debug.Log(istouched);
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (!(collision.collider.name == "Table"))
        {
            collisioncount--;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {


        if (istouched)
        {
            Vector3 pos = gameObject.transform.position;
            currentSample[0] = pos.x;
            currentSample[1] = pos.y;
            currentSample[2] = pos.z;
            outlet.push_sample(currentSample);

            istouched = false;
            Debug.Log("It was touched");

        }
    }
}