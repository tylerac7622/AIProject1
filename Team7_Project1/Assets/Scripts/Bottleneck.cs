using UnityEngine;

public class Bottleneck : MonoBehaviour
{
    FlockingManager manager;
    public Vector2 firstPointPub;
    public Vector2 secondPointPub;
    Vector3 firstPoint;
    Vector3 secondPoint;
    
    public Vector3 FirstPoint
    {
        get
        {
            return firstPoint;
        }
    }
    public Vector3 SecondPoint
    {
        get
        {
            return secondPoint;
        }
    }


    /// <summary>
    /// Tyler Coppenbarger
    /// Sets both the flockingManager and sets the proper height of the side points based on the terrain
    /// </summary>
    void Start ()
    {
        manager = GameObject.Find("FlockingManager").GetComponent<FlockingManager>();

        firstPoint = new Vector3(firstPointPub.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(firstPointPub.x, 0, firstPointPub.y)), firstPointPub.y);
        secondPoint = new Vector3(secondPointPub.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(secondPointPub.x, 0, secondPointPub.y)), secondPointPub.y);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// Tyler Coppenbarger
    /// Triggers the flock manager to start bottlenecking if a flock enters the collider
    /// Also sets any mover to be on a bridge (the only bottleneck), which sets the objects proper height
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //if flocking character, stop all flockers
        if (other.GetComponent<Flock>() != null)
        {
            other.GetComponent<BaseMovement>().OverBridge = true;
            if (!manager.runningBottleneck)
            {
                manager.StartBottlenecking(this);
            }
        }
    }
    /// <summary>
    /// Tyler Coppenbarger
    /// Sets any mover to not be on a bridge anymore, doing the inverse of OnTriggerEnter
    /// </summary>
    private void OnTriggerExit(Collider other)
    {
        //if flocking character, stop all flockers
        if (other.GetComponent<Flock>() != null)
        {
            other.GetComponent<BaseMovement>().OverBridge = false;
        }
    }
}
