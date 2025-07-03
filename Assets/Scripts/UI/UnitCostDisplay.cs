using UnityEngine;
using TMPro;

public class UnitCostDisplay : MonoBehaviour
{
    public UnitSpawner unitSpawner; // —сылка на UnitSpawner
    public TextMeshProUGUI meatCostText;


    void Update()
    {
        if (unitSpawner != null && unitSpawner.units.Length > unitSpawner.selectedUnitIndex)
        {
            float cost = unitSpawner.units[unitSpawner.selectedUnitIndex].meatCost;
            meatCostText.text = cost.ToString();
        }
    }
}
