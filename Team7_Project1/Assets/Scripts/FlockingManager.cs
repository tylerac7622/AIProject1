using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
    List<Flock> allFlock = new List<Flock>();
    enum FlockState
    {
        Separation,
        Cohesion,
        Alignment,
        PassingBottleneck,
    }
    FlockState state = FlockState.Alignment;
    Bottleneck toPass;
    int passingID;
    int bottleneckClosestPoint; //1 or 2
    enum SingleProgress
    {
        ToFirstPoint,
        ToSecondPoint,
        ToFinalPoint,
    }
    SingleProgress progress = SingleProgress.ToFirstPoint;

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

    // Use this for initialization
    void Start()
    {
        Flock[] all = GameObject.FindObjectsOfType<Flock>();
        for (int i = 0; i < all.Length; i++)
        {
            allFlock.Add(all[i]);
        }
    }

    // Update is called once per frame
    void Update() {

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
    public void StopAllFlock()
    {
    }

    public void ResumeAllFlock()
    {

    }

    public void StartBottlenecking(Bottleneck passing)
    {
        toPass = passing;
        StopAllFlock();
        state = FlockState.PassingBottleneck;
        passingID = 0;
        progress = SingleProgress.ToFirstPoint;
    }

    public void NextBottleneckPass()
    {
        passingID += 1;
        allFlock[passingID].State = Flock.FlockState.PassingBottleneck;
    }

    public void PassBottleneck()
    {
        if (progress == SingleProgress.ToFirstPoint)
        {
            if (bottleneckClosestPoint == 1)
            {
                //move allFlock[passingID] to toPass.FirstPoint
                if(Vector3.Distance(allFlock[passingID].transform.position, toPass.FirstPoint) < 1)
                {
                    progress = SingleProgress.ToSecondPoint;
                }
            }
            if (bottleneckClosestPoint == 2)
            {
                //move allFlock[passingID] to toPass.SecondPoint
                if (Vector3.Distance(allFlock[passingID].transform.position, toPass.SecondPoint) < 1)
                {
                    progress = SingleProgress.ToSecondPoint;
                }
            }
        }
        if (progress == SingleProgress.ToSecondPoint)
        {
            if (bottleneckClosestPoint == 1)
            {
                //move allFlock[passingID] to toPass.SecondPoint
                if (Vector3.Distance(allFlock[passingID].transform.position, toPass.SecondPoint) < 1)
                {
                    //progress = SingleProgress.ToFinalPoint;
                    progress = SingleProgress.ToFirstPoint;
                    passingID += 1;
                    if(passingID >= allFlock.Count)
                    {
                        ResumeAllFlock();
                    }
                }
            }
            if (bottleneckClosestPoint == 2)
            {
                //move allFlock[passingID] to toPass.FirstPoint
                if (Vector3.Distance(allFlock[passingID].transform.position, toPass.FirstPoint) < 1)
                {
                    //progress = SingleProgress.ToFinalPoint;
                    progress = SingleProgress.ToFirstPoint;
                    passingID += 1;
                    if (passingID >= allFlock.Count)
                    {
                        ResumeAllFlock();
                    }
                }
            }
        }
        if (progress == SingleProgress.ToFinalPoint)
        {

        }
        //for each flocker, move to closest bottleneck point
        //when close to point move on
        //then move to next bottleneck point
        //when close to point move on
        //after that, move to nearby position to make room for remaining flockers
    }
}
