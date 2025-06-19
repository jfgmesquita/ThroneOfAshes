using UnityEngine;

public class WraithProjectile : MonoBehaviour
{
    public float speed = 15f;
    public int damage = 30;
    public float lifetime = 3f;
    private Vector3 dir;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector3 direction)
    {
        dir = direction.normalized;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * dir;
    }

    void OnCollisionEnter(Collision collision)
    {
        PlayerHealth ph = collision.collider.GetComponentInParent<PlayerHealth>();
        if (ph != null)
        {
            ph.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
