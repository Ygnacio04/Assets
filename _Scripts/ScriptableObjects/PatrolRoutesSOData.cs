using System;
using System.Collections.Generic;
using UnityEngine;


public enum PatrolRoute
{
    Route01,
    Route02,
    Route03,
    Route04
}

[CreateAssetMenu(fileName = "PatrolRoutesData", menuName = "ScriptableObjects/PatrolRoutes", order = 1)]
public class PatrolRoutesSOData : ScriptableObject
{
    public List<Transform> patrolRoute01;
    public List<Transform> patrolRoute02;
    public List<Transform> patrolRoute03;
    public List<Transform> patrolRoute04;

    public List<Transform> GetPatrolRoute(PatrolRoute patrolRoute)
    {
        return patrolRoute switch
        {
            PatrolRoute.Route01 => patrolRoute01,
            PatrolRoute.Route02 => patrolRoute02,
            PatrolRoute.Route03 => patrolRoute03,
            PatrolRoute.Route04 => patrolRoute04,
            _ => throw new ArgumentOutOfRangeException(nameof(patrolRoute), patrolRoute, null)
        };
    }
}
