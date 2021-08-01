using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetPosition : MonoBehaviour
{

    public Vector3 pos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //position of this GameObject
        pos = transform.position;

        //position of the Tracker
        Vector3 trackerpos = GameObject.Find("TrackedGameObject").transform.position;

        //offset position needs to be corrected to
        Vector3 posCalculator = new Vector3(-1*(-0.0587f),0,-0.0172f);


        //new position is Trackers position plus the offset calculation
        pos = trackerpos + posCalculator;
        

        transform.position = pos;



       
        
    }
}
