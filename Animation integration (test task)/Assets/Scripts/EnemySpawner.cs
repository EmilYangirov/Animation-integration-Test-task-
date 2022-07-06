using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Enemy enemyPrefab;
    [SerializeField] private CharacterController player;

    [SerializeField] private float spawnRadius;

    private Enemy createdEnemy;


    private void Start()
    {
        SpawnEnemy();
    }
    private void SpawnEnemy()
    {               
        createdEnemy = Object.Instantiate(enemyPrefab);
        createdEnemy.transform.position = NewEnemyPosition();
        createdEnemy.OnKill += SpawnEnemyEvent;
    }

    private void SpawnEnemyEvent()
    {
        createdEnemy.OnKill -= SpawnEnemyEvent;
        GameObject enemyGo = createdEnemy.gameObject;
        createdEnemy = null;
        
        SpawnEnemy();
    }

    private Vector3 NewEnemyPosition()
    {
        Vector3 newPosition = player.transform.position;

        while (Vector3.Distance(newPosition, player.transform.position) < 3)
            newPosition = player.transform.position + Random.insideUnitSphere * spawnRadius;

        newPosition.y = 0;

        return newPosition;
    }
    
}
