using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class Rail : MonoBehaviour
{

    public List<Vector3> RailPath;
    public List<Vector3> RailRotation;
    public Vector3[] RailArray { get; set; }
    public Vector3[] RailRotationArray { get; set; }

    public Transform WaypointsHolder;

    void Start()
    {

        //Get the path

        for (int i = 0; i < WaypointsHolder.childCount; i++)
        {
            RailPath.Add(WaypointsHolder.transform.GetChild(i).position);
            RailRotation.Add(WaypointsHolder.transform.GetChild(i).eulerAngles);
        }

        RailArray = RailPath.ToArray();
        RailRotationArray = RailRotation.ToArray();

    }

    public Vector3 LinearPosition(int seg)
    {
        Vector3 p1 = RailArray[seg] + Quaternion.Euler(RailRotationArray[seg]) * Vector3.up;
        return p1;
    }

    public Vector3 LinearRotation(int seg)
    {
        Vector3 p1 = RailRotationArray[seg];
        return p1;
    }

}