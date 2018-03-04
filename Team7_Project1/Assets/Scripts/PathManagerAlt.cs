using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManagerAlt : MonoBehaviour {

    private bool[,] boolMatrix;
    public PathPointAlt[,] nodeMatrix;
    private int matWidth;
    private int matHeight;

    private float costSoFar;

    private bool didTestCase;

	// Use this for initialization
	void Start () {
        didTestCase = false;

        // set width and height arbitrarily
        matWidth = 10;
        matHeight = 10;

        // define the map matrix
        // true = is obstacle
        // false = not obstacle
        boolMatrix = new bool[matWidth, matHeight];
        nodeMatrix = new PathPointAlt[matWidth, matHeight];

        // test case mostly not obstacles, so set everything true first
        for (int i = 0; i < matWidth; i++)
        {
            for (int j = 0; j < matHeight; j++)
            {
                boolMatrix[i, j] = false;
                //nodeMatrix[i, j] =
            }
        }

        // obstacles for hard-coded test case
        boolMatrix[0, 0] = true;
        boolMatrix[1, 0] = true;

        boolMatrix[0, 1] = true;

        boolMatrix[3, 3] = true;
        boolMatrix[4, 3] = true;
        boolMatrix[5, 3] = true;
        boolMatrix[6, 3] = true;
        boolMatrix[7, 3] = true;

        boolMatrix[7, 4] = true;

        boolMatrix[7, 5] = true;

        boolMatrix[7, 6] = true;

        boolMatrix[0, 7] = true;
        boolMatrix[1, 7] = true;
        boolMatrix[2, 7] = true;

        boolMatrix[0, 8] = true;

        boolMatrix[6, 9] = true;
        boolMatrix[7, 9] = true;
    }

    // Update is called once per frame
    void Update () {

		if (!didTestCase)
        {
            didTestCase = true;
            /*
            for (int j = 0; j < matWidth; j++)
            {
                for (int i = 0; i < matHeight; i++)
                {
                    //if (nodeMatrix[i, j].obstacle)
                    //{
                    //    Debug.Log("obstacle at (" + i + ", " + j + ") = (" + nodeMatrix[i,j].x + ", " + nodeMatrix[i,j].z + ")");
                    //}
                    //Debug.Log("at index (" + i + ", " + j + "): (" + nodeMatrix[i, j].x + ", " + nodeMatrix[i, j].z + ")");
                }
            }
            //*/

            /*
            Stack<PathPointAlt> path = ConstructPath(nodeMatrix[4, 5], nodeMatrix[6, 1]);
            while(path.Count > 0)
            {
                PathPointAlt current = path.Pop();
                Debug.Log("(" + current.x + ", " + current.z + ")");
            }
            //*/
        }
	}

    Stack<PathPointAlt> ConstructPath(PathPointAlt start, PathPointAlt end)
    {
        // preliminary step: new path, so no node has a parent
        for (int i = 0; i < matWidth; i++)
        {
            for (int j = 0; j < matHeight; j++)
            {
                nodeMatrix[i, j].parent = null;
            }
        }

        // create open and closed lists
        List<PathPointAlt> open = new List<PathPointAlt>();
        List<PathPointAlt> closed = new List<PathPointAlt>();
        Stack<PathPointAlt> path = new Stack<PathPointAlt>();

        PathPointAlt current;
        costSoFar = 0.0f;

        // add starting node to open list
        open.Add(start);
        while(open.Count > 0)
        {
            // get this first node in the open list, should be the cheapest
            current = open[0];
            if (current == end)
            {
                Debug.Log("Completed path!");

                // get to start node via parent chain
                path.Push(current);
                while(current.parent != null)
                {
                    path.Push(current.parent);
                }

                return path;
            }
            else
            {
                // take current out of open and move it to closed
                open.Remove(current);
                closed.Add(current);

                // update the cost so far
                //costSoFar = ????

                // get all the nodes adjacent to this one
                List<PathPointAlt> adjacent = GetAdjacent(current.x, current.z);

                // check nodes
                for (int i = 0; i < adjacent.Count; i++)
                {
                    // if in neither list and not obstacle
                    if (!open.Contains(adjacent[i]) && !closed.Contains(adjacent[i]) && !adjacent[i].obstacle)
                    {
                        // set its parent to the current node
                        adjacent[i].parent = current;

                        // calculate score = cost + heuristic
                        adjacent[i].score = CalculateScore(adjacent[i], current, start, end);

                        // add to open list
                        open.Add(adjacent[i]);
                    }
                } // end for

                // sort open list
                SortList(open);

            } // end else
        } // end while

        return path;
    }

    float CalculateScore(PathPointAlt node, PathPointAlt current, PathPointAlt start, PathPointAlt end)
    {
        // old
        //costSoFar = CalculateCost(node, current);
        //return costSoFar + CalculateHeuristic(node, end);

        // new
        //return CalculateCost(node, current) + CalculateHeuristic(node, end);

        // new alternate, with cost being determined by start, rather than current
        return CalculateCost(node, start) + CalculateHeuristic(node, end);
    }

    float CalculateCost(PathPointAlt node, PathPointAlt current)
    {
        return costSoFar + Vector3.Distance(current.transform.position, node.transform.position);
    }

    float CalculateHeuristic(PathPointAlt node, PathPointAlt end)
    {
        return Vector3.Distance(node.transform.position, end.transform.position);
    }

    void SortList(List<PathPointAlt> list)
    {
        int last = list.Count - 1;
        for (int i = 0; i < last; i++)
        {
            int min = i;
            for (int j = i+1; j < list.Count; j++)
            {
                if (list[j].score < list[min].score)
                {
                    min = j;
                }
            }

            if (min != i)
            {
                PathPointAlt temp = list[min];

                list[min] = list[i];
                list[i] = temp;
            }
        }
    }

    List<PathPointAlt> GetAdjacent(int ptX, int ptZ)
    {
        List<PathPointAlt> result = new List<PathPointAlt>();
        // loop thru grid
        for (int x = ptX - 1; x <= ptX + 1; x++)
        {
            for (int z = ptZ - 1; z <= ptZ + 1; z++)
            {
                // only check within bounds
                if (x >= 0 && x < matWidth && z >= 0 && z < matHeight)
                {
                    // only add if not current point and not obstacle
                    if ((x != ptX || z != ptZ) && !nodeMatrix[x, z].obstacle)
                    {
                        result.Add(nodeMatrix[x, z]);
                    }
                }
            }
        }
        return result;
    }
}
