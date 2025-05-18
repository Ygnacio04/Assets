using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIBehaviour : MonoBehaviour
{
    [Header("Follow Path Settings")]
    [SerializeField] private PatrolRoutesSOData patrolRoutes;
    [SerializeField] private PatrolRoute patrolRoute;

    [Header("Steering")]
    public float maxSpeed;
    public float steeringMaxSpeed;
    public float stoppingDistance;

    [Header("Display Settings")]
    [SerializeField] private bool areVectorsOnDisplay;

    private Transform[] _pathPoints;
    private int _currentRouteIndex;
    private float _sqrRemainingDistance;
    private bool _pathPending;

    private void Start()
    {
        if (patrolRoutes != null && patrolRoute != null)
        {
            SetNewRoute(patrolRoute);
        }
    }

    #region Behaviours

    public void Seek(Vector3 target, Rigidbody rb)
    {
        var targetDirection = CalculateTargetDirection(target);

        var steeringDirection = CalculateSteeringDirection(targetDirection, rb.velocity);

        var finalDirection = CalculateFinalDirection(steeringDirection, rb.velocity);

        DisplayVectors(rb.velocity, targetDirection, steeringDirection);

        rb.velocity = CalculateFinalVelocity(finalDirection) * Arrive(target);
    }

    public void Flee(Vector3 target, Rigidbody rb)
    {
        var targetDirection = -CalculateTargetDirection(target);

        var steeringDirection = CalculateSteeringDirection(targetDirection, rb.velocity);

        var finalDirection = CalculateFinalDirection(steeringDirection, rb.velocity);

        DisplayVectors(rb.velocity, targetDirection, steeringDirection);

        rb.velocity = CalculateFinalVelocity(finalDirection);
    }

    private float Arrive(Vector3 target)
    {
        var sqrDistance = (target - transform.position).sqrMagnitude;
        if (sqrDistance > Mathf.Pow(stoppingDistance, 2))
            return 1;

        _pathPending = false;

        return (sqrDistance / Mathf.Pow(stoppingDistance, 2));
    }

    public void Pursue(Vector3 target, Rigidbody rb, Rigidbody targetRb)
    {
        if (rb.velocity.sqrMagnitude < 0.0001f || Vector3.Dot(targetRb.velocity.normalized, CalculateTargetDirection(target).normalized) < -0.8f)
            Seek(target, rb);
        else
        {
            var currentSqrSpeed = rb.velocity.sqrMagnitude;
            var sqrDistanceToTarget = CalculateTargetDirection(target).sqrMagnitude;
            var prediction = CalculatePrediction(sqrDistanceToTarget, currentSqrSpeed);

            var explicitTarget = CalculatePredictionExplicitTarget(target, targetRb, prediction);
            Seek(explicitTarget, rb);
        }
    }

    public void Evade(Vector3 target, Rigidbody rb, Rigidbody targetRb)
    {
        if (rb.velocity.sqrMagnitude < 0.0001f || Vector3.Dot(targetRb.velocity.normalized, CalculateTargetDirection(target).normalized) < -0.8f)
            Flee(target, rb);
        else
        {
            var currentSqrSpeed = rb.velocity.sqrMagnitude;
            var sqrDistanceToTarget = CalculateTargetDirection(target).sqrMagnitude;
            var prediction = CalculatePrediction(sqrDistanceToTarget, currentSqrSpeed);

            var explicitTarget = CalculatePredictionExplicitTarget(target, targetRb, prediction);
            Flee(explicitTarget, rb);
        }
    }

    public void Wander(Rigidbody rb)
    {
        var displacement = CalculateWanderDisplacement(rb.velocity);

        var wanderDirection = CalculateWanderDirection(rb.velocity.normalized, displacement);

        var steeringDirection = CalculateSteeringDirection(wanderDirection, rb.velocity);

        var finalDirection = CalculateFinalDirection(steeringDirection, rb.velocity);

        DisplayVectors(rb.velocity, wanderDirection, steeringDirection);

        rb.velocity = CalculateFinalVelocity(finalDirection);
    }

    public void FollowPath(Rigidbody rb)
    {
        var target = _pathPoints[_currentRouteIndex].position;
        if (CheckNextPoint())
        {
            target = SetNextRoutePoint();
        }
        Seek(target, rb);
    }
    #endregion

    #region Path Functions

    private void InitializePatrolPoints()
    {
        _pathPoints = patrolRoutes.GetPatrolRoute(patrolRoute).ToArray();
    }

    private void SetNewRoute(PatrolRoute newRoute)
    {
        patrolRoute = newRoute;
        _pathPoints = patrolRoutes.GetPatrolRoute(patrolRoute).ToArray();
        _currentRouteIndex = 0;
    }

    private Vector3 SetNextRoutePoint()
    {
        _currentRouteIndex = ++_currentRouteIndex % _pathPoints.Length;
        return _pathPoints.Length == 0
            ? Vector3.zero
            : SetTarget(_pathPoints[_currentRouteIndex].position);
    }

    private Vector3 SetTarget(Vector3 target)
    {
        _pathPending = true;
        return target;
    }

    private bool CheckNextPoint()
    {
        return !_pathPending && _sqrRemainingDistance <= Mathf.Pow(stoppingDistance, 2);
    }

    #endregion

    #region Calculations

    private Vector3 CalculateTargetDirection(Vector3 target)
    {
        return (target - transform.position).normalized * (maxSpeed * Time.fixedDeltaTime);
    }

    private Vector3 CalculateSteeringDirection(Vector3 targetDirection, Vector3 currentVelocity)
    {
        var steeringDirection = targetDirection - currentVelocity;

        return steeringDirection.sqrMagnitude > Mathf.Pow(steeringMaxSpeed, 2)
            ? steeringDirection.normalized * steeringMaxSpeed
            : steeringDirection;
    }

    private float CalculatePrediction(float sqrDistanceToTarget, float currentSqrSpeed)
    {
        return sqrDistanceToTarget / currentSqrSpeed;
    }

    private Vector3 CalculatePredictionExplicitTarget(Vector3 target, Rigidbody targetRb, float prediction)
    {
        return target + targetRb.velocity * prediction;
    }

    private Vector3 CalculateWanderDisplacement(Vector3 rbVelocity)
    {
        var randomPoint = Random.insideUnitCircle;

        return Quaternion.LookRotation(rbVelocity) * new Vector3(randomPoint.x, 0, randomPoint.y);
    }

    private Vector3 CalculateWanderDirection(Vector3 circleCenter, Vector3 displacement)
    {
        return (circleCenter + displacement).normalized * (maxSpeed * Time.fixedDeltaTime);
    }

    private Vector3 CalculateFinalDirection(Vector3 steeringDirection, Vector3 currentVelocity)
    {
        return currentVelocity + steeringDirection;
    }

    private Vector3 CalculateFinalVelocity(Vector3 finalDirection)
    {
        return finalDirection.sqrMagnitude > Mathf.Pow(maxSpeed, 2)
            ? finalDirection.normalized * maxSpeed
            : finalDirection;
    }

    private void DisplayVectors(Vector3 currentVelocity, Vector3 targetDirection, Vector3 steeringDirection)
    {
        if (!areVectorsOnDisplay) return;

        Debug.DrawRay(transform.position, currentVelocity, Color.blue);
        Debug.DrawRay(transform.position, targetDirection, Color.green);
        Debug.DrawRay(transform.position + currentVelocity, steeringDirection * 10, Color.red);
    }

    #endregion
}
