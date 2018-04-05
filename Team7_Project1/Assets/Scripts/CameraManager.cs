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
    //private Pathfinder pather;

    //private PathManager pMan;
    //private FlockingManager fMan;

    private float zoomSpeed = 1;
    private float moveSpeed = 2;

    // Use this for initialization
    void Start ()
    {
        //pather = GameObject.Find("AStarFollower").GetComponent<Pathfinder>();
        //pMan = GameObject.Find("PathManager").GetComponent<PathManager>();
        //fMan = GameObject.Find("FlockingManager").GetComponent<FlockingManager>();

        currentCam = 0;

        //set all other cameras inactive
        for (int i = 0; i < camList.Count; i++)
        {
            camList[i].SetActive(false);
        }

        camList[currentCam].SetActive(true);

        /*for (int i = 0; i < pMan.transform.childCount; i++)
        {
            pMan.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
        }*/
        /*for (int i = 0; i < fMan.forwardTargets.Length; i++)
        {
            fMan.forwardTargets[i].GetComponent<MeshRenderer>().enabled = false;
        }*/
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (currentCam == 0 && Input.GetButtonDown("Fire1"))
        {
            Ray ray = camList[currentCam].GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.name == "Terrain")
                {
                    //pather.ResetPath(new Vector2(hit.point.x, hit.point.z));
                }
                else if (hit.collider.name.StartsWith("Bound") || hit.collider.name.StartsWith("Water"))
                {
                    //clicked on ocean
                }
                else
                {
                    //clicked on object
                }
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            camList[currentCam].transform.Translate(new Vector3(0, moveSpeed, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            camList[currentCam].transform.Translate(new Vector3(-moveSpeed, 0, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            camList[currentCam].transform.Translate(new Vector3(0, -moveSpeed, 0));
        }
        if (Input.GetKey(KeyCode.D))
        {
            camList[currentCam].transform.Translate(new Vector3(moveSpeed, 0, 0));
        }
        if (Input.GetKey(KeyCode.Q))
        {
            camList[currentCam].GetComponent<Camera>().fieldOfView -= zoomSpeed;
        }
        if (Input.GetKey(KeyCode.E))
        {
            camList[currentCam].GetComponent<Camera>().fieldOfView += zoomSpeed;
        }
        if(camList[currentCam].GetComponent<Camera>().fieldOfView < 5)
        {
            camList[currentCam].GetComponent<Camera>().fieldOfView = 5;
        }
        if (camList[currentCam].GetComponent<Camera>().fieldOfView > 90)
        {
            camList[currentCam].GetComponent<Camera>().fieldOfView = 90;
        }
        //change camera view on pressing enter
        if (Input.GetKeyDown(KeyCode.D))
        {
            /*for(int i = 0; i < pMan.transform.childCount; i++)
            {
                pMan.transform.GetChild(i).GetChild(0).gameObject.SetActive(!pMan.transform.GetChild(i).GetChild(0).gameObject.activeSelf);
            }*/
            /*for (int i = 0; i < fMan.forwardTargets.Length; i++)
            {
                fMan.forwardTargets[i].GetComponent<MeshRenderer>().enabled = !fMan.forwardTargets[i].GetComponent<MeshRenderer>().enabled;
            }*/
        }
        //change camera view on pressing enter
        /*if (Input.GetKeyDown(KeyCode.Return))
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
            
        }*/
	}
}
