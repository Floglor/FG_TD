using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyBox;
using Prefaps.Consumables_and_traps;
using Prefaps.Spells.SpellScripts;
using Shooting;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ReSharper disable InconsistentNaming

public class PlayerStats : MonoBehaviour
{
    public static int Money;
    public int startMoney = 4;

    public static int Lives;
    public int startLives = 30;

    public static readonly float NodeWidth = 0.875f;

    public int startMana;
    public int maxMana;
    public int Mana;

    public int maxEssences;
    public int startEssences;
    public static int Essences;

    public static int enemiesAlive;

    public static PlayerStats instance;
    public List<GameObject> spells;
    public List<GameObject> consumables;
    public List<TowerAI> towers { get; set; }
    public Spell chargedSpell { get; set; }
    public Consumable chargedConsumable { get; set; }


    [Range(0, 100)] public int sellingPercentage;

    [Header("GLOBAL VALUES")] public int disruptionMultiplier;

    public float flightSpeedBuffFlat;
    [Range(0, 100)] public int flightSpeedBuffPercentage;

    [Header("GLOBAL VISUAL EFFECTS")] public GameObject shieldHitEffect;
    public GameObject shieldBreakEffect;
    public GameObject shield;
    public GameObject lightning;
    public GameObject lightningBeam;

    [Header("Unity Specific")] public Image manaBar;

    public Canvas spellCanvas;

    public TextMeshProUGUI gameSpeedText;

    public int gameSpeedMultiplier { get; set; }

    [Header("GLOBAL ALIGNMENT MVSP CHANGE")]
    
    public float smallMVSPDifferenceBarrier;
    [Range(0, 100)] public int smallMVSPChangePercent;

    public float mediumMVSPDifferenceBarrier;
    [Range(0, 100)] public int mediumMVSPChangePercent;

    public float bossMVSPDifferenceBarrier;
    [Range(0, 100)] public int bossMVSPChangePercent;

    public float uniqueMVSPDifferenceBarrier;
    [Range(0, 100)] public int uniqueMVSPChangePercent;


    private void Awake()
    {
        towers = new List<TowerAI>();
        gameSpeedText.text = "1x";
        gameSpeedMultiplier = 1;
        VerticalLayoutGroup panel = spellCanvas.GetComponentInChildren<VerticalLayoutGroup>();
        enemiesAlive = 0;
        if (instance != null)
        {
            Debug.LogError("More than one PlayerStats in scene. ");
        }

        instance = this;

        if (spells != null)
            foreach (GameObject spell in spells)
            {
                Button newButton = Instantiate(spell.GetComponent<Spell>().buttonPrefab, panel.transform, false);
                Button buttonComponent = newButton.GetComponent<Button>();

                buttonComponent.onClick.AddListener(delegate { ChargeSpell(spell.GetComponent<Spell>()); });
            }

        foreach (GameObject consumable in consumables)
        {
            Button newButton = Instantiate(consumable.GetComponent<Consumable>().buttonPrefab, panel.transform, false);
            Button buttonComponent = newButton.GetComponent<Button>();

            buttonComponent.onClick.AddListener(delegate { ChargeConsumable(consumable.GetComponent<Consumable>()); });
        }
    }

    private void Start()
    {
        Essences = startEssences;
        Money = startMoney;
        Lives = startLives;
        Mana = startMana;
        manaBar.fillAmount = (float) Mana / (float) maxMana;
    }

    public bool SpendMoney(int cost)
    {
        if (cost > 0)
            if (Money < cost)
                return false;

        Money -= cost;
        return true;
    }

    public bool SpendEssences(int cost)
    {
        if (cost > 0)
            if (Essences < cost)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.Log("Not enough essences");
                return false;
            }

        Essences -= cost;
        if (Essences > maxEssences) Essences = maxEssences;
        return true;
    }

    public bool SpendMana(int cost)
    {
        if (cost > 0)
            if (Mana < cost)
            {
                // ReSharper disable once Unity.PerformanceCriticalCodeInvocation
                Debug.Log("Not enough mana");
                return false;
            }

        Mana -= cost;
        if (Mana > 100) Mana = 100;
        manaBar.fillAmount = (float) Mana / (float) maxMana;
        return true;
    }

    public void ChargeSpell(Spell spell)
    {
        chargedSpell = spell;
        Debug.Log($"{spell.name} spell charged");
    }

    public void CancelSpell()
    {
        chargedSpell = null;
    }

    public void ChargeConsumable(Consumable consumable)
    {
        chargedConsumable = consumable;
        Debug.Log($"{consumable.name} consumable charged");
    }

    public void CancelConsumable()
    {
        chargedConsumable = null;
    }

    public void GameSpeedUp()
    {
        gameSpeedMultiplier++;
        gameSpeedText.text = gameSpeedMultiplier + "x";

        ReinvokeTowers();
    }

    private void ReinvokeTowers()
    {
        foreach (TowerAI towerAI in towers.Where(towerAI => towerAI.isRoundAttacking))
        {
            towerAI.InvokeRoundAttacking();
        }
    }

    public void GameSpeedDown()
    {
        if (gameSpeedMultiplier == 1) return;

        gameSpeedMultiplier--;
        gameSpeedText.text = gameSpeedMultiplier + "x";

        ReinvokeTowers();
    }
}