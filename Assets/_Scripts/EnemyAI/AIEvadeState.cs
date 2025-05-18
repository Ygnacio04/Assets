using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEvadeState : BaseState
{
    [SerializeField] private float evadeMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;
    [SerializeField] private float evadeDuration;

    private float evadeTimer = 0f;
    private Transform currentProjectile;

    public override void Construct()
    {
        aiBehaviour.maxSpeed = evadeMaxSpeed;
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;
        evadeTimer = 0f;

        FindNearestProjectile();
    }

    public override void Transition()
    {
        if (m_enemyAIStateMotor.stateEnum == AIState.Evade) return;
        
        base.Transition();
    }

    public override void UpdateState()
    {
        evadeTimer += Time.deltaTime;

        if (evadeTimer >= evadeDuration)
        {
            m_enemyAIStateMotor.stateEnum = AIState.FollowPath;
            return;
        }

        if (currentProjectile == null)
        {
            FindNearestProjectile();

            if (currentProjectile == null)
            {
                m_enemyAIStateMotor.stateEnum = AIState.FollowPath;
            }
        }
    }


    public override void FixedUpdateState()
    {
        if (currentProjectile != null)
        {
            Rigidbody projectileRb = currentProjectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                aiBehaviour.Evade(currentProjectile.position, m_enemyAIStateMotor.rb, projectileRb);
            }
            else
            {
                aiBehaviour.Flee(currentProjectile.position, m_enemyAIStateMotor.rb);
            }
        }
        else if (m_enemyAIStateMotor.target != null)
        {
            aiBehaviour.Evade(
                m_enemyAIStateMotor.target.position,
                m_enemyAIStateMotor.rb,
                m_enemyAIStateMotor.targetRb);
        }
    }

    private void FindNearestProjectile()
    {
        Projectile[] projectiles = FindObjectsOfType<Projectile>();
        float closestDistance = float.MaxValue;

        currentProjectile = null;

        foreach (Projectile projectile in projectiles)
        {
            float distance = Vector3.Distance(transform.position, projectile.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentProjectile = projectile.transform;
            }
        }
    }
}