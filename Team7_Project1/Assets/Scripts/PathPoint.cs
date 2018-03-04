using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    public List<GameObject> adjacent;
    public Vector2 smallPos;
    public PathPoint parent;
    public bool obstacle;

	// Use this for initialization
	void Start ()
    {
        smallPos = new Vector2(transform.position.x, transform.position.z);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
