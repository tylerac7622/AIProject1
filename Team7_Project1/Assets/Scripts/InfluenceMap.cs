using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap : MonoBehaviour
{
    //the placed units of the game
    public List<GameObject> placedUnits = new List<GameObject>();
    //the size of the grid, change this value to make the grid become the new size * size
    public int size = 10;
    //influence value of each square
    int[,] grid;

    //the prefab of the grid squares
    public GameObject square;

	// Use this for initialization
	void Start ()
    {
        grid = new int[size, size];
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    //adds values to the grid based on the inputed positions and values
    //uses a direction system to assure that already checked boxes do not get checked again
    //all = 0 -- checks every direction
    //left = 1 -- checks left, down, and up
    //right = 2 -- checks right, down, and up
    //bottom = 3 -- only checks down
    //top = 4 -- only checks up
    public void ChangeGrid(int x, int y, int val, int direction)
    {
        Debug.Log(x + ", " + y + " --- " + val);
        grid[x, y] += val;
        int nextVal = 0;
        if(val > 0)
        {
            nextVal = val - 1;
        }
        if (val < 0)
        {
            nextVal = val + 1;
        }
        if (nextVal != 0)
        {
            //left
            if (x > 0 && (direction == 0 || direction == 1))
            {
                ChangeGrid(x - 1, y, nextVal, 1);
            }
            //right
            if (x < size - 1 && (direction == 0 || direction == 2))
            {
                ChangeGrid(x + 1, y, nextVal, 2);
            }
            //top
            if (y > 0 && (direction < 3 || direction == 3))
            {
                ChangeGrid(x, y - 1, nextVal, 3);
            }
            //bottom
            if (y < size - 1 && (direction < 3 || direction == 4))
            {
                ChangeGrid(x, y + 1, nextVal, 4);
            }
        }
    }

    //completely regenerates the influence grid
    public void GenerateGrid()
    {
        //reset all the grid values
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                grid[i, j] = 0;
            }
        }
        //sets the value of everything in the grid, based on the placed units
        for (int i = 0; i < placedUnits.Count; i++)
        {
            Vector2 gridPos = new Vector2(Mathf.Floor((placedUnits[i].transform.position.x - transform.position.x) / (100f/size) + (size/2f)), Mathf.Floor((placedUnits[i].transform.position.z - transform.position.z) / (100f/size) + (size/2f)));
            ChangeGrid((int)gridPos.x, (int)gridPos.y, placedUnits[i].GetComponent<InfluencerData>().influence, 0);
        }
        Debug.Log("Generating...");
        //destroys the entire previous grid
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        //loops through recreating the grid
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //creates a new square in the grid, placing it in the proper position relative to the parent (this object)
                GameObject newSquare = Instantiate(square, transform.position + new Vector3(((i - (size/2f)) * (100f/size)), 1, ((j - (size/2f)) * (100f/size))), transform.rotation, transform);
                //sets the offset value of the grid values, since they are off otherwise
                newSquare.transform.localPosition += new Vector3(20f/size, 20f/size, 0);
                //also sets the scale of each square, based on how many will fit in the grid
                newSquare.transform.localScale = new Vector3(1f/size, 1f / size, 1);
                //sets the scalar colors for coloring the grid
                float greyColor = .3f; //the base grey of the boxes
                float sameScalar = 6; //divide this by the grid value for the grids color
                float sepScalar = ((1 - greyColor) * sameScalar) / greyColor; //use this to slowly phase out the greyness of the boxes, to make high scored boxes more obvious
                //draw grey boxes if neither side has claimed it
                if (grid[i, j] == 0)
                {
                    newSquare.GetComponent<SpriteRenderer>().color = new Color(greyColor, greyColor, greyColor, .75f);
                }
                else if(grid[i, j] < 0)
                {
                    newSquare.GetComponent<SpriteRenderer>().color = new Color(greyColor + (grid[i, j] / sepScalar), greyColor - (grid[i, j] / sameScalar), greyColor + (grid[i, j] / sepScalar), .75f);
                }
                else if (grid[i, j] > 0)
                {
                    newSquare.GetComponent<SpriteRenderer>().color = new Color(greyColor + (grid[i, j] / sameScalar), greyColor - (grid[i, j] / sepScalar), greyColor - (grid[i, j] / sepScalar), .75f);
                }
                //newSquare.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 255)/255f, Random.Range(0, 255) / 255f, 0, .75f);
            }
        }
    }

    //places a unit in the list of placed units
    public void PlaceUnit(GameObject unit)
    {
        placedUnits.Add(unit);
    }
}
