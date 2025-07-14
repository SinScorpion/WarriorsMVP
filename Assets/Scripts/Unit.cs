using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isPlayerUnit = false;
    public float moveSpeed = 1f;
    public float maxHP = 10f;
    public float currentHP = 10f;
    public int rewardGold = 0;

    public Vector2 moveDirection = Vector2.right;

    //Атака
    public float damage = 2f;
    public float attackRange = 0.1f;
    public float attackCooldown = 1.0f;

    public HPBar hpBarPrefab; // Префаб HP-бара
    private HPBar hpBarInstance; // Его экземпляр

    private float attackTimer = 0f; // Внутренний таймер для перезарядки 

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;

        // Создаем HP-бар
        if (hpBarPrefab !=null)
        {
            hpBarInstance = Instantiate(hpBarPrefab, Vector3.zero, Quaternion.identity, GameObject.Find("Canvas").transform);
            hpBarInstance.target = this.transform;
            
            hpBarInstance.SetHP(currentHP, maxHP);
            hpBarInstance.UpdatePositionImmediate();
        }
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
            // Если врагов нет - ищем базу
            Base targetBase = FindBaseInRange();
            if (targetBase !=null)
            {
                if (attackTimer <= 0f)
                {
                    AttackBase(targetBase);
                    attackTimer = attackCooldown; // Сброс перезарядки
                }
            }
            else
            {
                transform.Translate(moveDirection * moveSpeed * Time.deltaTime);
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
            hpBarInstance.SetHP(currentHP, maxHP);
        }
        if (currentHP <=0f)
        {
            Die();
        }
    }

    public void DealDamage()
    {
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
