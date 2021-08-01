using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VivePositionOveride : MonoBehaviour
{
    public GameObject ViveTrackerHolder;

    // Start is called before the first frame update
    void Start()
    {
       
        this.gameObject.transform.position = ViveTrackerHolder.transform.position;
        
    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.position = ViveTrackerHolder.transform.position;

    }
}
