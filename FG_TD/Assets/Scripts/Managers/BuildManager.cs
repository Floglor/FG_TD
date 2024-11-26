
using System;
using Managers;
using Shooting;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance;

    private ItemManager itemManager;
    
    

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene. ");
        }
        instance = this;
        itemManager = ItemManager.instance;
    }

    public NodeUI nodeUI;
    public Shop shop;

    private TurretBlueprint turretToBuild;
    private Node selectedTurret;
    private Node selectedNode;

    
  
    public void SelectNode(Node node)
    {
        if (selectedNode != null)
            DeselectNode();

        if (selectedTurret != null)
            DeselectTurret();

        selectedNode = node;
        node.SetSelected();
        if (node.turret == null)
            shop.ShowShop();
    }

    public void DeselectNode()
    {
        if (selectedNode != null)
        {
            selectedNode.isSelected = false;
            selectedNode.SetNormalColor();
            selectedNode = null;
            shop.Hide();
        }
    }

    public void SelectTurret(Node nodeWithTurret)
    {
        shop.Hide();
        nodeUI.Hide();
        if (selectedTurret == nodeWithTurret)
        {
            DeselectTurret();
            return;
        }

        selectedTurret = nodeWithTurret;
        turretToBuild = null;

        nodeUI.SetTarget(nodeWithTurret);

    }
    public void DeselectTurret()
    {
        selectedTurret = null;
        nodeUI.Hide();
    }

    public void SetTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
        selectedTurret = null;
        BuildOn(selectedNode);
        nodeUI.Hide();
    }

    internal void BuildOn(Node node)
    {
        if (PlayerStats.Money < turretToBuild.cost)
        {
            Debug.Log("Not enough money");
            return;
        }

        PlayerStats.Money -= turretToBuild.cost;

        
        GameObject turret = Instantiate(turretToBuild.prefab, new Vector3(node.transform.position.x, node.transform.position.y, -2), Quaternion.identity);

        turret.GetComponent<TowerAI>().totalCostRun = turretToBuild.cost;

        //Debug.Log(turret.GetComponent<TowerAI>().totalCost);
        
        node.turret = turret;
        node.UpdateTowerNode();
        node.ResetGraphics();
       // node.SetTowerColor();
        DeselectNode();
    }


}
