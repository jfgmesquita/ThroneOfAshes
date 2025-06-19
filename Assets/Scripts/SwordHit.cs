using UnityEngine;

public class SwordHit : MonoBehaviour
{
    public WeaponController wc;
    public GameObject HitParticle;
    public Transform HitPoint;
    public int SwordDamage = 10;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && wc.isAttacking)
        {
            GameObject particle = Instantiate(HitParticle, HitPoint.position, HitPoint.rotation);
            Destroy(particle, 1.0f);

            if (other.TryGetComponent<SoldierController>(out var enemy))
            {
                enemy.TakeDamage(SwordDamage);
            }

            if (other.TryGetComponent<AshWraithController>(out var wraitch))
            {
                wraitch.TakeDamage(SwordDamage);
            }
        }
    }
}
