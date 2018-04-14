using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaceUnit : MonoBehaviour {

    //The text to update for unit placement when level of influence is changed
    public Text unitPlacementText;
    //prefabs for the different types of units to be instantiated
    public GameObject black;
    public GameObject yellow;
    public GameObject blue;
    public GameObject white;

    private int currentInfluence;//strength of the current selected unit
    private InfluenceMap iM;

	// Use this for initialization
	void Start () {
        currentInfluence = 1;
        iM = GameObject.Find("InfluenceGrid").GetComponent<InfluenceMap>();
	}
	
	// Update is called once per frame
	void Update () {
        InfluenceChange();
        Place();
	}

    private void Place()
    {
        int layerMask = 1 << 8;

        //check which mouse button was clicked and what the current influence is set to
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = GameObject.Find("Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            //if the raycast hits something go ahead and place a unit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //if the terrain is underwater do not allow the user to place units
                if (hit.point.y < 49) return;

                GameObject prefabToUse = null;
                switch(currentInfluence)
                {
                    case 1: prefabToUse = white;
                        break;
                    case 2: prefabToUse = blue;
                        break;
                    case 3: prefabToUse = yellow;
                        break;
                    case 4: prefabToUse = black;
                        break;
                }
                //instantiate the unit at the exact point where the user clicked
                GameObject unit = Instantiate(prefabToUse, hit.point, Quaternion.identity);
                unit.transform.position += new Vector3(0, unit.transform.localScale.y/2, 0);
                unit.GetComponent<InfluencerData>().team = false;
                iM.PlaceUnit(unit);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = GameObject.Find("Camera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

            //if the raycast hits something go ahead and place a unit
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                //if the terrain is underwater do not allow the user to place units
                if (hit.point.y < 49) return;

                GameObject prefabToUse = null;
                switch (currentInfluence)
                {
                    case 1:
                        prefabToUse = white;
                        break;
                    case 2:
                        prefabToUse = blue;
                        break;
                    case 3:
                        prefabToUse = yellow;
                        break;
                    case 4:
                        prefabToUse = black;
                        break;
                }
                //instantiate the unit at the exact point where the user clicked
                GameObject unit = Instantiate(prefabToUse, hit.point, Quaternion.identity);
                unit.transform.position += new Vector3(0, unit.transform.localScale.y / 2, 0);
                //right mouse button clicked so team is green
                unit.GetComponent<InfluencerData>().team = true;
                iM.PlaceUnit(unit);
            }
            else
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                Debug.Log("Did not Hit");
            }
        }

    }

    /// <summary>
    /// Changes the type of unit being placed down by level of strength 
    /// </summary>
    private void InfluenceChange()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            currentInfluence = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            currentInfluence = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
        {
            currentInfluence = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            currentInfluence = 4;
        }

        unitPlacementText.text = "Placement Controls: \nCurrent Strength: "+currentInfluence+
            " \n1,2,3,4: Switch to unit of \nCorresponding Strength \nLeft Click: Red Team Placement \nRight Click: Green Team Placement";
    }

}
