using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWanderState : BaseState
{
    [SerializeField] private float wanderMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;

    public override void Construct()
    {
        aiBehaviour.maxSpeed = wanderMaxSpeed;
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;
    }

    public override void Transition()
    {
        if(m_enemyAIStateMotor.stateEnum == AIState.Wander) return;
        base.Transition();
    }

    public override void FixedUpdateState()
    {
        aiBehaviour.Wander(m_enemyAIStateMotor.rb);
    }
}
