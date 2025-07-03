using UnityEngine;
using UnityEngine.UI;

public class SpawnUnitButton : MonoBehaviour
{
    public Button button;
    public UnitSpawner unitSpawner;
   

    // Update is called once per frame
    void Update()
    {
        if (button !=null && unitSpawner != null)
        {
            float cost = unitSpawner.units[unitSpawner.selectedUnitIndex].meatCost;
            button.interactable = GameManager.Instance.meat >= cost;
        }
    }
}
