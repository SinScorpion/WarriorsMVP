using UnityEngine;

public class Base : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP = 100f;
    public int rewardGold = 0; // Золото за уничтожение базы
    public bool isPlayerBase = false;
    [HideInInspector] public HPBar hpBar;
    public GameObject hpBarPrefab;
    public Vector3 hpBarOffset = new Vector3(0.2f, 0.7f, 0);

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        HPBar bar = Instantiate(hpBarPrefab, canvas.transform).GetComponent<HPBar>();
        bar.target = this.transform;
        bar.offset = hpBarOffset;
        bar.SetHP(currentHP, maxHP);

        hpBar = bar;

        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP<0)
        {
            currentHP = 0;
        }
        hpBar.SetHP(currentHP, maxHP);

        if (currentHP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (hpBar != null)
        {
            Destroy(hpBar.gameObject);
        }

        GameStateController state = FindObjectOfType<GameStateController>();
        if (state !=null)
        {
            if (isPlayerBase)
                state.OnPlayerBaseDestroyed();
            else
                state.OnEnemyBaseDestroyed();
        }

        if (!isPlayerBase && GameManager.Instance !=null)
        {
            GameManager.Instance.AddGold(rewardGold);
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
