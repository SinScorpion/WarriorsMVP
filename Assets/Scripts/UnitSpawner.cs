using UnityEngine;

[System.Serializable]
public class UnitData
{
    public string unitName;
    public GameObject prefab;
    public float meatCost = 3f;
}
public class UnitSpawner : MonoBehaviour
{
    
    public Transform spawnPoint;
    
    public UnitData[] units;

    // —сылка на выбранного юнита
    public int selectedUnitIndex = 0;

    public void SpawnUnit()
    {
        var data = units[selectedUnitIndex];
        if (GameManager.Instance != null && GameManager.Instance.SpendMeat(data.meatCost))
        {
            Instantiate(data.prefab, spawnPoint.position, Quaternion.identity, transform);
        }
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
