
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour
{

    public TextMeshProUGUI livesText;
   
    void Update()
    {
        livesText.text = PlayerStats.Lives.ToString();
    }
}
