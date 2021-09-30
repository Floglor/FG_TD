using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipText : MonoBehaviour
{
    public Camera _camera;
    public TextMeshProUGUI textMeshProUGUI;

    public static ToolTipText instance;

    private void Awake()
    {
        instance = this;
        Hide();
    }

    private void Start()
    {
        MoveToMouse();
    }

    public void ShowUp()
    {
        gameObject.SetActive(true);
        NodeUI.instance.textCancelArea._toolTipText = this;
        NodeUI.instance.textCancelArea.Activate();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void MoveToMouse()
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition, _camera, out localPoint);
        transform.localPosition = localPoint;
        
    }

    public void ChangeText(string text)
    {
        textMeshProUGUI.text = text;
    }
    
    void Update()
    {
    }
}
