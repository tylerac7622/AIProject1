using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMap : MonoBehaviour
{
    List<GameObject> placedUnits;
    int[,] grid = new int[10, 10];
    public GameObject square;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    public void ChangeGrid(int x, int y, int val)
    {

    }

    public void GenerateGrid()
    {
        //placedUnits.Add(4);
        /*for(int i = 0; i < placedUnits.Count; i++)
        {

        }*/
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                grid[i, j] = 2;
            }
        }
        Debug.Log("Generating...");
        for(int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject newSquare = Instantiate(square, transform.position + new Vector3(((i - 5) * 10), 1, ((j - 5) * 10)), transform.rotation, transform);
                newSquare.transform.localPosition += new Vector3(2, 2, 0);
                newSquare.transform.localScale = new Vector3(.1f, .1f, 1);
                //newSquare.GetComponent<SpriteRenderer>().color = new Color(Mathf.Max(grid[i, j], 0), Mathf.Min(grid[i, j], 0), 0, .75f);
                newSquare.GetComponent<SpriteRenderer>().color = new Color(Random.Range(0, 255)/255f, Random.Range(0, 255) / 255f, 0, .75f);
            }
        }
    }

    public void PlaceUnit(GameObject unit)
    {
        //placedUnits.Add(unit);
    }
}
