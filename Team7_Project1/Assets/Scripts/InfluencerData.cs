using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluencerData : MonoBehaviour
{
    // position on grid
    public int gridX;
    public int gridY;

    // team
    // true = green
    // false = red
    public bool team;

    // influence
    public int influence;

	// Use this for initialization
	void Start ()
    {
		// set the influence value based on the object's tag
        switch(tag)
        {
            case "Black":
                influence = 3;
                break;
            case "Yellow":
                influence = 2;
                break;
            case "Blue":
                influence = 1;
                break;
            case "White":
                influence = 0;
                break;
            default:
                Debug.Log("This object should not have an InfluencerData script!");
                break;
        }

        // negative sign if on the red team
        if (!team)
        {
            influence *= -1;
        }

        // DEBUG
        Debug.Log("influence = " + influence);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
