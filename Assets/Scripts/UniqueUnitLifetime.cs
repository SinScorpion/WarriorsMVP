using UnityEngine;

public class UniqueUnitLifetime : MonoBehaviour
{
    private UnitSpawner spawner;

    public void Init(UnitSpawner s)
    {
        spawner = s;

    }

    private void OnDestroy()
    {
        if (spawner != null)
            spawner.NotifyHeroDied(gameObject);
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
