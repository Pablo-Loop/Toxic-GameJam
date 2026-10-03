using UnityEngine;
using PiroBros.Interfaces;

namespace PiroBros.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Grenade : MonoBehaviour
    {
        [SerializeField] private float explosionRadius = 4f;
        [SerializeField] private float fuseTime = 2f;

        private Rigidbody2D rb;
        private bool initialized = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 1f;
        }

        public void Initialize(Vector2 throwVelocity)
        {
            rb.linearVelocity = throwVelocity;
            initialized = true;
            Invoke(nameof(Explode), fuseTime);
        }

        // Si por alguna razón Initialize no se llamó, explotar igual
        private void Start()
        {
            if (!initialized)
                Invoke(nameof(Explode), fuseTime);
        }

        private void Explode()
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                explosionRadius
            );

            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Player")) continue;

                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null)
                    damageable.TakeDamage(999);
            }

            Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, explosionRadius);
        }
    }
}