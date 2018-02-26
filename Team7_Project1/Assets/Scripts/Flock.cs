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

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

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
                //move to toPass.FirstPoint
                if (Vector3.Distance(transform.position, manager.ToPass.FirstPoint) < 1)
                {
                    progress = SingleProgress.ToSecondPoint;
                }
            }
            if (manager.BottleneckClosestPoint == 2)
            {
                //move to toPass.SecondPoint
                if (Vector3.Distance(transform.position, manager.ToPass.SecondPoint) < 1)
                {
                    progress = SingleProgress.ToSecondPoint;
                }
            }
        }
        if (progress == SingleProgress.ToSecondPoint)
        {
            if (manager.BottleneckClosestPoint == 1)
            {
                //move to toPass.SecondPoint
                if (Vector3.Distance(transform.position, manager.ToPass.SecondPoint) < 1)
                {
                    //progress = SingleProgress.ToFinalPoint;
                    progress = SingleProgress.ToFirstPoint;
                    manager.NextBottleneckPass();
                    state = FlockState.Stopped;
                }
            }
            if (manager.BottleneckClosestPoint == 2)
            {
                //move to toPass.FirstPoint
                if (Vector3.Distance(transform.position, manager.ToPass.FirstPoint) < 1)
                {
                    //progress = SingleProgress.ToFinalPoint;
                    progress = SingleProgress.ToFirstPoint;
                    manager.NextBottleneckPass();
                    state = FlockState.Stopped;
                }
            }
        }
    }
}
