using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EssenceText : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI essenceText;

    private void Update()
    {
        essenceText.text = PlayerStats.Essences.ToString() + "E";
    }
   
    
}
