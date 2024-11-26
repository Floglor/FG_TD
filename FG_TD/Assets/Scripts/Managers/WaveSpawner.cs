using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Items;
using Managers;
using MyBox;
using ScriptableObjects;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = System.Random;

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
    public static List<LinearWaveScript> LinearWaves;
    private Random rng = new Random();

    //For wave end events
    public List<TowerAI> mineTowerList { get; set; }
    public List<TowerAI> essenceTowerList { get; set; } 
    


    private void Awake()
    {
        LinearWaves = new List<LinearWaveScript>();
        instance = this;
        essenceTowerList = new List<TowerAI>();
        mineTowerList = new List<TowerAI>();
        awaitingNextWaveButtonPress = true;
        NextWaveButtonOn();
        macroWaveNumber = 1;
        macroWaveNumberText.text = $"Wave {macroWaveNumber}";
    }

    private void NextWaveButtonOff()
    {
        nextWaveButton.GetComponent<Button>().interactable = false;
    }

    public void MakeWaveRelatedThings()
    { 
        awaitingNextWaveButtonPress = false;
      NextWaveButtonOff();
      
      

        foreach (TowerAI tower in mineTowerList)
        {
            tower.SpawnMine(tower.mineSpawnPerWave);
        }
        
    }

    private void GainEssences()
    {
        foreach (TowerAI tower in essenceTowerList)
        {
            PlayerStats.Essences += tower.essenceGainAmount;
            PlayerStats.Essences += tower.suckAmount;
            tower.suckAmount = 0;
            tower.isFullOfEssence = false;
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
                    PlayerStats.Essences += macroWave.essenceGain;
                    PlayerStats.instance.sellPoints += macroWave.sellPointGain;

                    macroWave = macroWave.nextMacroWave;
                    macroWaveNumber++;
                    waveNumber = 0;
                    countdown = 0;
                    awaitingNextWaveButtonPress = true;
                    NextWaveButtonOn();
                    GainEssences();
                    if (macroWaveNumber % 2 == 0)
                    {
                        Debug.Log($"MacroWaveNumber ({macroWaveNumber} % 2 = 0 ). Giving {macroWaveNumber/10} tier item");
                        GainItems(macroWaveNumber/10);
                    }
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

    private void NextWaveButtonOn()
    {
        nextWaveButton.GetComponent<Button>().interactable = true;
    }

    private void GainItems(int tier)
    {
        List<Item> itemsByTier = ItemManager.instance.allItems.Where(item => item.tier == tier).ToList();

        if (!itemsByTier.IsNullOrEmpty())
            ItemManager.instance.GetItemByArgument(itemsByTier[rng.Next(0, itemsByTier.Count)]);
        
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