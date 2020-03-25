
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

    private TurretBlueprint turretToBuild;


    public bool CanBuild
    {
        get { return turretToBuild != null; }
    }

    public bool HasMoney
    {
        get { return PlayerStats.Money >= turretToBuild.cost; }
    }


    public void SetTurretToBuild(TurretBlueprint turret)
    {
        turretToBuild = turret;
    }

    internal void BuildOn(Node node)
    {
        if (PlayerStats.Money < turretToBuild.cost)
        {
            Debug.Log("Not enough money");
            return;
        }

        PlayerStats.Money -= turretToBuild.cost;

        GameObject turret = Instantiate(turretToBuild.prefab, node.transform.position, Quaternion.identity);
        node.turret = turret;
    }






    // Start is called before the first frame update

}
