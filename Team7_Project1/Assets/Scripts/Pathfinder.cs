using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    Stack<PathPoint> path = new Stack<PathPoint>();
    PathManager manager;

    float movementSpeed = 5;

    // Use this for initialization
    void Start ()
    {
        manager = GameObject.Find("PathManager").GetComponent<PathManager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (path.Count > 0)
        {
            //move towards path.peek()
            Vector2 pos = Vector2.MoveTowards(new Vector2(transform.position.x, transform.position.z), path.Peek().smallPos, Time.deltaTime * movementSpeed);
            float terrainHeight = GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(pos.x, 0, pos.y));
            transform.position = new Vector3(pos.x, terrainHeight, pos.y);
            Quaternion oldRotation = transform.rotation;
            transform.LookAt(new Vector3(path.Peek().smallPos.x, transform.position.y, path.Peek().smallPos.y));
            transform.rotation = Quaternion.Lerp(oldRotation, transform.rotation, Time.deltaTime * movementSpeed);

            if (Vector2.Distance(path.Peek().smallPos, new Vector2(transform.position.x, transform.position.z)) < .2f)
            {
                path.Pop();
            }
        }
        else
        {
            int rand = manager.ClosestPoint(new Vector2(transform.position.x, transform.position.z)).transform.GetSiblingIndex();
            int rand2 = Random.Range(0, manager.transform.childCount);
            while (rand == rand2)
            {
                rand2 = Random.Range(0, manager.transform.childCount);
            }
            //path = manager.CreatePath(new Vector2(transform.position.x, transform.position.z), manager.transform.GetChild(rand).GetComponent<PathPoint>());
            path = manager.CreatePath(manager.transform.GetChild(rand).GetComponent<PathPoint>(), manager.transform.GetChild(rand2).GetComponent<PathPoint>());
        }
	}

    List<PathPoint> ConstructPath (PathPoint start, PathPoint end)
    {

        return null;
    }
}
