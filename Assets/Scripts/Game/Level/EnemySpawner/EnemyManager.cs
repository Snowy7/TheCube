using System.Collections.Generic;
using System.Linq;
using Actors.AI;
using NUnit.Framework;
using Snowy.Utils;
using UnityEngine;

namespace Level
{
    public class EnemyManager : MonoSingleton<EnemyManager>
    {
        [SerializeField] private List<Enemy> enemies = new();
        [SerializeField] private int patrolPointPerEnemy = 2;
        
        private Dictionary<Enemy, Transform> enemySpawnPoints = new(); 
        private Dictionary<Enemy, Transform> enemyToPatrolPoint = new(); 
        
        
        public int EnemyCount => enemies.Count;
        
        public void RegisterEnemy(Enemy enemy)
        {
            // Register enemy
            enemies.Add(enemy);
            
            // Assign the avoidance priority
            enemy.AssignPriority(enemies.Count);
            
            // Save the spawn point
            enemySpawnPoints.Add(enemy, enemy.transform);
        }
        
        public void UnregisterEnemy(Enemy enemy)
        {
            // Unregister enemy
            enemies.Remove(enemy);
            
            // Reassign the avoidance priority
            for (var i = 0; i < enemies.Count; i++)
            {
                enemies[i].AssignPriority(i + 1);
            }
        }

        public Transform GetPatrolPoint(Enemy enemy, Transform prev)
        {
            // Get closest available patrol point
            var patrolPoints = EnemySpawner.Instance.EnemyWaypoints;
            
            // filter the previous patrol point
            patrolPoints = patrolPoints.Where(point => point != prev).ToArray();
            
            // sort
            System.Array.Sort(patrolPoints, (a, b) => Vector3.Distance(enemy.transform.position, a.position).CompareTo(Vector3.Distance(enemy.transform.position, b.position)));
            
            // filter out the patrol points that are already assigned
            var availablePoints = patrolPoints.Where(point => !enemyToPatrolPoint.ContainsValue(point)).ToArray();
            
            // if there are no available points, return the closest point
            if (availablePoints.Length == 0)
            {
                enemyToPatrolPoint[enemy] = patrolPoints[0];
                return patrolPoints[0];
            }
            
            // assign the patrol point
            enemyToPatrolPoint[enemy] = availablePoints[0];
            // return the closest available point
            return availablePoints[0];
        }
        
        // Alert all enemies
        public void AlertNearbyEnemies(Vector3 location, float radius)
        {
            Debug.Log("Alerting enemies");
            foreach (var enemy in enemies)
            {
                if (Vector3.Distance(enemy.transform.position, location) <= radius)
                {
                    Debug.Log("Alerting enemy");
                    enemy.Alert(location);
                }
            }
        }
        
        public int GetEnemyTypeCount(int id) => enemies.FindAll(enemy => enemy.typeID == id && !enemy.IsDead).Count;
        
        public int GetEnemySpawnerCount(int id) => enemies.FindAll(enemy => enemy.spawnerID == id && !enemy.IsDead).Count;
    }
}