using UnityEngine;
using UnityEngine.UI;

public class SpawnUnitButton : MonoBehaviour
{
    public Button button;
    public UnitSpawner unitSpawner;
    public int unitIndex;
   

    // Update is called once per frame
    void Update()
    {
        if (button !=null && unitSpawner != null)
        {
            float cost = unitSpawner.units[unitIndex].meatCost;
            button.interactable = GameManager.Instance.meat >= cost;
        }
    }

    public void OnClick()
    {
        unitSpawner.SpawnUnit(unitIndex);
    }
}
