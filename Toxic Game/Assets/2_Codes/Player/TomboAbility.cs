using UnityEngine;
using System.Collections;
using PiroBros.Interfaces;

namespace PiroBros.Player
{
    public class TomboAbility : Core.Ability
    {
        [SerializeField] private float invincibilityDuration = 8f;
        [SerializeField] private int contactDamage = 999;

        private bool isActive = false;
        private Core.Player parentPlayer;

        protected override void Initialize()
        {
            maxUses = 2;
        }

        private void Start()
        {
            parentPlayer = GetComponentInParent<Core.Player>();
        }

        protected override void Activate()
        {
            StartCoroutine(StarPowerRoutine());
        }

        private IEnumerator StarPowerRoutine()
        {
            isActive = true;

            // Activar invencibilidad real en el Player
            if (parentPlayer != null)
                parentPlayer.SetAbilityInvincible(true);

            // Efecto visual — parpadeo amarillo
            SpriteRenderer sr = GetComponentInParent<SpriteRenderer>();
            Color originalColor = sr != null ? sr.color : Color.white;

            float elapsed = 0f;
            while (elapsed < invincibilityDuration)
            {
                if (sr != null)
                    sr.color = elapsed % 0.2f < 0.1f ? Color.yellow : Color.white;

                elapsed += Time.deltaTime;
                yield return null;
            }

            // Restaurar
            if (sr != null) sr.color = originalColor;

            // Desactivar invencibilidad
            if (parentPlayer != null)
                parentPlayer.SetAbilityInvincible(false);

            isActive = false;
        }

        public void OnContactWithEnemy(GameObject enemy)
        {
            if (!isActive) return;

            IDamageable damageable = enemy.GetComponent<IDamageable>();
            if (damageable != null)
                damageable.TakeDamage(contactDamage);
        }

        public bool IsActive => isActive;
    }
}