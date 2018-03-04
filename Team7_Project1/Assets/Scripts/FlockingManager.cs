using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    //list of all flocker targets
    public List<GameObject> targets;
    private int targetIndex;
    public bool targetReached;

    List<Flock> allFlock = new List<Flock>();
    Flock.FlockState state;
    Vector3 saveTarget;
    Bottleneck toPass;
    public bool runningBottleneck = false;
    int passingID;
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
            targetIndex++;//increment cam index

            //reset to the beginning if on last camera
            if (targetIndex >= targets.Count)
            {
                targetIndex = 0;
            }

            //set all other cameras inactive
            for (int i = 0; i < targets.Count; i++)
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
        //StartBottlenecking(GameObject.Find("Bridge (1)").transform.GetChild(0).GetComponent<Bottleneck>());


        state = Flock.FlockState.Standard;
    }

    // Update is called once per frame
    void Update()
    {
        SwitchTarget();
    }

    public void StopAllFlock()
    {
        for (int i = 0; i < allFlock.Count; i++)
        {
            allFlock[i].State = Flock.FlockState.Stopped;
        }
    }

    public void ResumeAllFlock()
    {
        for (int i = 0; i < allFlock.Count; i++)
        {
            allFlock[i].State = Flock.FlockState.Standard;
            allFlock[i].TargetPosition = saveTarget;
        }
    }

    public void StartBottlenecking(Bottleneck passing)
    {
        toPass = passing;
        StopAllFlock();
        runningBottleneck = true;
        state = Flock.FlockState.PassingBottleneck;
        saveTarget = allFlock[0].TargetPosition;
        passingID = 0;
        allFlock[passingID].State = Flock.FlockState.PassingBottleneck;
        allFlock[passingID].TargetPosition = toPass.FirstPoint;
        bottleneckClosestPoint = 1;
        if (Vector3.Distance(allFlock[0].transform.position, toPass.FirstPoint) > Vector3.Distance(allFlock[0].transform.position, toPass.SecondPoint))
        {
            bottleneckClosestPoint = 2;
            allFlock[passingID].TargetPosition = toPass.SecondPoint;
        }
    }

    public void NextBottleneckPass()
    {
        allFlock[passingID].State = Flock.FlockState.Stopped;
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
}
