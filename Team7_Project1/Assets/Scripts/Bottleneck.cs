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

    // Use this for initialization
    void Start ()
    {
        manager = GameObject.Find("FlockingManager").GetComponent<FlockingManager>();

        firstPoint = new Vector3(firstPointPub.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(firstPointPub.x, 0, firstPointPub.y)), firstPointPub.y);
        secondPoint = new Vector3(secondPointPub.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(secondPointPub.x, 0, secondPointPub.y)), secondPointPub.y);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTERED");
        //if flocking character, stop all flockers
        if (other.GetComponent<Flock>() != null && !manager.runningBottleneck)
        {
            manager.StartBottlenecking(this);
        }
    }
}
