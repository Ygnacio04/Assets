using UnityEngine;

[RequireComponent(typeof(EnemyAIStateMotor))]
public class EnemyRoleChanger : MonoBehaviour
{
    private EnemyAIStateMotor motor;
    private MeshRenderer meshRenderer;

    [Header("Materials")]
    [SerializeField] private Material redMat;  // Persigue
    [SerializeField] private Material blueMat; // Huye
    [SerializeField] private Material purpleMat; // Follow Path

    private float lastToggleTime = 0f;
    [SerializeField] private float toggleCooldown = 1.0f;
    [SerializeField] private float projectileDetectionRadius = 5.0f;

    private void Start()
    {
        motor = GetComponent<EnemyAIStateMotor>();
        meshRenderer = GetComponent<MeshRenderer>();

        // Estado inicial - FollowPath
        motor.stateEnum = AIState.FollowPath;
        meshRenderer.material = purpleMat;
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, projectileDetectionRadius);
        bool projectileNearby = false;

        foreach (Collider col in colliders)
        {
            if (col.GetComponent<Projectile>() != null)
            {
                projectileNearby = true;
                break;
            }
        }

        if (projectileNearby && motor.stateEnum == AIState.FollowPath && Time.time - lastToggleTime > toggleCooldown)
        {
            lastToggleTime = Time.time;
            motor.stateEnum = AIState.Evade;
            meshRenderer.material = blueMat;
        }
        else if (!projectileNearby && motor.stateEnum == AIState.Evade && Time.time - lastToggleTime > toggleCooldown)
        {
            lastToggleTime = Time.time;
            motor.stateEnum = AIState.FollowPath;
            meshRenderer.material = purpleMat;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && Time.time - lastToggleTime > toggleCooldown)
        {
            lastToggleTime = Time.time;

            if (motor.stateEnum == AIState.Seek)
            {
                motor.stateEnum = AIState.Flee;
                meshRenderer.material = blueMat;
            }
            else if (motor.stateEnum != AIState.FollowPath)
            {
                motor.stateEnum = AIState.Seek;
                meshRenderer.material = redMat;
            }
        }
    }
}