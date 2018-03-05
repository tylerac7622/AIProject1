using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    public Camera flockerFollow;

    //list of all flocker targets
    public GameObject[] forwardTargets;
    private GameObject[] reverseTargets;
    private GameObject[] targets;
    private int targetIndex;
    public bool targetReached;
    private bool onForwardPath = true;

    List<Flock> allFlock = new List<Flock>();
    Flock.FlockState state;
    Vector3 saveTarget;
    Bottleneck toPass;
    public bool runningBottleneck = false;
    bool waitingForFlock;
    Vector2 waitingPoint;
    int passingID;
    int lastStartedID;
    int bottleneckClosestPoint; //1 or 2

    public Bottleneck ToPass
    {
        get
        {
            return toPass;
        }
    }
    public int BottleneckClosestPoint
    {
        get
        {
            return bottleneckClosestPoint;
        }
    }

    public int TargetIndex
    {
        get
        {
            return targetIndex;
        }
    }


    private void SwitchTarget()
    {
        //change camera view on pressing enter
        if (targetReached)
        {
            targetIndex++;//increment target index

            //reverses the order of the targets if on last target
            if (targetIndex >= targets.Length)
            {
                targetIndex = 1;
                if(onForwardPath)
                {
                    targets = reverseTargets;//reverses the order of the targets instead of starting over at the beginning of the path
                    onForwardPath = false;
                }
                else
                {
                    targets = forwardTargets;//once you reach the beginning of the path, turn around and go back to moving in the default direction
                    onForwardPath = true;
                }
                    
            }

            //set all other targets inactive
            for (int i = 0; i < targets.Length; i++)
            {
                targets[i].SetActive(false);
            }

            targets[targetIndex].SetActive(true);

            targetReached = false;

            for (int i = 0; i < allFlock.Count; i++)
            {
                allFlock[i].GetComponent<Flock>().TargetPosition = targets[targetIndex].transform.position;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        targetReached = false;

        Flock[] all = GameObject.FindObjectsOfType<Flock>();
        for (int i = 0; i < all.Length; i++)
        {
            allFlock.Add(all[i]);
        }

        //make a reverse array
        reverseTargets = new GameObject[forwardTargets.Length];
        for (int i = 0; i < forwardTargets.Length; i++)
        {
            reverseTargets[reverseTargets.Length - (i+1)] = forwardTargets[i];
        }

        targets = forwardTargets;

        state = Flock.FlockState.Standard;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchTarget();

        //set the flocker follow camera to follow the center of the flock
        flockerFollow.transform.position = allFlock[0].FlockCenter - allFlock[0].FlockDirection * 5;
        flockerFollow.transform.position = new Vector3(flockerFollow.transform.position.x, flockerFollow.transform.position.y + 5, flockerFollow.transform.position.z);
        flockerFollow.transform.LookAt(allFlock[0].FlockCenter + allFlock[0].transform.up);
        if(waitingForFlock)
        {
            if (FarthestReachFromPoint(waitingPoint) < 5)
            {
                WaitStart();
                waitingForFlock = false;
            }
            else
            {
                for (int i = 0; i < allFlock.Count; i++)
                {
                    float distTest = Vector2.Distance(waitingPoint, new Vector2(allFlock[i].transform.position.x, allFlock[i].transform.position.z));
                    if (distTest < 5)
                    {
                        allFlock[i].State = Flock.FlockState.Stopped;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Tyler Coppenbarger
    /// Stops the entire flock, freezing them in place and making them wait their turn
    /// </summary>
    public void StopAllFlock()
    {
        for (int i = 0; i < allFlock.Count; i++)
        {
            allFlock[i].State = Flock.FlockState.Stopped;
        }
    }

    /// <summary>
    /// Tyler Coppenbarger
    /// Restarts the entire flock and resets their targetPosition to the position they were originally heading to before bottlenecking
    /// </summary>
    public void ResumeAllFlock()
    {
        for (int i = 0; i < allFlock.Count; i++)
        {
            allFlock[i].State = Flock.FlockState.Standard;
            allFlock[i].TargetPosition = saveTarget;
        }
    }

    public float FarthestReachFromPoint(Vector2 point)
    {
        float dist = 0;
        for (int i = 0; i < allFlock.Count; i++)
        {
            float distTest = Vector2.Distance(point, new Vector2(allFlock[i].transform.position.x, allFlock[i].transform.position.z));
            if (distTest > dist)
            {
                dist = distTest;
            }
        }
        return dist;
    }

    /// <summary>
    /// Tyler Coppenbarger
    /// Starts the bottlenecking system by stopping all flockers, then starting the first flocker on the list to cross the bridge
    /// Also checks which side of the bridge the flock is on and sets the variables accordingly
    /// </summary>
    public void StartBottlenecking(Bottleneck passing, Vector2 firstCheck)
    {
        waitingPoint = firstCheck;
        runningBottleneck = true;
        passingID = 0;
        lastStartedID = 0;
        toPass = passing;

        if (FarthestReachFromPoint(firstCheck) < 5)
        {
            WaitStart();
            waitingForFlock = false;
        }
        else
        {
            waitingForFlock = true;
            for (int i = 0; i < allFlock.Count; i++)
            {
                float distTest = Vector2.Distance(firstCheck, new Vector2(allFlock[i].transform.position.x, allFlock[i].transform.position.z));
                if (distTest < 5)
                {
                    allFlock[i].State = Flock.FlockState.Stopped;
                }
            }
        }
    }

    public void WaitStart()
    {
        StopAllFlock();
        saveTarget = allFlock[0].TargetPosition;
        allFlock[passingID].State = Flock.FlockState.PassingBottleneck;
        allFlock[passingID].trackFlockPosition = new Vector2(0, 0);
        for (int i = 1; i < allFlock.Count; i++)
        {
            allFlock[i].trackFlockPosition = new Vector2(allFlock[i].transform.position.x - allFlock[0].transform.position.x, allFlock[i].transform.position.z - allFlock[0].transform.position.z);
        }
        allFlock[passingID].TargetPosition = toPass.FirstPoint;
        bottleneckClosestPoint = 1;
        if (Vector3.Distance(allFlock[0].transform.position, toPass.FirstPoint) > Vector3.Distance(allFlock[0].transform.position, toPass.SecondPoint))
        {
            bottleneckClosestPoint = 2;
            allFlock[passingID].TargetPosition = toPass.SecondPoint;
        }
    }

    /// <summary>
    /// Tyler Coppenbarger
    /// Iterates the bottlenecking through to the next flocker. If there are no more flockers, stop bottlenecking
    /// This function is run when a flocker that is bottlenecking reaches the end point of the bottleneck
    /// </summary>
    public void NextBottleneckPass()
    {
        if (passingID != 0)
        {
            allFlock[passingID].State = Flock.FlockState.Stopped;
        }
        passingID += 1;
        if (passingID >= allFlock.Count)
        {
            ResumeAllFlock();
            runningBottleneck = false;
        }
        else
        {
            allFlock[passingID].State = Flock.FlockState.PassingBottleneck;
            allFlock[passingID].TargetPosition = toPass.FirstPoint;
            if (bottleneckClosestPoint == 2)
            {
                allFlock[passingID].TargetPosition = toPass.SecondPoint;
            }
        }
    }
    public void FinishCurrentBottlenecker()
    {
        allFlock[passingID].State = Flock.FlockState.Stopped;

        passingID += 1;
        if (passingID >= allFlock.Count)
        {
            ResumeAllFlock();
            runningBottleneck = false;
        }
    }
    public void StartNextBottlenecker()
    {
        lastStartedID += 1;
        if (lastStartedID < allFlock.Count)
        {
            allFlock[lastStartedID].State = Flock.FlockState.PassingBottleneck;
            allFlock[lastStartedID].TargetPosition = toPass.FirstPoint;
            if (bottleneckClosestPoint == 2)
            {
                allFlock[lastStartedID].TargetPosition = toPass.SecondPoint;
            }
        }
    }
}
