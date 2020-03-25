using UnityEngine;
using UnityEngine.EventSystems;
public class Node : MonoBehaviour
{

    public Color hoverColor;
    private SpriteRenderer rend;
    private Color startColor;

    public Color notEnoughMoneyColor;

    [Header("Optional")]
    public GameObject turret;

    BuildManager buildManager;

    private void Start()
    {
        buildManager = BuildManager.instance;
        rend = GetComponent<SpriteRenderer>();
        startColor = rend.material.color;
    }

    private void OnMouseDown()
    {

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (!buildManager.CanBuild)
            return;
        
        if (turret != null)
        {
            Debug.Log("Can't build there");
            return;
        }

        buildManager.BuildOn(this);
        

        //Build a turret
    }

    private void OnMouseEnter()
    {
        if (!buildManager.CanBuild)
            return;

        if (buildManager.HasMoney)
        {
            rend.material.color = hoverColor;

        }
        else
        {
            rend.material.color = notEnoughMoneyColor;
        }

       
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}
