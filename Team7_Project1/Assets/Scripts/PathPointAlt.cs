using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPointAlt : MonoBehaviour {

    public PathPointAlt parent;
    public PathManagerAlt manager;

    // true = obstacle
    // false = not obstacle
    public bool obstacle;

    public int x;
    public int z;

    public float score;

    // Use this for initialization
    void Start () {

        // convert float position to int (exact numbers in test case)
        x = (int)transform.position.x;
        z = (int)transform.position.z;

        // set node matrix up
        manager.nodeMatrix[x, z] = this;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
