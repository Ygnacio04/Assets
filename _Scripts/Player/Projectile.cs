using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    public float lifespan = 5f;

    private Rigidbody rb;
    private AIBehaviour ai;
    private Transform target;
    private Rigidbody targetRb;

    [Header("Steering Settings")]
    public float maxSpeed = 20f;
    public float steeringSpeed = 15f;
    public float detectionRadius = 30f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        ai = GetComponent<AIBehaviour>();
        if (ai == null) ai = gameObject.AddComponent<AIBehaviour>();

        ai.maxSpeed = maxSpeed;
        ai.steeringMaxSpeed = steeringSpeed;

        FindNearestEnemy();

        Destroy(gameObject, lifespan);
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = float.MaxValue;
        GameObject closestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemy;
            }
        }

        if (closestEnemy != null)
        {
            target = closestEnemy.transform;
            targetRb = closestEnemy.GetComponent<Rigidbody>();
        }
    }

    private void FixedUpdate()
    {

        if (target != null && targetRb != null)
        {
            ai.Pursue(target.position, rb, targetRb);
        }
        else
        {
            rb.velocity = transform.forward * maxSpeed;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {

            Destroy(gameObject);
        }
        else if (other.CompareTag("Environment"))
        {
            Destroy(gameObject);
        }
    }
}