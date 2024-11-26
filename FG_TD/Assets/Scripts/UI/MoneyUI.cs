using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI moneyText;

    private void Update()
    {
        moneyText.text = PlayerStats.Money.ToString() + "G";
    }
}
