using UnityEngine;

public class ShooterUnit : Unit
{
    public GameObject projectilePrefab;
    public Transform throwPoint;

    private Transform targetForShot; // Храним текущую цель для броска
    protected override void Attack(Unit target)
    {
        targetForShot = target.transform;
        GetComponent<Animator>().SetTrigger("Attack");
    }

    protected override void AttackBase(Base baseTarget)
    {
        targetForShot = baseTarget.transform;
        GetComponent<Animator>().SetTrigger("Attack");
    }

    public void ThrowProjectile()
    {
        if (targetForShot == null) return;

        GameObject projectile = Instantiate(projectilePrefab, throwPoint.position, Quaternion.identity);
        var proj = projectile.GetComponent<Projectile>();
        if (proj !=null)
        {
            proj.Init(targetForShot, damage);
        }
    }
}
