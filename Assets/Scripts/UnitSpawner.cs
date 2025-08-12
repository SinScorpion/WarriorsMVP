using UnityEngine;

[System.Serializable]
public class UnitData
{
    public string unitName;
    public GameObject prefab;
    public float meatCost = 3f;
    public bool unlocked = false; //������ �� ���� ���������
    public int unlockGoldCost = 150; // ������� ������ ��� ��������
}
public class UnitSpawner : MonoBehaviour
{
    
    public Transform spawnPoint;
    
    public UnitData[] units;

    // ������ �� ���������� �����
    public int selectedUnitIndex = 0;

    public void SpawnUnit(int index)
    {
        TrySpawnUnit(index);
        
    }

    public bool TrySpawnUnit(int index)
    {
        if (units == null || index <0 || index >= units.Length)
        {
            Debug.LogWarning($"[UnitSpawner] Bad index {index}");
            return false;
        }

        var data = units[index];

        if (!data.unlocked)
        {
            return false;
        }

        if (GameManager.Instance !=null && GameManager.Instance.SpendMeat(data.meatCost))
        {
            Instantiate(data.prefab, spawnPoint.position, Quaternion.identity, transform);
            return true;
        }

        return false;
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
