using UnityEngine;

public class Shop : MonoBehaviour
{
    public TurretBlueprint standartPinar;
    public TurretBlueprint fastPinar;
    public TurretBlueprint aoePinar;
    BuildManager buildManager;
    public GameObject shopCanvas;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }
    public void SelectStandartTurret()
    {
    
        buildManager.SetTurretToBuild(standartPinar);
    }

    public void SelectFastTurret()
    {
      
        buildManager.SetTurretToBuild(fastPinar);

    }

    public void SelectAOEturret()
    {
        buildManager.SetTurretToBuild(aoePinar);
    }

    public void Hide()
    {
        shopCanvas.SetActive(false);
        buildManager.DeselectNode();
    }

    public void Show()
    {
        shopCanvas.SetActive(true);
    }
}
