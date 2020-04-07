
using System;
using UnityEngine;

public class BuildManager : MonoBehaviour
{

    public static BuildManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("More than one BuildManager in scene. ");
        }
        instance = this;
    }

    public GameObject standartTurretPrefab;
    public GameObject secondaryTurretPrefab;
    public GameObject aoeTurretPrefab;

    public NodeUI nodeUI;
    public Shop shop;

    private TurretBlueprint turretToBuild;
    private Node selectedTurret;
    private Node selectedNode;




   // public bool HasMoney
   // {
   //     get { return PlayerStats.Money >= turretToBuild.cost; }
   // }

  
    public void SelectNode(Node node)
    {
        if (selectedNode != null)
            DeselectNode();

        selectedNode = node;
        node.SetSelected();
        shop.Show();
    }

    public void DeselectNode()
    {
        selectedNode.SetNormalColor();
        selectedNode = null;
        shop.Hide();
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
    private void DeselectTurret()
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
        node.turret = turret;
        DeselectNode();
    }


}
