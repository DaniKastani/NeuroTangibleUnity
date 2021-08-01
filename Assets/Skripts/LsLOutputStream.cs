using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class LsLOutputStream : MonoBehaviour
{
    private StreamOutlet outl;
    private float[] currentSample;

    public string StreamName = "Unity.ExampleStream";
    public string StreamType = "Unity.StreamType";
    public string StreamId = "MyStreamID-Unity1234";

    public string[] sample;
    public string[] startingsample;
    private string[] endsample;

    int collisioncount = 0;
    bool itstarted = false;
    bool istouched = false;

    // Start is called before the first frame update
    void Start()
    {
        StreamInfo inf = new StreamInfo("Unity Markers", "Markers", 1, 0, channel_format_t.cf_string, "giu4569");
        outl = new StreamOutlet(inf);

        startingsample = new string[1];
        startingsample[0] = "So it begins";

        sample = new string[1];
        sample[0] = "UwU";

        endsample = new string[1];
        endsample[0] = "Its over. Its done";

        System.Threading.Thread.Sleep(5000);
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
        if (!itstarted)
        {
            outl.push_sample(startingsample);
            Debug.Log("So it begins");
            itstarted = true;

        }
        // send a marker and wait for a random interval
        if (istouched)
        {
            outl.push_sample(sample);
            istouched = false;
            Debug.Log("UwU");
        }

    }

    private void OnDestroy()
    {
        outl.push_sample(endsample);
        Debug.Log("Its over. Its done");
        
    }
}