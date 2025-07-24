using UnityEngine;

public class Unit : MonoBehaviour
{
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
    private AudioSource audioSource;

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

        Unit target = FindTargetInRange(); // Ищем врага в радиусе

        if (target != null)
        {
            if (attackTimer <= 0f)
            {
                Attack(target);
                attackTimer = attackCooldown; // Сброс перезарядки
            }
            // Если есть враг - НЕ двигаемся!
        }
        else
        {
            // Движение к ближайшему врагу
            Unit closestEnemy = FindClosestEnemy();
            if (closestEnemy !=null)
            {
                Vector2 targetPos = closestEnemy.transform.position;
                if (IsAllyInFront())
                {
                    float offset = (Random.value > 0.5f) ? avoidYOffset : -avoidYOffset;
                    Vector2 avoidDirection = new Vector2(moveDirection.x, offset).normalized;
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
                Base targetBase = FindBaseInRange();
                if (targetBase !=null)
                {
                    if (attackTimer <= 0f)
                    {
                        AttackBase(targetBase);
                        attackTimer = attackCooldown;
                    }

                }
                else
                {
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
                    if (enemyBase !=null)
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
            
        }
    }

    void Attack(Unit target)
    {
        //Запуск анимации
        GetComponent<Animator>().SetTrigger("Attack");
       /* target.TakeDamage(damage); */// Наносим урон врагу
        // Потом добавим анимацию и звук
    }

    void AttackBase(Base targetBase)
    {
        GetComponent<Animator>().SetTrigger("Attack");
        //targetBase.TakeDamage(damage);
    }

    //Этот метод вызывает урон юниту
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (hpBarInstance !=null)
        {
            if (!hpBarInstance.gameObject.activeSelf)
            {
                hpBarInstance.gameObject.SetActive(true);
            }

            hpBarInstance.SetHP(currentHP, maxHP);
        }
        if (currentHP <=0f)
        {
            Die();
        }
    }

    public void DealDamage()
    {
        if (hitSound !=null && audioSource !=null)
        {
            audioSource.volume = 0.1f;
            audioSource.PlayOneShot(hitSound);
        }

        Unit target = FindTargetInRange();
        if (target !=null)
        {
            target.TakeDamage(damage);
        }
        else
        {
            Base baseTarget = FindBaseInRange();
            if (baseTarget != null)
            {
                baseTarget.TakeDamage(damage);
            }
        }
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
        foreach (var hit in hits)
        {
            Unit other = hit.GetComponent<Unit>();
            if (other != null && other != this && other.isPlayerUnit != this.isPlayerUnit)
            {
                return other; // нашли врага - возвращаем его
            }
        }
        return null;
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

}
