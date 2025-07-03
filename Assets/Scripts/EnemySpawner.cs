using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    public GameObject enemyPrefab;
    public Transform spawnPoint;

    //Структура одной волны
    [System.Serializable]
    public class Wave
    {
        public int enemyCount = 1;
        public float spawnInterval = 2f;
        public float startTime = 0f;
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

            for (int i = 0; i < wave.enemyCount; i++)
            {
                Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity, transform);
                // Интервал между врагами
                yield return new WaitForSeconds(wave.spawnInterval);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
