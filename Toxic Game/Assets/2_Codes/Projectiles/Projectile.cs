// Projectile.cs
// Representa cualquier proyectil del juego.
// Recibe daño y dirección al ser disparado, viaja en línea recta
// y aplica daño a lo primero que toca que sea IDamageable.

using UnityEngine;
using PiroBros.Interfaces;

namespace PiroBros.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float lifetime = 3f; // segundos antes de destruirse solo

        private int damage;
        private Rigidbody2D rb;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f; // las balas no caen por gravedad
        }

        // Llamado por el arma justo después de instanciar el proyectil.
        // Le dice cuánto daño hace y a qué velocidad viaja.
        public void Initialize(int projectileDamage, Vector2 direction, float speed)
        {
            damage = projectileDamage;
            rb.linearVelocity = direction * speed;

            // Auto-destruirse después de X segundos para no llenar la escena
            Destroy(gameObject, lifetime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Ignorar colisión con otros proyectiles
            if (other.CompareTag("Projectile")) return;

            IDamageable damageable = other.GetComponent<IDamageable>();

            if (damageable != null)
            {
                damageable.TakeDamage(damage);
                Destroy(gameObject);
                return;
            }

            // Destruirse contra el suelo u objetos sólidos
            if (other.CompareTag("Ground"))
                Destroy(gameObject);
        }
    }
}
