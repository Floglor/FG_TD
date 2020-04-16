using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Orientation
{
    Horizontal,
    Vertical
}
public class RailsS : MonoBehaviour
{

    public Orientation orientation;
    [ConditionalField(nameof(orientation), false, Orientation.Horizontal)] public float yAlignment;
    [ConditionalField(nameof(orientation), false, Orientation.Vertical)] public float xAlignment;
    void SpawnLineAOE()
    {

    }
}
