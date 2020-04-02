using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    private Node target;

    public void SetTarget(Node node)
    {
        target = node;

        
        ui.SetActive(true);
        target.OnMouseExit();
    }

    public void Hide()
    {
        ui.SetActive(false);
    }
}
