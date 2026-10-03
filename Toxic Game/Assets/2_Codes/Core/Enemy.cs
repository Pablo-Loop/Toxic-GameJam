// Enemy.cs
// Clase base abstracta para todos los enemigos del juego.
// Maneja vida, movimiento hacia el jugador, daño y muerte.
// Diablito hereda de aquí — si en el futuro hay más enemigos,
// también heredarán sin repetir esta lógica.

using UnityEngine;
using PiroBros.Interfaces;

namespace PiroBros.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public abstract class Enemy : MonoBehaviour, IDamageable
    {
        // ─── STATS ───
        [Header("Estadísticas")]
        [SerializeField] protected int maxHealth = 5;
        [SerializeField] protected float moveSpeed = 2.5f;
        [SerializeField] protected int attackDamage = 10;

        // ─── IA ───
        [Header("IA")]
        [SerializeField] protected float detectionRange = 8f;  // rango para detectar al jugador
        [SerializeField] protected float attackRange = 1f;     // rango para atacar
        [SerializeField] protected float attackCooldown = 1f;  // segundos entre ataques

        // ─── ESTADO ───
        protected int currentHealth;
        protected bool isAlive = true;
        protected float lastAttackTime = -999f;

        // ─── REFERENCIAS ───
        protected Rigidbody2D rb;
        protected Animator animator;
        protected Transform playerTarget; // referencia al jugador más cercano

        // ────────────────────────────────────────────────────────────────────
        // UNITY
        // ────────────────────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            currentHealth = maxHealth;

            Initialize();
        }

        protected virtual void Update()
        {
            if (!isAlive) return;

            // Si el juego no está activo, detener toda la IA
            if (Managers.GameManager.Instance != null && !Managers.GameManager.Instance.IsGameActive) return;

            FindPlayer();
            HandleAI();
            HandleFlip();
        }

        // ────────────────────────────────────────────────────────────────────
        // ABSTRACTO — cada enemigo define sus propios valores
        // ────────────────────────────────────────────────────────────────────

        protected abstract void Initialize();

        // ────────────────────────────────────────────────────────────────────
        // IA
        // ────────────────────────────────────────────────────────────────────

        // Busca al jugador activo en la escena
        protected void FindPlayer()
        {
            // Buscar el jugador activo usando la tag "Player"
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                playerTarget = playerObj.transform;
        }

        private void HandleAI()
        {
            if (playerTarget == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, playerTarget.position);

            if (distanceToPlayer <= attackRange)
            {
                // Está cerca — atacar
                StopMoving();
                TryAttack();
            }
            else if (distanceToPlayer <= detectionRange)
            {
                // Está en rango de detección — caminar hacia él
                MoveTowardsPlayer();
            }
            else
            {
                // Fuera de rango — quedarse quieto
                StopMoving();
            }
        }

        private void MoveTowardsPlayer()
        {
            float direction = playerTarget.position.x > transform.position.x ? 1f : -1f;
            rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);

            if (animator != null)
                animator.SetBool("IsWalking", true);
        }

        private void StopMoving()
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

            if (animator != null)
                animator.SetBool("IsWalking", false);
        }

        private void TryAttack()
        {
            if (Time.time < lastAttackTime + attackCooldown) return;

            lastAttackTime = Time.time;
            Attack();
        }

        // El ataque puede sobreescribirse en cada enemigo hijo
        protected virtual void Attack()
        {
            if (playerTarget == null) return;

            IDamageable damageable = playerTarget.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(attackDamage);

            // Aplicar knockback si el objetivo es un Player
            PiroBros.Core.Player player = playerTarget.GetComponent<PiroBros.Core.Player>();
            if (player != null)
                player.ApplyKnockback(transform.position);

            if (animator != null)
                animator.SetTrigger("Attack");
        }

        // ────────────────────────────────────────────────────────────────────
        // FLIP
        // ────────────────────────────────────────────────────────────────────

        private void HandleFlip()
        {
            if (playerTarget == null) return;

            // Mirar hacia el jugador
            bool playerIsToTheRight = playerTarget.position.x > transform.position.x;
            Vector3 scale = transform.localScale;

            if (playerIsToTheRight && scale.x < 0)
                scale.x *= -1;
            else if (!playerIsToTheRight && scale.x > 0)
                scale.x *= -1;

            transform.localScale = scale;
        }

        // ────────────────────────────────────────────────────────────────────
        // IDAMAGEABLE
        // ────────────────────────────────────────────────────────────────────

        public virtual void TakeDamage(int damage)
        {
            if (!isAlive) return;

            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

            if (animator != null)
                animator.SetTrigger("Hit");

            if (currentHealth <= 0)
                Die();
        }

        public virtual void Die()
        {
            isAlive = false;
            rb.linearVelocity = Vector2.zero;

            // Desactivar el collider para que no siga recibiendo daño
            GetComponent<Collider2D>().enabled = false;

            if (animator != null)
                animator.SetTrigger("Die");

            Destroy(gameObject, 1f);
        }

        // ────────────────────────────────────────────────────────────────────
        // PROPIEDADES
        // ────────────────────────────────────────────────────────────────────

        public bool IsAlive => isAlive;

        // ────────────────────────────────────────────────────────────────────
        // GIZMOS — visualizar rangos en el editor
        // ────────────────────────────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            // Rango de detección en amarillo
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            // Rango de ataque en rojo
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }
}
