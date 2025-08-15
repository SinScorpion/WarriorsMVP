using UnityEngine;

public class Base : MonoBehaviour
{
    [SerializeField] private AudioClip playerBaseDestroyedSfx; // звук, когда ломают твою базу
    [SerializeField] private AudioClip enemyBaseDestroyedSfx;  // звук, когда ты ломаешь врага
    [SerializeField] private float destroyedSfxVolume = 0.85f; // громкость

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

        // Награда за уничтожение вражеской базы
        if (!isPlayerBase && GameManager.Instance != null)
            GameManager.Instance.AddGold(rewardGold);

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
