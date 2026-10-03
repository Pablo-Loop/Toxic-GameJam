// Wolf.cs
// Lobo de Shakira. Avanza en línea recta y destruye
// a cualquier enemigo que toque.

using UnityEngine;
using PiroBros.Interfaces;

namespace PiroBros.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Wolf : MonoBehaviour
    {
        private float speed;
        private float direction;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        public void Initialize(float dir, float spd)
        {
            direction = dir;
            speed = spd;
            rb.linearVelocity = new Vector2(direction * speed, 0f);

            // Auto destruirse después de 5 segundos
            Destroy(gameObject, 5f);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignorar al jugador
            if (other.CompareTag("Player")) return;

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(999);
                // El lobo NO se destruye — sigue avanzando
            }
        }
    }
}
