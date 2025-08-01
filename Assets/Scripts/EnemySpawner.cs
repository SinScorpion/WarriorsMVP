using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public Transform spawnPoint;

    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject enemyPrefab;
        public int count = 1; // Сколько таких врагов спавнить
        public float spawnDelay = 0.5f;
    }

    //Структура одной волны
    [System.Serializable]
    public class Wave
    {
        public float startTime = 0f;
        public List<EnemySpawnData> enemies = new List<EnemySpawnData>();
        public float interEnemyDelay = 0.5f; // Задержка между разными типами
        
    }

    //Список всех волн
    public List<Wave> waves = new List<Wave>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        foreach (var wave in waves )
        {
            // Старт волны
            yield return new WaitForSeconds(wave.startTime);

            foreach (var enemyData in wave.enemies)
            {
                for (int i = 0; i < enemyData.count; i++)
                {
                    Instantiate(enemyData.enemyPrefab, spawnPoint.position, Quaternion.identity, transform);
                    yield return new WaitForSeconds(enemyData.spawnDelay);

                }
                yield return new WaitForSeconds(wave.interEnemyDelay);
            }

            
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
