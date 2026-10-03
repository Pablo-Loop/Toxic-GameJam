// Spikes.cs
// Trampa estática que quita una vida al jugador.
// Respeta el sistema de invencibilidad temporal.

using UnityEngine;
using PiroBros.Interfaces;

namespace PiroBros.Core
{
    public class Spikes : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(1);

            // Aplicar knockback hacia arriba para sacar al jugador de las púas
            Player player = other.GetComponent<Player>();
            if (player != null)
                player.ApplyKnockback(transform.position);
        }
    }
}
