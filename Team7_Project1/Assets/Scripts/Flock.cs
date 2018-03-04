﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : BaseMovement
{
    FlockingManager manager;
    public enum FlockState
    {
        Standard,
        PassingBottleneck,
        Stopped,
    }
    public FlockState state = FlockState.Standard;
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

    public float tooClose;//how close is too close for separation

    //weights for the 3 separate parts of flocking
    public float sepWgt;
    public float alignWgt;
    public float cohWgt;
    public float seekWgt;//importance of seeking the target that moves the flock around//10

    //whether or not the parts of flocking are turned on
    private bool separationOn;
    private bool alingmentOn;
    private bool cohesionOn;

    public float maxForce; //how quickly they can turn/accelerate
    private Vector3 ultForce;//the vector3 that determines acceleration

    List<GameObject> flock;
    public float flockRadius;//how far away a flocker can be from this flocker to be considered part of its flock
    private Vector3 flockCenter;//point in the middle of this flocker's flock
    private Vector3 flockDirection;//direction of this flocker's flock


    private Vector3 targetPosition;

    //props
    public Vector3 TargetPosition
    {
        get
        {
            return targetPosition;
        }
        set { targetPosition = value; }
    }

    // Use this for initialization
    public override void Start () {
        manager = GameObject.Find("FlockingManager").GetComponent<FlockingManager>();

        targetPosition = GameObject.Find("Target").transform.position;

        //set the flocking forces to be on
        separationOn = true;
        alingmentOn = true;
        cohesionOn = true;

        state = FlockState.Standard;

        base.Start();
	}
	
	// Update is called once per frame
	public override void Update()
    {
        if (state == FlockState.PassingBottleneck)
        {
            PassBottleneck();
        }
        else if(state == FlockState.Standard)
        {
            //toggle the different forces in flocking
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                cohesionOn = !cohesionOn;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                alingmentOn = !alingmentOn;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                separationOn = !separationOn;
            }

            DetectFlock();

            //update the target if you reached current target
            if (Vector3.Distance(transform.position, targetPosition) < 1)
            {
                manager.targetReached = true;
            }
        }

        base.Update();

        // DEBUG
        //Debug.Log("progress: " + progress);
    }

    protected override void CalcSteeringForces()
    {
        //reset ultForce
        ultForce = Vector3.zero;
        //Debug.Log(state);
        if (state == FlockState.Standard)
        {
            //apply all flocking forces as default, but allow toggling of parts by keyboard input
            if (cohesionOn == true)
            {
                ultForce += cohWgt * Cohesion();
            }
            if (alingmentOn == true)
            {
                ultForce += alignWgt * Alignment();
            }
            if (separationOn == true)
            {
                ultForce += sepWgt * Separation();
            }
        }
        //apply the seek force that moves the flock toward
        ultForce += seekWgt * Seek(targetPosition);
        if (state == FlockState.Stopped)
        {
            velocity = Vector3.zero;
            acceleration = Vector3.zero;
            ultForce = Vector3.zero;
        }
        else
        {
            //limit by max force
            Vector3.ClampMagnitude(ultForce, maxForce);
            //Debug.Log(state + " - " + ultForce);
            //apply ultimate force
            ApplyForce(ultForce);
        }
    }

    /// <summary>
    /// Katarina Weis
    /// Will search through all flockers in the scene 
    /// Determine which ones are within its radius of awareness
    /// Consider those flockers its flock
    /// Set the center and direction of the flock appropriately
    /// </summary>
    private void DetectFlock()
    {
        flock = new List<GameObject>();
        GameObject[] all = GameObject.FindGameObjectsWithTag("flocker");//get all flockers

        //reset center and direction
        flockCenter = Vector3.zero;
        flockDirection = Vector3.zero;

        for (int i = 0; i < all.Length; i++)
        {
            //add flockers within the radius to this flockers flock
            if (Vector3.Distance(all[i].transform.position, transform.position) < flockRadius)
            {
                flock.Add(all[i]);
            }
        }

        //determine the center of the flock and the average direction for use in flocking algorithm
        for (int i = 0; i < flock.Count; i++)
        {
            flockCenter += flock[i].transform.position;
            flockDirection += flock[i].transform.forward;
        }

        flockCenter = flockCenter / flock.Count;
        flockDirection.Normalize();
    }

    /// <summary>
    /// Katarina Weis
    /// Push the flocker slightly away from the other flockers in the flock
    /// Flee the other flockers in the flock
    /// Divide by the distance between them so the force is bigger the closer they are
    /// <returns>Separation force</returns>
    /// </summary>
    public Vector3 Separation()
    {
        Vector3 sep = Vector3.zero;
        //calc separation for every flocker that is too close
        foreach (GameObject f in flock)
        {
            //ensure that it doesn't check itself
            if (f.transform.position == transform.position && f.transform.forward == transform.forward)
            {
                break;
            }
            //create a vector from character to the center of the obstacle
            Vector3 vecToCenter = f.transform.position - position;
            //get diff as mag squared of that vector
            float distance = vecToCenter.sqrMagnitude;
            if (distance < Mathf.Pow(tooClose, 2))
            {
                //vary strength to reflect proximity
                sep += Flee(f.transform.position) * (tooClose / distance);
            }
        }
        //return the sum of all the separation vectors
        return sep;
    }

    /// <summary>
    /// Katarina Weis
    /// Push this flocker closer to the center of the flock
    /// Causes bunching up
    /// Seek the center of the flock
    /// <returns>Cohesion force</returns>
    /// </summary>
    public Vector3 Cohesion()
    {
        return Seek(flockCenter);
    }

    /// <summary>
    /// Katarina Weis
    /// Direct the flocker in the direction of the rest of the flock
    /// Lines up the direction of the flocker with the average of the flock
    /// <returns>Alignment force</returns>
    /// </summary>
    public Vector3 Alignment()
    {
        return (flockDirection * maxSpeed) - velocity;
    }

    public void PassBottleneck()
    {
        Debug.Log(progress + " - " + Vector3.Distance(transform.position, targetPosition));
        if (progress == SingleProgress.ToFirstPoint)
        {
            if (manager.BottleneckClosestPoint == 1)
            {
                if (Vector3.Distance(transform.position, targetPosition) < .5f)
                {
                    progress = SingleProgress.ToSecondPoint;
                    targetPosition = manager.ToPass.SecondPoint;
                }
            }
            if (manager.BottleneckClosestPoint == 2)
            {
                if (Vector3.Distance(transform.position, targetPosition) < .5f)
                {
                    progress = SingleProgress.ToSecondPoint;
                    targetPosition = manager.ToPass.FirstPoint;
                }
            }
        }
        if (progress == SingleProgress.ToSecondPoint)
        {
            if (Vector3.Distance(transform.position, targetPosition) < .5f)
            {
                progress = SingleProgress.ToFinalPoint;
                targetPosition = transform.position + (transform.forward * Random.Range(.5f, 5)) + (transform.right * Random.Range(-2.5f, 2.5f));
                targetPosition.y = GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(targetPosition.x, 0, targetPosition.z));
            }
        }
        if (progress == SingleProgress.ToFinalPoint)
        {
            if (Vector3.Distance(transform.position, targetPosition) < .5f)
            {
                progress = SingleProgress.ToFirstPoint;
                manager.NextBottleneckPass();
            }
        }



        /*if (progress == SingleProgress.ToFirstPoint)
        {
            if (manager.BottleneckClosestPoint == 1)
            {
                //transform.LookAt(manager.ToPass.FirstPoint);
                //transform.position = Vector3.MoveTowards(transform.position, manager.ToPass.FirstPoint, Time.deltaTime * movementSpeed);
                //transform.position = new Vector3(transform.position.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)), transform.position.z);
                if (Vector3.Distance(transform.position, manager.ToPass.FirstPoint) < .2f)
                {
                    progress = SingleProgress.ToSecondPoint;
                    targetPosition = transform.position + (transform.forward * Random.Range(.5f, 5)) + (transform.right * Random.Range(-2.5f, 2.5f));
                }
            }
            if (manager.BottleneckClosestPoint == 2)
            {
                //transform.LookAt(manager.ToPass.SecondPoint);
                //transform.position = Vector3.MoveTowards(transform.position, manager.ToPass.SecondPoint, Time.deltaTime * movementSpeed);
                //transform.position = new Vector3(transform.position.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)), transform.position.z);
                if (Vector3.Distance(transform.position, manager.ToPass.SecondPoint) < .2f)
                {
                    progress = SingleProgress.ToSecondPoint;
                    targetPosition = transform.position + (transform.forward * Random.Range(.5f, 5)) + (transform.right * Random.Range(-2.5f, 2.5f));
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
                    targetPosition = transform.position + (transform.forward * Random.Range(.5f, 5)) + (transform.right * Random.Range(-2.5f, 2.5f));
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
                    targetPosition = transform.position + (transform.forward * Random.Range(.5f, 5)) + (transform.right * Random.Range(-2.5f, 2.5f));
                }
            }
        }
        if (progress == SingleProgress.ToFinalPoint)
        {
            //move to toPass.FirstPoint
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * movementSpeed);
            transform.position = new Vector3(transform.position.x, GameObject.Find("Terrain").GetComponent<Terrain>().SampleHeight(new Vector3(transform.position.x, 0, transform.position.z)), transform.position.z);
            if (Vector3.Distance(transform.position, targetPosition) < .2f)
            {
                //progress = SingleProgress.ToFinalPoint;
                progress = SingleProgress.ToFirstPoint;
                manager.NextBottleneckPass();
                state = FlockState.Stopped;
            }
        }*/
    }
}
