using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;

public class LSLOutputOriginal : MonoBehaviour
{
    private StreamOutlet outlet;
    private string[] startstream;
    private string[] touchedstream;
    private string[] endstream;

    public string StreamName = "Unity.ExampleStream";
    public string StreamType = "Unity.SteamType";
    public string StreamId = "MyStreamID-Unity1234";

    int collisioncount = 0;
    bool istouched = false;
    

    // Start is called before the first frame update
    void Start()
    {
        StreamInfo streamInfo = new StreamInfo(StreamName, StreamType, 1, 0, channel_format_t.cf_float32);
        outlet = new StreamOutlet(streamInfo);

        startstream = new string[1];
        touchedstream = new string[1];
        endstream = new string[1];

        startstream[0] = "and so it beginns";
        outlet.push_sample(startstream);
        Debug.Log("and so it beginns");
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
            touchedstream[0] = "touched";
            outlet.push_sample(touchedstream);
            istouched = false;
            Debug.Log("It was touched");
        }
    }

    private void OnApplicationQuit()
    {
        endstream[0] = "Its over. Its done.";
        outlet.push_sample(endstream);

        Debug.Log("Its over. ITs done");

    }
}