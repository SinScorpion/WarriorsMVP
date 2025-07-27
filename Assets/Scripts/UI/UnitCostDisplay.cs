using UnityEngine;
using TMPro;

public class UnitCostDisplay : MonoBehaviour
{
    public UnitSpawner unitSpawner; // —сылка на UnitSpawner
    public int unitIndex;
    public TextMeshProUGUI meatCostText;


    void Update()
    {
        if (unitSpawner != null && unitSpawner.units.Length > unitIndex)
        {
            float cost = unitSpawner.units[unitIndex].meatCost;
            meatCostText.text = cost.ToString();
        }
    }
}
