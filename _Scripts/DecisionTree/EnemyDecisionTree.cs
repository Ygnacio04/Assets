using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDecisionTree : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyAIStateMotor aiStateMotor;

    [Header("Detection")]
    [SerializeField] private float playerDetectionRadius = 10f;
    [SerializeField] private float itemDetectionRadius = 15f;
    [SerializeField] private LayerMask playerLayer;

    [Header("Decision Settings")]
    [SerializeField] private float decisionUpdateInterval = 0.5f;

    // Variables para contar entidades
    private int nearbyPlayerCount = 0;
    private int nearbyEnemyCount = 0;

    private float lastDecisionTime;

    private void Start()
    {
        if (aiStateMotor == null)
            aiStateMotor = GetComponent<EnemyAIStateMotor>();

        lastDecisionTime = 0f;
    }

    private void Update()
    {
        // Actualizar la decisi�n peri�dicamente para no sobrecargar
        if (Time.time - lastDecisionTime >= decisionUpdateInterval)
        {
            MakeDecision();
            lastDecisionTime = Time.time;
        }
    }

    private void MakeDecision()
    {
        // 1. Comprobar si el jugador est� cerca
        Collider[] playerColliders = Physics.OverlapSphere(transform.position, playerDetectionRadius, playerLayer);
        nearbyPlayerCount = playerColliders.Length;

        if (nearbyPlayerCount > 0)
        {
            // Jugador detectado - �Link cerca? = S�

            // Contar enemigos cercanos al jugador para determinar si somos m�s fuertes
            CountNearbyEnemies(playerColliders[0].transform.position);


            if (nearbyEnemyCount > nearbyPlayerCount)
            {
                // Tenemos ventaja num�rica - Atacar
                aiStateMotor.stateEnum = AIState.Seek;

                // Establecer el objetivo como el jugador m�s cercano
                aiStateMotor.target = playerColliders[0].transform;
            }
            else
            {
                // No tenemos ventaja o estamos igualados - Huir
                aiStateMotor.stateEnum = AIState.Flee;

                // Establecer el objetivo como el jugador del que queremos huir
                aiStateMotor.target = playerColliders[0].transform;
            }
        }

    }

    private void CountNearbyEnemies(Vector3 playerPosition)
    {
        // Contar cu�ntos enemigos (incluyendo a este) est�n cerca del jugador
        nearbyEnemyCount = 0;

        // Buscar objetos con el mismo tag que este enemigo
        string myTag = this.gameObject.tag;
        GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(myTag);

        foreach (GameObject enemy in allEnemies)
        {
            if (Vector3.Distance(enemy.transform.position, playerPosition) <= playerDetectionRadius)
            {
                nearbyEnemyCount++;
            }
        }
    }

    // M�todo de ayuda para visualizar los radios de detecci�n en el editor
    private void OnDrawGizmosSelected()
    {
        // Dibujar radio de detecci�n de jugador
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, playerDetectionRadius);

        // Dibujar radio de detecci�n de �tems
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, itemDetectionRadius);
    }
}