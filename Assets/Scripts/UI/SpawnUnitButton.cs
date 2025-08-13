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
        if (button ==null || unitSpawner == null || GameManager.Instance == null)
        {

            if (button != null) button.interactable = false;
            return;
        }
        if (unitSpawner.units == null || unitIndex < 0 || unitIndex >= unitSpawner.units.Length)
        {
            button.interactable = false;
            return;
        }

        var data = unitSpawner.units[unitIndex];

        bool canInteract = data.unlocked && GameManager.Instance.meat >= data.meatCost;

        //Блокировка кнопки героя
        if (unitIndex == unitSpawner.HeroUnitIndex && unitSpawner.IsHeroAlive)
        {
            button.interactable = false;
            return;
        }

        button.interactable = canInteract;
    }

    public void OnClick()
    {
        unitSpawner.SpawnUnit(unitIndex);
    }
}
