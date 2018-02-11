using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // member data for each camera
    public GameObject IsometricCamera;
    public GameObject ThirdPersonCamera;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            IsometricCamera.SetActive(!IsometricCamera.activeSelf);
            ThirdPersonCamera.SetActive(!ThirdPersonCamera.activeSelf);
        }
	}
}
