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



        [Header("Toxicidad")]

        [SerializeField] private Image toxicityBarFill;

        [SerializeField] private TextMeshProUGUI toxicityText;



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



        [Header("Controles Móviles")]

        [SerializeField] private GameObject mobileControlsPanel; // Panel contenedor de botones táctiles / joystick



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




            if (restartButton != null)

                restartButton.onClick.AddListener(OnRestartClicked);



            if (gameOverPanel != null)

                gameOverPanel.SetActive(false);



            if (victoryPanel != null)

                victoryPanel.SetActive(false);



            if (mobileControlsPanel != null)

                mobileControlsPanel.SetActive(false);

        }



        private void Start()
        {
            // Asignar eventos de botones de personaje
            for (int i = 0; i < characterButtons.Length; i++)
            {
                characterButtons[i].onClick.RemoveAllListeners();
                int index = i;
                characterButtons[i].onClick.AddListener(() => OnCharacterSelected(index));
            }

            HideGameOver();
            HideVictory();

            // El panel de personaje inicia OCULTO mientras el Lore se muestra
            if (characterSelectPanel != null)
                characterSelectPanel.SetActive(false);
        }



        // ────────────────────────────────────────────────────────────────────

        // SELECCIÓN DE PERSONAJE

        // ────────────────────────────────────────────────────────────────────



        private void OnCharacterSelected(int index)

        {

            if (characterSelectPanel != null)

                characterSelectPanel.SetActive(false);



            // Mostrar botones y joystick táctiles al iniciar gameplay

            if (mobileControlsPanel != null)

                mobileControlsPanel.SetActive(true);



            Managers.GameManager.Instance.StartGame(index);

        }



        public void ShowCharacterSelect()

        {

            if (characterSelectPanel != null)

                characterSelectPanel.SetActive(true);



            // Ocultar controles táctiles y otros paneles durante la selección

            if (mobileControlsPanel != null)

                mobileControlsPanel.SetActive(false);



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

                livesText.text = $"Lives: {lives}";

        }



        // Actualiza la barra de toxicidad

        public void UpdateToxicity(float current, float max)

        {

            if (toxicityBarFill != null)

                toxicityBarFill.fillAmount = current / max;



            if (toxicityText != null)

                toxicityText.text = $"{Mathf.CeilToInt(current)}%";

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



            if (mobileControlsPanel != null)

                mobileControlsPanel.SetActive(false);

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



            if (mobileControlsPanel != null)

                mobileControlsPanel.SetActive(false);

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

                abilityUsesText.text = $"Ability: {remainingUses}";



            if (abilityCooldownText != null)

            {

                if (cooldownLeft <= 0f)

                    abilityCooldownText.text = "Ready";

                else

                    abilityCooldownText.text = $"Couldown: {Mathf.CeilToInt(cooldownLeft)}s";

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



            // Mostrar selección de personaje para empezar limpio (esto también ocultará el panel táctil)

            ShowCharacterSelect();



            // Resetear el estado del GameManager

            GameManager.Instance.ResetGame();

        }

    }

}

