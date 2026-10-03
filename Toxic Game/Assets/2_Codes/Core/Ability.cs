// Ability.cs
// Clase base abstracta para todas las habilidades de los personajes.
// Maneja el cooldown global, los usos restantes y el feedback a la UI.

using UnityEngine;

namespace PiroBros.Core
{
    public abstract class Ability : MonoBehaviour
    {
        // ─── CONFIGURACIÓN ───
        protected int maxUses;                          // usos totales por partida
        protected float cooldownDuration = 40f;        // segundos entre usos

        // ─── ESTADO ───
        private int remainingUses;
        private float lastUseTime = -999f;

        // ─── PROPIEDADES ───
        public int RemainingUses => remainingUses;
        public bool IsOnCooldown => Time.time < lastUseTime + cooldownDuration;
        public float CooldownTimeLeft => Mathf.Max(0f, (lastUseTime + cooldownDuration) - Time.time);

        // ────────────────────────────────────────────────────────────────────
        // INICIALIZACIÓN
        // ────────────────────────────────────────────────────────────────────

        protected virtual void Awake()
        {
            Initialize();
            remainingUses = maxUses;
        }

        // Cada habilidad define sus propios valores
        protected abstract void Initialize();

        // ────────────────────────────────────────────────────────────────────
        // USO DE LA HABILIDAD
        // ────────────────────────────────────────────────────────────────────

        public void TryActivate()
        {
            if (remainingUses <= 0)
            {
                ShowNoUsesMessage();
                return;
            }

            // Verificar cooldown global en el GameManager
            if (Managers.GameManager.Instance.AbilityOnCooldown)
            {
                ShowCooldownMessage();
                return;
            }

            remainingUses--;

            // Registrar el uso en el GameManager para que persista
            Managers.GameManager.Instance.RegisterAbilityUse();

            Activate();
            UpdateUI();
        }

        // Cada habilidad implementa su efecto aquí
        protected abstract void Activate();

        // ────────────────────────────────────────────────────────────────────
        // FEEDBACK
        // ────────────────────────────────────────────────────────────────────

        private void ShowNoUsesMessage()
        {
            if (Managers.UIManager.Instance != null)
                Managers.UIManager.Instance.ShowAbilityMessage("¡No quedan habilidades disponibles!");
        }

        private void ShowCooldownMessage()
        {
            float timeLeft = Managers.GameManager.Instance.AbilityCooldownLeft;
            if (Managers.UIManager.Instance != null)
                Managers.UIManager.Instance.ShowAbilityMessage(
                    $"Habilidad disponible en {Mathf.CeilToInt(timeLeft)}s"
                );
        }

        public void UpdateUI()
        {
            if (Managers.UIManager.Instance != null)
                Managers.UIManager.Instance.UpdateAbility(remainingUses, CooldownTimeLeft);
        }

        // ────────────────────────────────────────────────────────────────────
        // COOLDOWN COMPARTIDO ENTRE PERSONAJES
        // El cooldown vive en el GameManager para que persista
        // aunque cambies de personaje
        // ────────────────────────────────────────────────────────────────────

        public float LastUseTime => lastUseTime;

        public void SetLastUseTime(float time)
        {
            lastUseTime = time;
        }
    }
}
