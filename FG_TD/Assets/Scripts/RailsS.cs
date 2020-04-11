using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Orientation
{
    Horisontal,
    Vertical
}
public class RailsS : MonoBehaviour
{

    public Orientation orientation;
    [ConditionalField(nameof(orientation), false, Orientation.Horisontal)] public float yAlignment;
    [ConditionalField(nameof(orientation), false, Orientation.Vertical)] public float xAlignment;
    void SpawnLineAOE()
    {

    }
}
