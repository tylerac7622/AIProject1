using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    FlockingManager manager;
    public enum FlockState
    {
        Separation,
        Cohesion,
        Alignment,
        PassingBottleneck,
        Stopped,
    }
    FlockState state = FlockState.Alignment;
    enum SingleProgress
    {
        ToFirstPoint,
        ToSecondPoint,
        ToFinalPoint,
    }
    SingleProgress progress = SingleProgress.ToFirstPoint;

    public FlockState State
    {
        set
        {
            state = value;
        }
    }

    float movementSpeed = 5;

    Vector3 targetPostion;

    // Use this for initialization
    void Start () {
        manager = GameObject.Find("FlockingManager").GetComponent<FlockingManager>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if(state == FlockState.PassingBottleneck)
        {
            PassBottleneck();
        }
    }

    public void Separation()
    {
    }

    public void Cohesion()
    {
    }

    public void Alignment()
    {
    }

    public void PassBottleneck()
    {
        if (progress == SingleProgress.ToFirstPoint)
        {
            if (manager.BottleneckClosestPoint == 1)
            {
                transform.LookAt(manager.ToPass.FirstPoint);
                transform.position = Vector3.MoveTowards(transform.position, manager.ToPass.FirstPoint, Time.deltaTime * movementSpeed);
                transform.position = new Vector3(transform.position.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)), transform.position.z);
                if (Vector3.Distance(transform.position, manager.ToPass.FirstPoint) < .2f)
                {
                    progress = SingleProgress.ToSecondPoint;
                }
            }
            if (manager.BottleneckClosestPoint == 2)
            {
                transform.LookAt(manager.ToPass.SecondPoint);
                transform.position = Vector3.MoveTowards(transform.position, manager.ToPass.SecondPoint, Time.deltaTime * movementSpeed);
                transform.position = new Vector3(transform.position.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)), transform.position.z);
                if (Vector3.Distance(transform.position, manager.ToPass.SecondPoint) < .2f)
                {
                    progress = SingleProgress.ToSecondPoint;
                }
            }
        }
        if (progress == SingleProgress.ToSecondPoint)
        {
            if (manager.BottleneckClosestPoint == 1)
            {
                transform.LookAt(manager.ToPass.SecondPoint);
                transform.position = Vector3.MoveTowards(transform.position, manager.ToPass.SecondPoint, Time.deltaTime * movementSpeed);
                //stay on bridge, not terrain
                transform.position = new Vector3(transform.position.x, 50, transform.position.z);
                if (Vector3.Distance(transform.position, manager.ToPass.SecondPoint) < .2f)
                {
                    progress = SingleProgress.ToFinalPoint;
                    targetPostion = transform.position + (transform.forward * Random.Range(.5f, 5)) + (transform.right * Random.Range(-2.5f, 2.5f));
                }
            }
            if (manager.BottleneckClosestPoint == 2)
            {
                transform.LookAt(manager.ToPass.FirstPoint);
                transform.position = Vector3.MoveTowards(transform.position, manager.ToPass.FirstPoint, Time.deltaTime * movementSpeed);
                //stay on bridge, not terrain
                transform.position = new Vector3(transform.position.x, 50, transform.position.z);
                if (Vector3.Distance(transform.position, manager.ToPass.FirstPoint) < .2f)
                {
                    progress = SingleProgress.ToFinalPoint;
                    targetPostion = transform.position + (transform.forward * Random.Range(.5f, 5)) + (transform.right * Random.Range(-2.5f, 2.5f));
                }
            }
        }
        if (progress == SingleProgress.ToFinalPoint)
        {
            //move to toPass.FirstPoint
            transform.position = Vector3.MoveTowards(transform.position, targetPostion, Time.deltaTime * movementSpeed);
            transform.position = new Vector3(transform.position.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)), transform.position.z);
            if (Vector3.Distance(transform.position, targetPostion) < .2f)
            {
                //progress = SingleProgress.ToFinalPoint;
                progress = SingleProgress.ToFirstPoint;
                manager.NextBottleneckPass();
                state = FlockState.Stopped;
            }
        }
    }
}
