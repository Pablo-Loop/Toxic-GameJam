// DiabloSicario.cs
// Enemigo estático que dispara proyectiles hacia el jugador.
// No se mueve nunca — solo detecta, apunta y dispara.
// Hereda de Enemy pero sobreescribe la IA para ignorar el movimiento.

using UnityEngine;
using PiroBros.Interfaces;

namespace PiroBros.Enemies
{
    public class DiabloSicario : Core.Enemy
    {
        [Header("Detección DiabloSicario")]
        [SerializeField] private float horizontalRange = 10f;  // rango horizontal de detección
        [SerializeField] private float verticalTolerance = 1.5f; // diferencia vertical máxima

        [Header("Disparo")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform firePoint;
        [SerializeField] private float projectileSpeed = 12f;

        // ────────────────────────────────────────────────────────────────────
        // INICIALIZACIÓN
        // ────────────────────────────────────────────────────────────────────

        protected override void Initialize()
        {
            maxHealth = 1;
            attackDamage = 1;    // quita una vida completa al contacto
            attackCooldown = 2f; // dispara cada 2 segundos
        }

        // ────────────────────────────────────────────────────────────────────
        // IA — sobreescribe el Update de Enemy para no moverse nunca
        // ────────────────────────────────────────────────────────────────────

        protected override void Update()
        {
            if (!isAlive) return;

            // Si el juego no está activo, detener toda la IA
            if (Managers.GameManager.Instance != null && !Managers.GameManager.Instance.IsGameActive) return;

            FindPlayer();
            HandleShootingAI();
            HandleFlipTowardsPlayer();
        }

        private void HandleShootingAI()
        {
            if (playerTarget == null) return;

            float horizontalDistance = Mathf.Abs(
                playerTarget.position.x - transform.position.x
            );

            float verticalDistance = Mathf.Abs(
                playerTarget.position.y - transform.position.y
            );

            // Solo dispara si el jugador está en rango horizontal
            // Y aproximadamente a la misma altura
            bool inRange = horizontalDistance <= horizontalRange;
            bool sameHeight = verticalDistance <= verticalTolerance;

            if (inRange && sameHeight)
                TryShoot();
        }

        // ────────────────────────────────────────────────────────────────────
        // DISPARO
        // ────────────────────────────────────────────────────────────────────

        private void TryShoot()
        {
            if (Time.time < lastAttackTime + attackCooldown) return;

            lastAttackTime = Time.time;
            Shoot();
        }

        private void Shoot()
        {
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogWarning("DiabloSicario: falta projectilePrefab o firePoint.");
                return;
            }

            // Determinar dirección según posición del jugador
            Vector2 direction = playerTarget.position.x > transform.position.x
                ? Vector2.right
                : Vector2.left;

            GameObject obj = Instantiate(
                projectilePrefab,
                firePoint.position,
                Quaternion.identity
            );

            PiroBros.Projectiles.Projectile projectile =
                obj.GetComponent<PiroBros.Projectiles.Projectile>();

            if (projectile != null)
                projectile.Initialize(attackDamage, direction, projectileSpeed);

            if (animator != null)
                animator.SetTrigger("Attack");
        }

        // ────────────────────────────────────────────────────────────────────
        // FLIP — mirar hacia el jugador sin moverse
        // ────────────────────────────────────────────────────────────────────

        private void HandleFlipTowardsPlayer()
        {
            if (playerTarget == null) return;

            bool playerIsToTheRight = playerTarget.position.x > transform.position.x;
            Vector3 scale = transform.localScale;

            if (playerIsToTheRight && scale.x < 0)
                scale.x *= -1;
            else if (!playerIsToTheRight && scale.x > 0)
                scale.x *= -1;

            transform.localScale = scale;
        }

        // ────────────────────────────────────────────────────────────────────
        // GIZMOS — visualizar rangos en el editor
        // ────────────────────────────────────────────────────────────────────

        private new void OnDrawGizmosSelected()
        {
            // Rango horizontal en azul
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(
                transform.position,
                new Vector3(horizontalRange * 2, verticalTolerance * 2, 0)
            );
        }
    }
}
