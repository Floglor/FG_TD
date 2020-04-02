using UnityEngine;
using UnityEngine.EventSystems;
public class Node : MonoBehaviour
{

    public Color hoverColor;
    private SpriteRenderer rend;
    private Color startColor;

    public Color notEnoughMoneyColor;

    public Color selectedColor;

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

        if (turret != null)
        {

            buildManager.SelectTurret(this);
            return;
        }
        else
        {
            buildManager.SelectNode(this);
        }



        // buildManager.BuildOn(this);

    }

    private void OnMouseEnter()
    {
        // if (!buildManager.CanBuild)
        //     return;

        //  if (buildManager.HasMoney)
        //  {
        if (rend.material.color != selectedColor)
            rend.material.color = hoverColor;

        // }
        // else
        // {
        //     rend.material.color = notEnoughMoneyColor;
        // }


    }

    public Vector3 GetBuildPosition()
    {
        return transform.position;
    }

    public void OnMouseExit()
    {
        if (rend.material.color != selectedColor)
            rend.material.color = startColor;
    }

    public void SetNormalColor()
    {
        rend.material.color = startColor;
    }



    public void SetSelected()
    {
        rend.material.color = selectedColor;
    }


}
