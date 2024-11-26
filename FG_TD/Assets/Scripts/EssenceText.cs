using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EssenceText : MonoBehaviour
{
    // Start is called before the first frame update

    private TextMeshProUGUI thisText;

    private void Start()
    {
        thisText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        thisText.text = PlayerStats.Essences + "E";
    }
   
    
}
