using System.Collections;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public static int Money;
    public int startMoney = 4;

    public static int Lives;
    public int startLives = 30;

    private void Start()
    {
        Money = startMoney;
        Lives = startLives;
    }
}
   
  
