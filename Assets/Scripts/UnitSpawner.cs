using UnityEngine;

[System.Serializable]
public class UnitData
{
    public string unitName;
    public GameObject prefab;
    public float meatCost = 3f;
    public bool unlocked = false; //открыт ли слот персонажа
    public int unlockGoldCost = 150; // —колько золота дл€ открыти€

    
}
public class UnitSpawner : MonoBehaviour
{
    
    public Transform spawnPoint;
    
    public UnitData[] units;

    // —сылка на выбранного юнита
    public int selectedUnitIndex = 0;

    // Hero
    [SerializeField] private int heroUnitIndex = 3;
    private GameObject aliveHero;

    public int HeroUnitIndex { get { return heroUnitIndex; } }
    public bool IsHeroAlive { get { return aliveHero != null; } }

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

        if (index == heroUnitIndex && aliveHero != null)
            return false;

        var data = units[index];

        if (!data.unlocked)
        {
            return false;
        }

        if (GameManager.Instance !=null && GameManager.Instance.SpendMeat(data.meatCost))
        {
            GameObject go = Instantiate(data.prefab, spawnPoint.position, Quaternion.identity, transform);

            if (index == heroUnitIndex)
            {
                aliveHero = go;

                UniqueUnitLifetime marker = go.GetComponent<UniqueUnitLifetime>();
                if (marker == null) marker = go.AddComponent<UniqueUnitLifetime>();
                marker.Init(this);
            }

            return true;
        }

        return false;
    }

    public void NotifyHeroDied(GameObject who)
    {
        if (aliveHero == who)
            aliveHero = null;
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
