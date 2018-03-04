using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : BaseMovement
{
    Stack<PathPoint> path = new Stack<PathPoint>();
    PathManager manager;

    // Use this for initialization
    void Start ()
    {
        manager = GameObject.Find("PathManager").GetComponent<PathManager>();
        base.Start();
    }

    /// <summary>
    /// Tyler Coppenbarger
    /// Recreates the A* path if there currently is no path, or check the character's position against the existing path
    /// </summary>
    void Update ()
    {
        if (path.Count > 0)
        {
            if (Vector2.Distance(path.Peek().smallPos, new Vector2(transform.position.x, transform.position.z)) < .2f)
            {
                path.Pop();
            }
        }
        else
        {
            //not actually random, just named so
            int rand = manager.ClosestPoint(new Vector2(transform.position.x, transform.position.z)).transform.GetSiblingIndex();
            int rand2 = Random.Range(0, manager.transform.childCount);
            while (rand == rand2)
            {
                rand2 = Random.Range(0, manager.transform.childCount);
            }
            //path = manager.CreatePath(new Vector2(transform.position.x, transform.position.z), manager.transform.GetChild(rand).GetComponent<PathPoint>());
            path = manager.CreatePath(manager.transform.GetChild(rand).GetComponent<PathPoint>(), manager.transform.GetChild(rand2).GetComponent<PathPoint>());
        }

        base.Update();
	}

    /// <summary>
    /// Tyler Coppenbarger
    /// Steers the character towards the first point in the path, using BaseMovement.cs's methods
    /// </summary>
    protected override void CalcSteeringForces()
    {
        Vector3 ultForce = Vector3.zero;
        Vector3 target = new Vector3(path.Peek().smallPos.x, 0, path.Peek().smallPos.y);
        target.y = GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(target.x, 0, target.y));
        ultForce += Seek(target) * 5;

        //limit by max force
        Vector3.ClampMagnitude(ultForce, 10);
        Debug.Log(ultForce);
        //apply ultimate force
        ApplyForce(ultForce);
    }
}
