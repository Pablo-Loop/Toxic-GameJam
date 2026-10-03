// UIManager.cs
// Controla todo lo visual del HUD.
// Recibe información de otros sistemas y actualiza la UI.
// No toma decisiones de juego — solo muestra información.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PiroBros.Managers
{
    public class UIManager : MonoBehaviour
    {
        // ─── SINGLETON ───
        public static UIManager Instance { get; private set; }

        [Header("Vida")]
        [SerializeField] private Image healthBarFill;
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Vidas")]
        [SerializeField] private TextMeshProUGUI livesText;

        [Header("Personaje")]
        [SerializeField] private TextMeshProUGUI characterNameText;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Button restartButton;

        [Header("Victoria")]
        [SerializeField] private GameObject victoryPanel;

        [Header("Selección de Personaje")]
        [SerializeField] private GameObject characterSelectPanel;
        [SerializeField] private Button[] characterButtons;

        [Header("Habilidad")]
        [SerializeField] private TextMeshProUGUI abilityUsesText;
        [SerializeField] private TextMeshProUGUI abilityCooldownText;
        [SerializeField] private TextMeshProUGUI abilityMessageText;

        // ────────────────────────────────────────────────────────────────────
        // UNITY
        // ────────────────────────────────────────────────────────────────────

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            // Persistir entre escenas igual que el GameManager
            DontDestroyOnLoad(gameObject);

            if (restartButton != null)
                restartButton.onClick.AddListener(OnRestartClicked);

            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (victoryPanel != null)
                victoryPanel.SetActive(false);
        }

        private void Start()
        {
            // Limpiar listeners anteriores para evitar acumulación
            for (int i = 0; i < characterButtons.Length; i++)
            {
                characterButtons[i].onClick.RemoveAllListeners();
                int index = i;
                characterButtons[i].onClick.AddListener(() => OnCharacterSelected(index));
            }

            // Ocultar todo excepto la selección de personaje
            HideGameOver();
            HideVictory();
            ShowCharacterSelect();
        }

        // ────────────────────────────────────────────────────────────────────
        // SELECCIÓN DE PERSONAJE
        // ────────────────────────────────────────────────────────────────────

        private void OnCharacterSelected(int index)
        {
            if (characterSelectPanel != null)
                characterSelectPanel.SetActive(false);

            Managers.GameManager.Instance.StartGame(index);
        }

        public void ShowCharacterSelect()
        {
            if (characterSelectPanel != null)
                characterSelectPanel.SetActive(true);

            // Ocultar otros paneles durante la selección
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (victoryPanel != null)
                victoryPanel.SetActive(false);
        }

        // ────────────────────────────────────────────────────────────────────
        // MÉTODOS PÚBLICOS
        // ────────────────────────────────────────────────────────────────────

        // Actualiza la barra de vida visual
        public void UpdateHealthBar(int current, int max)
        {
            if (healthBarFill != null)
                healthBarFill.fillAmount = (float)current / max;

            if (healthText != null)
                healthText.text = $"{current} / {max}";
        }

        // Actualiza el contador de vidas
        public void UpdateLives(int lives)
        {
            if (livesText != null)
                livesText.text = $"Vidas: {lives}";
        }

        // Muestra el nombre del personaje activo
        public void UpdateCharacterName(string name)
        {
            if (characterNameText != null)
                characterNameText.text = name;
        }

        // Muestra el panel de Game Over
        public void ShowGameOver()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);
        }

        // Oculta el panel de Game Over
        public void HideGameOver()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }

        // Muestra el panel de Victoria
        public void ShowVictory()
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(true);
        }

        // Oculta el panel de Victoria
        public void HideVictory()
        {
            if (victoryPanel != null)
                victoryPanel.SetActive(false);
        }

        public void UpdateAbility(int remainingUses, float cooldownLeft)
        {
            if (abilityUsesText != null)
                abilityUsesText.text = $"Habilidad: {remainingUses} usos";

            if (abilityCooldownText != null)
            {
                if (cooldownLeft <= 0f) 
                    abilityCooldownText.text = "Listo";
                else
                    abilityCooldownText.text = $"Listo en: {Mathf.CeilToInt(cooldownLeft)}s";
            }
        }

        public void ShowAbilityMessage(string message)
        {
            if (abilityMessageText != null)
            {
                abilityMessageText.text = message;
                // Ocultar el mensaje después de 2 segundos
                CancelInvoke(nameof(HideAbilityMessage));
                Invoke(nameof(HideAbilityMessage), 2f);
            }
        }

        private void HideAbilityMessage()
        {
            if (abilityMessageText != null)
                abilityMessageText.text = "";
        }

        // ────────────────────────────────────────────────────────────────────
        // EVENTOS
        // ────────────────────────────────────────────────────────────────────

        private void OnRestartClicked()
        {
            // Ocultar paneles
            HideGameOver();
            HideVictory();

            // Mostrar selección de personaje para empezar limpio
            ShowCharacterSelect();

            // Resetear el estado del GameManager
            GameManager.Instance.ResetGame();
        }
    }
}