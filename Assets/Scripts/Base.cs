using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private AudioClip playerBaseDestroyedSfx; // звук, когда ломают твою базу
    [SerializeField] private AudioClip enemyBaseDestroyedSfx;  // звук, когда ты ломаешь врага
    [SerializeField] private float destroyedSfxVolume = 0.85f; // громкость

    public float maxHP = 100f;
    public float currentHP = 100f;

    // --- Раздача золота по урону ---
    [Tooltip("Сколько монет в сумме даёт база при полном уничтожении.")]
    public int rewardGold = 0; // Золото за уничтожение базы
    private int goldAwardedSoFar = 0;     // сколько монет уже выдано за урон
    private float goldPerHp = 0f;         // монет за 1 HP урона

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

        goldPerHp = (maxHP > 0f) ? (rewardGold / maxHP) : 0f;
        goldAwardedSoFar = 0;

    }

    public void TakeDamage(float amount)
    {
        float prevHP = currentHP;

        currentHP -= amount;
        if (currentHP < 0f) currentHP = 0f;

        if (hpBar != null) hpBar.SetHP(currentHP, maxHP);

        //  НОВОЕ: выдаём монеты за нанесённый урон (только если бьём ВРАЖЕСКУЮ базу)
        if (!isPlayerBase && GameManager.Instance != null && rewardGold > 0)
        {
            // Сколько монет ДОЛЖНО быть выдано с учётом суммарного снесённого HP
            int expectedCoins = Mathf.FloorToInt((maxHP - currentHP) * goldPerHp);
            int toGive = expectedCoins - goldAwardedSoFar;
            if (toGive > 0)
            {
                GameManager.Instance.AddGold(toGive);
                goldAwardedSoFar += toGive;
            }
        }
        // 

        if (currentHP <= 0f)
        {
            Die();
        }
    }


    void Die()
    {
        if (hpBar != null)
            Destroy(hpBar.gameObject);

        // ПРОИГРЫВАЕМ ЗВУК
        PlayDestroyedSfx();

        // Показываем Win/Lose и стопаем бой
        GameStateController state = FindObjectOfType<GameStateController>();
        if (state != null)
        {
            if (isPlayerBase) state.OnPlayerBaseDestroyed();
            else state.OnEnemyBaseDestroyed();
        }

      

        Destroy(gameObject);
    }


    private void PlayDestroyedSfx()
    {
        AudioClip clip = isPlayerBase ? playerBaseDestroyedSfx : enemyBaseDestroyedSfx;
        if (clip == null) return;

        // временный источник, чтобы не оборвался при Destroy(this)
        GameObject temp = new GameObject("BaseDestroyedSFX");
        temp.transform.position = transform.position;

        var src = temp.AddComponent<AudioSource>();
        src.clip = clip;
        src.volume = destroyedSfxVolume;
        src.spatialBlend = 0f;               // 2D-звук для мобильных
        src.pitch = Random.Range(0.98f, 1.02f); // лёгкая вариативность
        src.Play();

        Destroy(temp, clip.length);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
