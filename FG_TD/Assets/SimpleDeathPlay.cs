using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleDeathPlay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (ParticleSystem componentsInChild in GetComponentsInChildren<ParticleSystem>())
        {
            componentsInChild.Play();
        }  
    }
    
}
