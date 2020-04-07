using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour
{
    public GameObject ui;
    private Node target;
    private TowerAI tower;
    List<UpgradeVariants> turretUpgrades;
    List<GameObject> buttons;

    public GameObject buttonPrefab;

    private void Awake()
    {
        buttons = new List<GameObject>();
        gameObject.SetActive(false);
    }

    public void SetTarget(Node node)
    {
        ClearButtons();
        target = node;
        tower = node.turret.GetComponent<TowerAI>();

        if (tower.upgradeVariants != null && tower.upgradeVariants.Count > 0)
            turretUpgrades = tower.upgradeVariants;
        else return;

        Canvas canvas = ui.GetComponentInChildren<Canvas>();
        HorizontalLayoutGroup panel = canvas.GetComponentInChildren<HorizontalLayoutGroup>();

       

        foreach (UpgradeVariants upgradeVariant in turretUpgrades)
        {
            GameObject newButton = Instantiate(buttonPrefab) as GameObject;
            

            newButton.transform.SetParent(panel.transform, false);

            Button buttonComponent = newButton.GetComponent<Button>();
            buttonComponent.onClick.AddListener(delegate { node.UpgrageTower(upgradeVariant); });

            Text text = newButton.GetComponentInChildren<Text>();

            StringBuilder sb = new StringBuilder();

            foreach (Stats stat in upgradeVariant.statList)
            {
                sb.Append(" " + stat.statName + "+" + stat.statValue);
                text.text = sb.ToString();
            }
            

 
        }

        foreach (UpgradeVariants upgradeVariant in turretUpgrades)
        {
            if (upgradeVariant.towerUpgrades.Count > 0)
            {
                foreach (GameObject tower in upgradeVariant.towerUpgrades)
                {
                    GameObject newButton = Instantiate(buttonPrefab) as GameObject;


                    newButton.transform.SetParent(panel.transform, false);

                    Button buttonComponent = newButton.GetComponent<Button>();
                    buttonComponent.onClick.AddListener(delegate { node.ReplaceTowerFromPrefab(tower); });

                    StringBuilder sb = new StringBuilder();

                    Text text = newButton.GetComponentInChildren<Text>();

                    sb.Append(tower.name);

                    text.text = sb.ToString();


                }

            }
            else return;

        }

        ui.SetActive(true);
        target.OnMouseExit();
    }

    public void Hide()
    {
        ui.SetActive(false);
        ClearButtons();
    }

    void ClearButtons()
    {
        if (buttons != null)
            buttons.Clear();
        else Debug.LogError("buttons is null!");
    }
}
