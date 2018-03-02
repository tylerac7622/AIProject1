using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // member data for each camera
    //public GameObject IsometricCamera;
    //public GameObject ThirdPersonCamera;
    //public GameObject FlockerCamera;

    // member data for each camera
    public List<GameObject> camList;
    private int currentCam;

    // Use this for initialization
    void Start ()
    {
        currentCam = 0;

        //set all other cameras inactive
        for (int i = 0; i < camList.Count; i++)
        {
            camList[i].SetActive(false);
        }

        camList[currentCam].SetActive(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
        //change camera view on pressing enter
        if (Input.GetKeyDown(KeyCode.Return))
        {
            currentCam++;//increment cam index

            //reset to the beginning if on last camera
            if(currentCam >= camList.Count)
            {
                currentCam = 0;
            }
            
            //set all other cameras inactive
            for(int i = 0; i < camList.Count; i++)
            {
                camList[i].SetActive(false);
            }

            camList[currentCam].SetActive(true);
            
        }
	}
}
