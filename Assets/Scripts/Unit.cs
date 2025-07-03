using UnityEngine;

public class Unit : MonoBehaviour
{
    public bool isPlayerUnit = false;
    public float moveSpeed = 1f;
    public float maxHP = 10f;
    public float currentHP = 10f;
    public int rewardGold = 0;

    public Vector2 moveDirection = Vector2.right;
    //�����
    public float damage = 2f;
    public float attackRange = 0.1f;
    public float attackCooldown = 1.0f;

    private float attackTimer = 0f; // ���������� ������ ��� ����������� 

    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        attackTimer -= Time.deltaTime; // ��������� ������ ������ ����

        Unit target = FindTargetInRange(); // ���� ����� � �������

        if (target != null)
        {
            if (attackTimer <= 0f)
            {
                Attack(target);
                attackTimer = attackCooldown; // ����� �����������
            }
            // ���� ���� ���� - �� ���������!
        }
        else
        {
            // ���� ������ ��� - ���� ����
            Base targetBase = FindBaseInRange();
            if (targetBase !=null)
            {
                if (attackTimer <= 0f)
                {
                    AttackBase(targetBase);
                    attackTimer = attackCooldown; // ����� �����������
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
        target.TakeDamage(damage); // ������� ���� �����
        // ����� ������� �������� � ����
    }

    void AttackBase(Base targetBase)
    {
        targetBase.TakeDamage(damage);
    }

    //���� ����� �������� ���� �����
    public void TakeDamage(float amount)
    {
        currentHP -= amount;
        if (currentHP <=0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (!isPlayerUnit && GameManager.Instance !=null)
        {
            GameManager.Instance.AddGold(rewardGold);
        }

        Destroy(gameObject);
    }

    // ����� ���������� ����� � ������� �����
    Unit FindTargetInRange()
    {
        // ������� ��� ���������� � ������� attackRange
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        foreach (var hit in hits)
        {
            Unit other = hit.GetComponent<Unit>();
            if (other != null && other != this && other.isPlayerUnit != this.isPlayerUnit)
            {
                return other; // ����� ����� - ���������� ���
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
