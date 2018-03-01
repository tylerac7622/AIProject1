using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{


	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

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

    public Stack<PathPoint> CreatePath(PathPoint start, PathPoint end)
    {
        //start from end, work backward making path to start
        Stack<PathPoint> result = new Stack<PathPoint>();
        bool[] allChecked = new bool[transform.childCount];
        for (int i = 0; i < allChecked.Length; i++)
        {
            allChecked[i] = false;
        }
        result.Push(end);
        allChecked[end.transform.GetSiblingIndex()] = true;
        bool pathEnded = false;
        PathPoint currentPoint = end;
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
                result.Push(shortest);
                currentPoint = shortest;
                allChecked[currentPoint.transform.GetSiblingIndex()] = true;
                if(shortest == start)
                {
                    pathEnded = true;
                }
            }
        }
        return result;
    }
}
