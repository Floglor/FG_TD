using System;
using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Orientation
{
    Horizontal,
    Vertical
}
public class Rail : MonoBehaviour
{
    public const string myTag = "Rail";
    
    public Orientation orientation;
    [ConditionalField(nameof(orientation), false, Orientation.Horizontal)] public float yAlignment;
    [ConditionalField(nameof(orientation), false, Orientation.Vertical)] public float xAlignment;
    void SpawnLineAOE()
    {

    }
    
}
