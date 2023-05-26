using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Since we only have 1 train now, as POC lets just track this train as we move along yea
public class UITrain : MonoBehaviour
{

    public Camera WorldCam;
    public GameObject train;


    public void CamFollowObject()
    {
        WorldCamera wc = WorldCam.GetComponent<WorldCamera>();
        Debug.Log(wc);
        StartCoroutine(wc.followtrain(train));
    }
}
