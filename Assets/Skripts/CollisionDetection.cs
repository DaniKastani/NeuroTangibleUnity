using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

    int collisioncount = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Debug.Log("it was touched");
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (!(collision.collider.name == "Table"))
        {
            collisioncount--;
        }
    }
}
