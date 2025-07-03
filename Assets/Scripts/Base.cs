using UnityEngine;

public class Base : MonoBehaviour
{
    public float maxHP = 100f;
    public float currentHP = 100f;
    public int rewardGold = 0; // Золото за уничтожение базы
    public bool isPlayerBase = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
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
