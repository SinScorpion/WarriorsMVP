using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float speed = 5f;
    private float damage;
    private Unit attacker;

    public void Init(Transform target, float damage, Unit attacker)
    {
        this.target = target;
        this.damage = damage;
        this.attacker = attacker;
    }

    // Update is called once per frame
    void Update()
    {
        if(target == null) { Destroy(gameObject); return; }

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.15f)
        {
            var unit = target.GetComponent<Unit>();
            if (unit !=null)
            {
                unit.TakeDamage(damage);
            }
            else
            {
                var baseTarget = target.GetComponent<Base>();
                if (baseTarget !=null)
                {
                    baseTarget.TakeDamage(damage);

                    if (attacker !=null && attacker.hitSound !=null && attacker.audioSource !=null)
                    {
                        attacker.audioSource.volume = 0.1f;
                        attacker.audioSource.PlayOneShot(attacker.hitSound);
                    }
                }
            }
            Destroy(gameObject);
        }
    }
}
