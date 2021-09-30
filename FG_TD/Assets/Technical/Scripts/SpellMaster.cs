using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellMaster : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || PlayerStats.instance.chargedSpell == null) return;
        
        Vector2 mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        
        
        Collider2D[] colliders = Physics2D.OverlapCircleAll(mousePosition, 0.1f);

        bool railFound = false;
        GameObject rail = null;

        foreach (Collider2D collider2D1 in colliders)
        {
            if (!collider2D1.CompareTag(Rail.myTag)) continue;
            railFound = true;
            rail = collider2D1.gameObject;
        }
            
        if (!railFound)
        {
           PlayerStats.instance.CancelSpell();
           return;
        }
        
        PlayerStats.instance.chargedSpell.TakeEffect(rail, mousePosition);
        PlayerStats.instance.CancelSpell();
        
    }
}
