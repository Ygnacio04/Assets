using UnityEngine;

public class FuzzyLogicAI : MonoBehaviour
{
    [Header("Distance Variables")]
    public float distanceToPlayer = 0f;
    [Range(0, 50)] public float maxDistance = 20f;

    [Header("Distance Membership Functions")]
    public AnimationCurve nearDistance;
    public AnimationCurve mediumDistance;
    public AnimationCurve farDistance;

    [Header("Output Values")]
    public float attackValue;
    public float patrolValue;
    public float fleeValue;

    private EnemyAIStateMotor aiStateMotor;
    private Transform player;

    private void Start()
    {
        aiStateMotor = GetComponent<EnemyAIStateMotor>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        InitializeMembershipFunctions();
    }

    private void Update()
    {
        if (player != null)
        {
            UpdateDistanceToPlayer();
            EvaluateFuzzyState();
            UpdateEnemyBehavior();
        }
    }

    private void UpdateDistanceToPlayer()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);
    }

    private void InitializeMembershipFunctions()
    {
        // Initialize distance membership functions if not set in inspector
        if (nearDistance == null || nearDistance.keys.Length == 0)
        {
            nearDistance = new AnimationCurve(
                new Keyframe(0, 1),
                new Keyframe(5, 0)
            );
        }

        if (mediumDistance == null || mediumDistance.keys.Length == 0)
        {
            mediumDistance = new AnimationCurve(
                new Keyframe(3, 0),
                new Keyframe(10, 1),
                new Keyframe(15, 0)
            );
        }

        if (farDistance == null || farDistance.keys.Length == 0)
        {
            farDistance = new AnimationCurve(
                new Keyframe(12, 0),
                new Keyframe(20, 1)
            );
        }
    }

    private void EvaluateFuzzyState()
    {
        // Calculate membership values for distance
        float nearDistanceValue = nearDistance.Evaluate(distanceToPlayer);
        float mediumDistanceValue = mediumDistance.Evaluate(distanceToPlayer);
        float farDistanceValue = farDistance.Evaluate(distanceToPlayer);

        // Fuzzy rules based solely on distance

        // If player is near, attack
        attackValue = nearDistanceValue;

        // If player is at medium distance, patrol/follow
        patrolValue = mediumDistanceValue;

        // If player is far, patrol/wander
        fleeValue = farDistanceValue;
    }

    private void UpdateEnemyBehavior()
    {
        // Determine the winning behavior based on the highest value
        if (attackValue > patrolValue && attackValue > fleeValue)
        {
            if (aiStateMotor.stateEnum != AIState.Pursue)
                aiStateMotor.stateEnum = AIState.Pursue;
        }
        else if (patrolValue > attackValue && patrolValue > fleeValue)
        {
            if (aiStateMotor.stateEnum != AIState.FollowPath)
                aiStateMotor.stateEnum = AIState.FollowPath;
        }
        else
        {
            if (aiStateMotor.stateEnum != AIState.Wander)
                aiStateMotor.stateEnum = AIState.Wander;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw spheres to visualize distance ranges
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 5); // Near range

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 10); // Medium range

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 20); // Far range
    }

    // Optional debugging information
    void OnGUI()
    {
        if (Vector3.Distance(Camera.main.transform.position, transform.position) < 15f) // Solo mostrar si estás cerca
        {
            Vector3 screenPos = Camera.main.WorldToScreenPoint(transform.position + Vector3.up * 2);

            if (screenPos.z > 0) // Solo mostrar si está delante de la cámara
            {
                GUILayout.BeginArea(new Rect(screenPos.x - 100, Screen.height - screenPos.y, 200, 80));
                GUILayout.Box($"Distance: {distanceToPlayer:F1}m");
                GUILayout.Box($"Attack: {attackValue:F2} | Patrol: {patrolValue:F2} | Flee: {fleeValue:F2}");

                string currentState = "Unknown";
                if (attackValue > patrolValue && attackValue > fleeValue)
                    currentState = "Attack";
                else if (patrolValue > attackValue && patrolValue > fleeValue)
                    currentState = "Patrol";
                else
                    currentState = "Flee";

                GUILayout.Box($"State: {currentState}");
                GUILayout.EndArea();
            }
        }
    }
}