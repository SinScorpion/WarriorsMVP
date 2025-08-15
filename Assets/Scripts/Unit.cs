using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private bool defendAtBase = false;
    // --- Hit flash ---
    [SerializeField] private Color hitFlashColor = Color.red; // цвет вспышки
    [SerializeField] private float hitFlashDuration = 0.08f;    // длительность вспышки, сек

    private SpriteRenderer[] hitRenderers;  // все спрайты юнита (на случай дочерних)
    private Color[] originalColors;         // их исходные цвета
    private Coroutine flashRoutine;         // чтобы не «накладывались» вспышки


    public bool isPlayerUnit = false;
    public float moveSpeed = 1f;
    public float maxHP = 10f;
    public float currentHP = 10f;
    //Расстояние до союзника
    public float allyCheckDistance = 0.3f;
    // Смещение при обходе
    public float avoidYOffset = 1.4f;
    public int rewardGold = 0;

    public Vector2 moveDirection = Vector2.right;

    //Атака
    public float damage = 2f;
    public float attackRange = 0.1f;
    public float attackCooldown = 1.0f;
    public AudioClip hitSound;
    public AudioSource audioSource;

    public HPBar hpBarPrefab; // Префаб HP-бара
    private HPBar hpBarInstance; // Его экземпляр

    private float attackTimer = 0f; // Внутренний таймер для перезарядки 
    private SpriteRenderer spriteRenderer;

    private bool IsAllyInFront()
    {
        Vector2 checkDir = isPlayerUnit ? Vector2.right : Vector2.left;
        Vector2 checkOrigin = (Vector2)transform.position + checkDir * 0.2f;
        RaycastHit2D hit = Physics2D.Raycast(checkOrigin, checkDir, allyCheckDistance, LayerMask.GetMask("Unit"));
        if (hit.collider !=null)
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit !=null && unit.isPlayerUnit == this.isPlayerUnit && unit != this)
            {
                return true;
            }
            
        }
        return false;
    }

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        hitRenderers = GetComponentsInChildren<SpriteRenderer>();
        if (hitRenderers == null || hitRenderers.Length == 0 && spriteRenderer != null)
            hitRenderers = new SpriteRenderer[] { spriteRenderer };

        originalColors = new Color[hitRenderers.Length];
        for (int i = 0; i < hitRenderers.Length; i++)
            originalColors[i] = hitRenderers[i].color;


        gameObject.tag = isPlayerUnit ? "PlayerUnit" : "EnemyUnit";

        currentHP = maxHP;

        // Создаем HP-бар
        if (hpBarPrefab !=null)
        {
            hpBarInstance = Instantiate(hpBarPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Canvas").transform);
            hpBarInstance.target = this.transform;
            
            hpBarInstance.SetHP(currentHP, maxHP);
            hpBarInstance.UpdatePositionImmediate();
            //Скрываем полоску хп при полном здоровье
            if (currentHP >= maxHP)
            {
                hpBarInstance.gameObject.SetActive(false);
            }
        }

        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void LateUpdate()
    {
        spriteRenderer.sortingOrder = Mathf.RoundToInt(-transform.position.y * 100);
    }
    // Update is called once per frame
    void Update()
    {
        attackTimer -= Time.deltaTime; // Уменьшаем таймер каждый кадр

        // 1) Если база в радиусе — бьём базу и никуда не уходим
        Base baseInRange = FindBaseInRange();
        if (baseInRange != null)
        {
            // Если включена самооборона и рядом есть враг — бьём врага
            if (defendAtBase)
            {
                Unit enemyClose = FindTargetInRange();
                if (enemyClose != null)
                {
                    if (attackTimer <= 0f)
                    {
                        Attack(enemyClose);
                        attackTimer = attackCooldown;
                    }
                    return; // стоим и бьём врага у базы
                }
            }

            // Иначе — бьём базу (как сейчас)
            if (attackTimer <= 0f)
            {
                AttackBase(baseInRange);
                attackTimer = attackCooldown;
            }
            return; // важный ранний выход
        }


        // 2) Иначе, если враг-юнит в радиусе — бьём юнита (твоё прежнее поведение)
        Unit target = FindTargetInRange();
        if (target != null)
        {
            if (attackTimer <= 0f)
            {
                Attack(target);
                attackTimer = attackCooldown; // Сброс перезарядки
            }
            return; // стоим на месте во время атаки
        }

        // 3) Никого рядом — двигаемся как раньше
        Unit closestEnemy = FindClosestEnemy();
        if (closestEnemy != null)
        {
            Vector2 targetPos = closestEnemy.transform.position;
            if (IsAllyInFront())
            {
                float offset = (Random.value > 0.5f) ? avoidYOffset : -avoidYOffset;
                Vector2 avoidDirection = new Vector2(moveDirection.x, offset);
                transform.Translate(avoidDirection * moveSpeed * Time.deltaTime);
            }
            else
            {
                Vector2 direction = ((Vector2)targetPos - (Vector2)transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
        }
        else
        {
            // как у тебя было: движемся к вражеской базе или по moveDirection
            Base[] allBases = FindObjectsOfType<Base>();
            Base enemyBase = null;
            foreach (var b in allBases)
            {
                if (b.isPlayerBase != this.isPlayerUnit)
                {
                    enemyBase = b;
                    break;
                }
            }
            if (enemyBase != null)
            {
                Vector2 direction = ((Vector2)enemyBase.transform.position - (Vector2)transform.position).normalized;
                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
            else
            {
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
            }
        }
    }


    protected virtual void Attack(Unit target)
    {
        //Запуск анимации
        GetComponent<Animator>().SetTrigger("Attack");
       /* target.TakeDamage(damage); */// Наносим урон врагу
        // Потом добавим анимацию и звук
    }

    protected virtual void AttackBase(Base targetBase)
    {
        GetComponent<Animator>().SetTrigger("Attack");
        //targetBase.TakeDamage(damage);

        if (hitSound != null && audioSource != null)
        {
            audioSource.volume = 0.1f;
            audioSource.PlayOneShot(hitSound);
        }
    }

    //Этот метод вызывает урон юниту
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        TriggerHitFlash();

        if (hpBarInstance !=null)
        {
            if (!hpBarInstance.gameObject.activeSelf)
            {
                hpBarInstance.gameObject.SetActive(true);
            }

            hpBarInstance.SetHP(currentHP, maxHP);
        }

        if (hitSound != null && audioSource != null)
        {
            audioSource.volume = 0.1f;

            if (currentHP - amount <= 0f)
            {
                GameObject tempGO = new GameObject("TempAudio");
                tempGO.transform.position = transform.position;

                AudioSource tempSource = tempGO.AddComponent<AudioSource>();
                tempSource.clip = hitSound;
                tempSource.volume = 0.1f;
                tempSource.Play();

                Destroy(tempGO, hitSound.length);
            }
            else
            {
                audioSource.PlayOneShot(hitSound);
            }

        }

        if (currentHP <=0f)
        {
            Die();
        }
    }

    public void DealDamage()
    {
        // Сначала база (если рядом), но с учётом самообороны
        Base baseTarget = FindBaseInRange();
        if (baseTarget != null)
        {
            if (defendAtBase)
            {
                Unit enemyClose = FindTargetInRange();
                if (enemyClose != null)
                {
                    enemyClose.TakeDamage(damage);
                    return;
                }
            }

            baseTarget.TakeDamage(damage);
            return;
        }

        // Как и раньше: вне базы — бьём ближайшего врага-юнита в радиусе
        Unit target = FindTargetInRange();
        if (target != null)
            target.TakeDamage(damage);
    }


    void Die()
    {
        if (hpBarInstance !=null)
        {
            Destroy(hpBarInstance.gameObject);
        }

        if (!isPlayerUnit && GameManager.Instance !=null)
        {
            GameManager.Instance.AddGold(rewardGold);
        }

        Destroy(gameObject);
    }

    // Поиск ближайшего врага в радиусе атаки
    Unit FindTargetInRange()
    {
        // Находим все коллайдеры в радиусе attackRange
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);

        Unit closest = null;
        float minDist = Mathf.Infinity;

        foreach (var hit in hits)
        {
            Unit other = hit.GetComponent<Unit>();
            if (other != null && other != this && other.isPlayerUnit != this.isPlayerUnit)
            {
                float dist = Vector2.Distance(transform.position, other.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = other;
                }
            }
        }
        return closest;
    }

    public Unit FindClosestEnemy()
    {
        // Определяем тэг вражеских юнитов
        string enemyTag = isPlayerUnit ? "EnemyUnit" : "PlayerUnit";

        // Ищем все объекты с этим тэгом
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        Unit closest = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;

        foreach (GameObject go in enemies)
        {
            Unit unit = go.GetComponent<Unit>();

            if (unit != null && unit !=this)
            {
                float dist = Vector3.Distance(currentPos, go.transform.position);
                if (dist <minDist)
                {
                    minDist = dist;
                    closest = unit;
                }
            }
        }
        return closest;
    }

    Base FindBaseInRange()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            Base targetBase = hit.GetComponent<Base>();
            if (targetBase != null && targetBase.isPlayerBase != this.isPlayerUnit)
            {
                return targetBase;
            }
        }
        return null;
    }

    private void TriggerHitFlash()
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(HitFlashCo());
    }

    private System.Collections.IEnumerator HitFlashCo()
    {
        // красим в «вспышку»
        for (int i = 0; i < hitRenderers.Length; i++)
            if (hitRenderers[i] != null) hitRenderers[i].color = hitFlashColor;

        yield return new WaitForSeconds(hitFlashDuration);

        // возвращаем цвета
        for (int i = 0; i < hitRenderers.Length; i++)
            if (hitRenderers[i] != null) hitRenderers[i].color = originalColors[i];

        flashRoutine = null;
    }

}
