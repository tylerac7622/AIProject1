using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockingManager : MonoBehaviour
{
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

    // Use this for initialization
    void Start()
    {
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
