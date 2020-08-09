using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyBox;
using ScriptableObjects;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{
    public MacroWave macroWave;

    public GameObject nextWaveButton;

    private float countdown = 0;

    public TextMeshProUGUI waveCountDownText;
    public TextMeshProUGUI macroWaveNumberText;

    public Transform spawnPoint;

    private int waveNumber;
    private int macroWaveNumber;

    private bool gameOver;
    private bool awaitingNextWaveButtonPress;

    public static WaveSpawner instance;

    //For wave end events
    public List<TowerAI> mineTowerList { get; set; }


    private void Awake()
    {
        instance = this;
        mineTowerList = new List<TowerAI>();
        awaitingNextWaveButtonPress = true;
        nextWaveButton.SetActive(true);
        macroWaveNumber = 1;
        macroWaveNumberText.text = $"Wave {macroWaveNumber}";
    }

    public void MakeWaveRelatedThings()
    {
        awaitingNextWaveButtonPress = false;
        nextWaveButton.SetActive(false);

        foreach (TowerAI tower in mineTowerList)
        {
            tower.SpawnMine(tower.mineSpawnPerWave);
        }
    }

    private void Update()
    {
        //Debug.Log(PlayerStats.enemiesAlive);
        if (gameOver) return;

        if (countdown <= 0f)
        {
            if (waveNumber >= macroWave.microwaves.Count
                && System.Object.ReferenceEquals(macroWave.nextMacroWave, null)
                && !gameOver)
            {
                Debug.Log("That's it.");
                countdown = 0;
                gameOver = true;
            }


            if (gameOver) return;


            if (waveNumber >= macroWave.microwaves.Count && macroWave.nextMacroWave != null)
            {
                if (PlayerStats.enemiesAlive == 0)
                {
                    PlayerStats.Money += macroWave.moneyGain;
                    PlayerStats.instance.SpendMana(-macroWave.manaGain);

                    macroWave = macroWave.nextMacroWave;
                    macroWaveNumber++;
                    waveNumber = 0;
                    countdown = 0;
                    awaitingNextWaveButtonPress = true;
                    nextWaveButton.SetActive(true);
                }
            }
            else if (!awaitingNextWaveButtonPress)
            {
                for (int i = waveNumber; i < macroWave.microwaves.Count; i++)
                {
                    
                    AdvanceWave();

                    if (!macroWave.microwaves[i].nextIsSimultaneous)
                        break;
                    
                }
            }
        }

        if (awaitingNextWaveButtonPress) return;
        macroWaveNumberText.text = $"Wave {macroWaveNumber}";
        countdown -= Time.deltaTime * PlayerStats.instance.gameSpeedMultiplier;
        countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
        waveCountDownText.text = $"{countdown:00.00}";
    }

    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public void AdvanceWave()
    {
        //Debug.Log(nextIsSimultaneous);
        
        StartCoroutine(SpawnWave(macroWave.microwaves[waveNumber]));
        
        if (!macroWave.microwaves[waveNumber].nextIsSimultaneous)
        {
            countdown = macroWave.microwaves[waveNumber].nextDelay +
                        macroWave.microwaves[waveNumber].enemyCount * 0.3f;
            
        }
        
        PlayerStats.enemiesAlive += macroWave.microwaves[waveNumber].enemyCount;
        waveNumber++;
    }

    private IEnumerator SpawnWave(MicroWave wave)
    {
        for (int i = 0; i < wave.enemyCount; i++)
        {
            SpawnEnemy(wave.enemyType.transform);
            yield return new WaitForSeconds(0.3f / PlayerStats.instance.gameSpeedMultiplier);
        }
    }

    void SpawnEnemy(Transform enemyPrefab)
    {
        Enemy enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation).GetComponent<Enemy>();
        
        
    }
}