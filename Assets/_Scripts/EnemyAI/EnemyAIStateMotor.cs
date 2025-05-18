using UnityEngine;


public enum AIState
{
    Flee,
    Seek,
    Evade,
    Pursue,
    FollowPath,
    Wander
}

[RequireComponent(typeof(AIPatrolState), typeof(AIIdleState), typeof(AISeekState))]
[RequireComponent(typeof(AIFleeState), typeof(Rigidbody), typeof(Animator))]
[RequireComponent(typeof(AIEvadeState), typeof(AIPursueState), typeof(AIWanderState))]
[RequireComponent(typeof(AIFollowPathState), typeof(AIBehaviour))]
public class EnemyAIStateMotor : MonoBehaviour
{
    [Header("State")]
    public AIState stateEnum;
    [Header("AI Components")]
    public Animator anim;
    public Rigidbody rb;
    //public EnemyAIBehaviour enemyBehaviour;
    //public FPSPlayer player;
    [Header("Target Components")]
    public Transform target;
    public Rigidbody targetRb;
    [Header("Evade Settings")]
    public float projectileDetectionRadius = 10f;
    public LayerMask projectileLayer;
    [Header("Booleans")]
    public bool isPlayerOnSight, isIdleDone;
    public bool isEvading = false;

    private BaseState m_state;

    private void Awake()
    {
        //enemyBehaviour = GetComponent<EnemyAIBehaviour>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        m_state = GetComponent<AIFollowPathState>();
    }

    private void Start()
    {
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                targetRb = player.GetComponent<Rigidbody>();
            }
        }
        else
        {
            targetRb = target.GetComponent<Rigidbody>();
        }

        m_state.Construct();
    }

    private void Update()
    {
        //if(!GameManager.Instance.isPaused)
        UpdateMotor();
        CheckForProjectiles();
    }

    private void FixedUpdate()
    {
        m_state.FixedUpdateState();

    }

    private void UpdateMotor()
    {
        m_state.Transition();
        //m_state.UpdateState();
    }

    public void ChangeState(BaseState newState)
    {
        m_state.Destruct();
        m_state = newState;
        m_state.Construct();
    }
    private void CheckForProjectiles()
    {
        // This is now handled by EnemyRoleChanger
    }
}
