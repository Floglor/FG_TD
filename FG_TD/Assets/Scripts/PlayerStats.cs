using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int Money;
    public int startMoney = 4;

    public static int Lives;
    public int startLives = 30;

    public static PlayerStats instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one PlayerStats in scene. ");
        }
        instance = this;
    }

    private void Start()
    {
        Money = startMoney;
        Lives = startLives;
    }

    public static bool SpendMoney(int cost)
    {
        if (Money < cost)
            return false;
        else
        {
            Money -= cost;
            return true;
        }
        
    }
}
   
  
