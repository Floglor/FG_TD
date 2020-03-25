using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint standartPinar;
    public TurretBlueprint fastPinar;
    BuildManager buildManager;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }
    public void SelectStandartTurret()
    {
        Debug.Log("Pinar turret selected");
        buildManager.SetTurretToBuild(standartPinar);
    }

    public void SelectFastTurret()
    {
        Debug.Log("Pinar second turret selected");
        buildManager.SetTurretToBuild(fastPinar);

    }
}
