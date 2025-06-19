using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
    public GameObject impactEffect;
    public int fireDamage = 20;

    void Start()
    {
        Destroy(gameObject, 1.5f);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (impactEffect != null)
        {
            GameObject impact = Instantiate(impactEffect, transform.position, Quaternion.identity);
            Destroy(impact, 1f);
        }

        Destroy(gameObject);

        if (collision.gameObject.TryGetComponent<SoldierController>(out var enemy))
        {
            enemy.TakeDamage(fireDamage);
        }

        if (collision.gameObject.TryGetComponent<AshWraithController>(out var wraitch))
        {
            wraitch.TakeDamage(fireDamage);
        }
    }
}
