using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public bool debug;

	// Use this for initialization
	void Start ()
    {
        debug = true;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    /// <summary>
    /// Tyler Coppenbarger
    /// Gets the closest point to the passed in position (this will need to check island numbers eventually to prevent river crossing)
    /// return : the pathPoint closest to the passed in vector
    /// </summary>
    public PathPoint ClosestPoint(Vector2 check)
    {
        PathPoint result = null;
        for (int i = 0; i < transform.childCount; i++)
        {
            if(result == null || Vector2.Distance(check, transform.GetChild(i).GetComponent<PathPoint>().smallPos) < Vector2.Distance(check, result.smallPos))
            {
                result = transform.GetChild(i).GetComponent<PathPoint>();
            }
        }
        return result;
    }

    /// <summary>
    /// Tyler Coppenbarger
    /// Creates an A* path when passed in a start and end pathPoint (eventually make this work with any position, not just pathPoints
    /// return: a stack of the shortest path between the two points
    /// </summary>
    public Stack<PathPoint> CreatePath(PathPoint start, PathPoint end)
    {
        //start from end, work backward making path to start
        Stack<PathPoint> result = new Stack<PathPoint>();

        // this object's children are all just path points

        //boolean to determine which points have been checked
        bool[] allChecked = new bool[transform.childCount];
        for (int i = 0; i < allChecked.Length; i++)
        {
            allChecked[i] = false;
        }
        //the end is the last point in the stack
        result.Push(end);
        allChecked[end.transform.GetSiblingIndex()] = true;
        bool pathEnded = false;
        PathPoint currentPoint = end;
        //end the loop when start is reached (whenever start is adjacent to the currentPoint
        while (!pathEnded)
        {
            PathPoint shortest = null;
            for (int i = 0; i < currentPoint.adjacent.Count; i++)
            {
                PathPoint checkingPoint = currentPoint.adjacent[i].GetComponent<PathPoint>();
                //choose the adjacent point that is closest to start
                if (!allChecked[checkingPoint.transform.GetSiblingIndex()] && (shortest == null || Vector2.Distance(start.smallPos, checkingPoint.smallPos) < Vector2.Distance(start.smallPos, shortest.smallPos)))
                {
                    shortest = checkingPoint;
                }
            }
            //cant go anywhere from here, so pop off the latest addition and try again
            if (shortest == null)
            {
                result.Pop();
                if (result.Count == 0)
                {
                    Debug.Log("There is no possible path. THIS SHOULDN'T HAPPEN.");
                }
                currentPoint = result.Peek();
            }
            else
            {
                //if there is a valid adjacent point (shortest), make it the currentPoint, say it has been checked, and add it to the result stack
                result.Push(shortest);
                currentPoint = shortest;
                allChecked[currentPoint.transform.GetSiblingIndex()] = true;
                if(shortest == start)
                {
                    pathEnded = true;
                }
            }
        }
        Debug.Log("New path!");
        return result;
    }
}
