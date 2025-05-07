using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Actors.AI;
using Level;
using Snowy.Utils;

[Serializable] class Spawner
{
    public int spawnCapacity;
    public int maxAtTime;
    public Transform spawnPosition;
    public int spawnArea;
    public Enemy enemy;
}

public class EnemySpawner : MonoSingleton<EnemySpawner>
{
    [SerializeField, ReorderableListExposed] private Spawner[] spawners;
    [SerializeField, ReorderableList] private List<Transform> enemyWaypoints;
    [SerializeField] private float checkInterval = 1f;
    
    public Transform[] EnemyWaypoints => enemyWaypoints.ToArray();
    
    private void Start()
    {
        if (NetworkServer.active)
            InvokeRepeating(nameof(CheckSpawners), 0f, checkInterval);
    }
    
    private void CheckSpawners()
    {
        for (var i = 0; i < spawners.Length; i++)
        {
            var spawner = spawners[i];
            if (spawner.spawnCapacity > 0)
            {
                var enemyCount = EnemyManager.Instance.GetEnemySpawnerCount(i);
                if (enemyCount < spawner.maxAtTime)
                {
                    SpawnEnemy(spawner, spawner.maxAtTime - enemyCount);
                }
            }
        }
    }
    
    private void SpawnEnemy(Spawner spawner, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var pos = spawner.spawnPosition.position;
            pos.x += UnityEngine.Random.Range(-spawner.spawnArea, spawner.spawnArea);
            pos.z += UnityEngine.Random.Range(-spawner.spawnArea, spawner.spawnArea);
            
            var enemy = Instantiate(spawner.enemy, spawner.spawnPosition.position, Quaternion.identity);
            enemy.spawnerID = Array.IndexOf(spawners, spawner);
            NetworkServer.Spawn(enemy.gameObject);
            spawner.spawnCapacity--;
        }
    }
    
    # if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        foreach (var spawner in spawners)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(spawner.spawnPosition.position, spawner.spawnArea);
        }
    }
    # endif
}
