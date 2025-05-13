using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFollowPathState : BaseState
{
    [SerializeField] private float followMaxSpeed;
    [SerializeField] private float steeringMaxSpeed;

    public override void Construct()
    {
        aiBehaviour.maxSpeed = followMaxSpeed;
        aiBehaviour.steeringMaxSpeed = steeringMaxSpeed;
    }

    public override void Transition()
    {
        if(m_enemyAIStateMotor.stateEnum == AIState.FollowPath) return;
        base.Transition();
    }

    public override void FixedUpdateState()
    {
        aiBehaviour.FollowPath(m_enemyAIStateMotor.rb);
    }
}
