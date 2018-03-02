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
    Bottleneck toPass;
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


        state = Flock.FlockState.Alignment;
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
            allFlock[i].State = state;
        }
    }

    public void StartBottlenecking(Bottleneck passing)
    {
        toPass = passing;
        StopAllFlock();
        state = Flock.FlockState.PassingBottleneck;
        passingID = 0;
        allFlock[passingID].State = Flock.FlockState.PassingBottleneck;
        bottleneckClosestPoint = 1;
        if (Vector3.Distance(allFlock[0].transform.position, toPass.FirstPoint) > Vector3.Distance(allFlock[0].transform.position, toPass.SecondPoint))
        {
            bottleneckClosestPoint = 2;
        }
    }

    public void NextBottleneckPass()
    {
        passingID += 1;
        if (passingID >= allFlock.Count)
        {
            ResumeAllFlock();
        }
        else
        {
            allFlock[passingID].State = Flock.FlockState.PassingBottleneck;
        }
    }
}
