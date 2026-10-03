// Player.cs
// Clase base abstracta de todos los personajes jugables.
// NUNCA se instancia directamente — es una plantilla.

using UnityEngine;
using UnityEngine.InputSystem;
using PiroBros.Interfaces;

namespace PiroBros.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CapsuleCollider2D))]
    public abstract class Player : MonoBehaviour, IDamageable
    {
        // ─── STATS ───
        [Header("Estadísticas")]
        [SerializeField] protected float moveSpeed = 7f;
        [SerializeField] protected float jumpForce = 14f;

        // ─── ESTADO ───
        protected bool isAlive = true;
        protected bool isFacingRight = true;

        // ─── DETECCIÓN DE SUELO ───
        [Header("Detección de Suelo")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayer;
        protected bool isGrounded;

        // ─── INVENCIBILIDAD ───
        [Header("Combate")]
        [SerializeField] private float invincibilityDuration = 5f;
        private float lastDamageTime = -999f;
        private bool abilityInvincible = false;
        public bool IsInvincible =>
            Time.time < lastDamageTime + invincibilityDuration || abilityInvincible;

        // ─── REFERENCIAS ───
        protected Rigidbody2D rb;
        protected Animator animator;
        protected Weapon currentWeapon;
        protected Ability currentAbility;

        // ─── INPUT ───
        protected Vector2 moveInput;

        // ────────────────────────────────────────────────────────────────────
        // UNITY
        // ────────────────────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            Initialize();
        }

        protected virtual void Update()
        {
            if (!isAlive) return;
            CheckGround();
            HandleFlip();
            UpdateAnimator();
        }

        protected virtual void FixedUpdate()
        {
            if (!isAlive) return;
            HandleMovement();
        }

        // ────────────────────────────────────────────────────────────────────
        // ABSTRACTO
        // ────────────────────────────────────────────────────────────────────

        protected abstract void Initialize();

        public abstract string CharacterName { get; }

        // ────────────────────────────────────────────────────────────────────
        // MOVIMIENTO
        // ────────────────────────────────────────────────────────────────────

        private void HandleMovement()
        {
            rb.linearVelocity = new Vector2(moveInput.x * moveSpeed, rb.linearVelocity.y);
        }

        protected void Jump()
        {
            if (isGrounded)
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // ────────────────────────────────────────────────────────────────────
        // SUELO
        // ────────────────────────────────────────────────────────────────────

        private void CheckGround()
        {
            isGrounded = Physics2D.OverlapCircle(
                groundCheck.position,
                groundCheckRadius,
                groundLayer
            );
        }

        // ────────────────────────────────────────────────────────────────────
        // FLIP
        // ────────────────────────────────────────────────────────────────────

        private void HandleFlip()
        {
            if (moveInput.x > 0 && !isFacingRight) Flip();
            else if (moveInput.x < 0 && isFacingRight) Flip();
        }

        private void Flip()
        {
            isFacingRight = !isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

        // ────────────────────────────────────────────────────────────────────
        // ANIMACIONES
        // ────────────────────────────────────────────────────────────────────

        private void UpdateAnimator()
        {
            if (animator == null) return;
            animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
            animator.SetBool("IsGrounded", isGrounded);
        }

        // ────────────────────────────────────────────────────────────────────
        // COMBATE — IDamageable
        // ────────────────────────────────────────────────────────────────────

        public void TakeDamage(int damage)
        {
            if (IsInvincible || !isAlive) return;

            lastDamageTime = Time.time;

            Managers.GameManager.Instance.OnPlayerHit();

            StartCoroutine(InvincibilityBlink());
        }

        public void Die()
        {
            isAlive = false;
            rb.linearVelocity = Vector2.zero;

            if (animator != null)
                animator.SetTrigger("Die");
        }

        public void Revive()
        {
            isAlive = true;
            StartCoroutine(InvincibilityBlink());
        }

        // ────────────────────────────────────────────────────────────────────
        // INVENCIBILIDAD POR HABILIDAD
        // ────────────────────────────────────────────────────────────────────

        public void SetAbilityInvincible(bool value)
        {
            abilityInvincible = value;
        }

        // ────────────────────────────────────────────────────────────────────
        // KNOCKBACK
        // ────────────────────────────────────────────────────────────────────

        public void ApplyKnockback(Vector2 attackerPosition)
        {
            if (!isAlive) return;
            Vector2 direction = ((Vector2)transform.position - attackerPosition).normalized;
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * 5f, ForceMode2D.Impulse);
        }

        // ────────────────────────────────────────────────────────────────────
        // BLINK
        // ────────────────────────────────────────────────────────────────────

        private System.Collections.IEnumerator InvincibilityBlink()
        {
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr == null) yield break;

            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                sr.enabled = !sr.enabled;
                yield return new WaitForSeconds(0.1f);
                elapsed += 0.1f;
            }

            sr.enabled = true;
        }

        // ────────────────────────────────────────────────────────────────────
        // CONTACTO CON ENEMIGOS — Tombo sobreescribe esto
        // ────────────────────────────────────────────────────────────────────

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            // Base vacía — cada personaje decide qué hacer al tocar algo
        }

        // ────────────────────────────────────────────────────────────────────
        // INPUT
        // ────────────────────────────────────────────────────────────────────

        public void OnMove(InputValue value)
        {
            moveInput = value.Get<Vector2>();
        }

        public void OnJump(InputValue value)
        {
            if (value.isPressed) Jump();
        }

        public void OnFire(InputValue value)
        {
            if (value.isPressed && currentWeapon != null)
                currentWeapon.Attack();
        }

        public void OnAbility(InputValue value)
        {
            if (value.isPressed && currentAbility != null)
                currentAbility.TryActivate();
        }

        // ────────────────────────────────────────────────────────────────────
        // UI
        // ────────────────────────────────────────────────────────────────────

        protected virtual void OnHealthChanged(int current, int max) { }

        public void RefreshUI()
        {
            if (Managers.UIManager.Instance != null)
            {
                Managers.UIManager.Instance.UpdateCharacterName(CharacterName);

                // Refrescar UI de habilidad si existe
                if (currentAbility != null)
                    currentAbility.UpdateUI();
            }
        }

        // ────────────────────────────────────────────────────────────────────
        // PROPIEDADES
        // ────────────────────────────────────────────────────────────────────

        public bool IsAlive => isAlive;

        // ────────────────────────────────────────────────────────────────────
        // GIZMOS
        // ────────────────────────────────────────────────────────────────────

        private void OnDrawGizmosSelected()
        {
            if (groundCheck == null) return;
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}