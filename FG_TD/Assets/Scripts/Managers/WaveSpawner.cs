using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public Transform enemyPrefab;

    public MacroWave macroWave;

    
    private float countdown = 2f;

    public TextMeshProUGUI waveCountDownText;

    public Transform spawnPoint;

    public int numberOfEnemies = 4;

    private int waveNumber = 0;

    private bool gameOver;
    private void Update()
    {
        if (countdown <= 0f)
        {
            if (waveNumber >= macroWave.microwaves.Count && macroWave.nextMacroWave == null && !gameOver)
            {
                Debug.Log("That's it.");
                gameOver = true;
            }

            if (!gameOver)
            {
                StartCoroutine(SpawnWave());
                countdown = macroWave.microwaves[waveNumber].nextDelay
                + (macroWave.microwaves[waveNumber].enemyCount * 0.3f);
            }


        }

        if (!gameOver)
        {
            countdown -= Time.deltaTime;
            countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
            waveCountDownText.text = string.Format("{0:00.00}", countdown);
        }
    }

    IEnumerator SpawnWave()
    {
        Debug.Log(waveNumber);

        if (waveNumber >= macroWave.microwaves.Count && macroWave.nextMacroWave != null)
        {
            macroWave = macroWave.nextMacroWave;
            waveNumber = 0;
            yield break;
        }

        if (macroWave.microwaves.Count >= waveNumber)
        for (int i = 0; i < macroWave.microwaves[waveNumber].enemyCount; i++)
        {

            SpawnEnemy(macroWave.microwaves[waveNumber].enemyType.transform);
            yield return new WaitForSeconds(0.3f);
        }

        if (macroWave.microwaves.Count != waveNumber)
        {
            waveNumber++;
        }

        Debug.Log(waveNumber);


    }

    void SpawnEnemy(Transform enemyPrefab)
    {
        Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
    }






}
