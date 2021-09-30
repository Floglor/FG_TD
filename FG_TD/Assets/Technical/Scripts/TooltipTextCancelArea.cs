using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTextCancelArea : MonoBehaviour, IPointerExitHandler
{
   
    public ToolTipText _toolTipText;
    public Camera cam;
    public bool activated;

    public void Activate()
    {
        //Debug.Log("kekw");
        gameObject.SetActive(true);
        MoveToMouse();
        activated = true;
    }
    private void Start()
    {
        activated = false;
        
        Hide();

    }
    

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnMouseExit()
    {
        //Debug.Log(activated);
        if (activated == false) return;
        activated = false;
        _toolTipText.Hide();
        Hide();
    }
    public void MoveToMouse()
    {
        
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponent<RectTransform>(),
            Input.mousePosition, cam, out localPoint);
        transform.localPosition = localPoint;
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log(activated);
        if (activated == false) return;
        activated = false;
        _toolTipText.Hide();
        Hide();
    }
}
