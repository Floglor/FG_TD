using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DescriptionLinks : MonoBehaviour
{
    public static DescriptionLinks Instance;
    public List<DescriptionLink> descriptionLinks;

    private void Awake()
    {
        Instance = this;
    }
}