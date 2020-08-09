using UnityEngine;
using UnityEngine.Serialization;

public class Shop : MonoBehaviour
{
    [FormerlySerializedAs("standartPinar")] public TurretBlueprint firstPinar;
    [FormerlySerializedAs("fastPinar")] public TurretBlueprint secondPinar;
    [FormerlySerializedAs("aoePinar")] public TurretBlueprint thirdPinar;
    public TurretBlueprint fourthPinar;
    
    
    BuildManager buildManager;
    public GameObject shopCanvas;

    private void Start()
    {
        buildManager = BuildManager.instance;
    }
    public void SelectStandartTurret()
    {
    
        buildManager.SetTurretToBuild(firstPinar);
    }

    public void SelectFastTurret()
    {
      
        buildManager.SetTurretToBuild(secondPinar);

    }

    public void SelectFourthTurret()
    {
        buildManager.SetTurretToBuild(fourthPinar);
    }
    

    public void SelectAOEturret()
    {
        buildManager.SetTurretToBuild(thirdPinar);
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
