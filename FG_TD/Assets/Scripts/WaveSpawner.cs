using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab;

    public float timeBetweenWaves = 500f;
    private float countdown = 2f;

    public Text waveCountDownText;

    public Transform spawnPoint;

    public int numberOfEnemies = 4;

    private int waveNumber = 0;
    private void Update()
    {
        if (countdown <= 0f)
        {
            
                StartCoroutine(SpawnWave());
            countdown = timeBetweenWaves;
                //+ (waveNumber * 0.5f);
            
           
        }

        countdown -= Time.deltaTime;
        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
        waveCountDownText.text = string.Format("{0:00.00}", countdown);
    }

    IEnumerator SpawnWave()
    {
        waveNumber++;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(0.3f);
        }
      

    }

    void SpawnEnemy()
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }


   
 

   
}
