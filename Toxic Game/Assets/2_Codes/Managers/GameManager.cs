// GameManager.cs
// Cerebro del juego. Persiste entre escenas usando DontDestroyOnLoad.
// Maneja vidas, personajes, niveles e intentos en Nivel 2.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using PiroBros.Core;

namespace PiroBros.Managers
{
    public class GameManager : MonoBehaviour
    {
        // ─── SINGLETON ───
        public static GameManager Instance { get; private set; }

        [Header("Configuración")]
        [SerializeField] private int startingLives = 3;

        [Header("Prefabs de Personajes")]
        [SerializeField] private Core.Player[] characterPrefabs;

        [Header("Cámara")]
        [SerializeField] private Camera mainCamera;

        // ─── ESTADO ───
        private bool isGameActive = false;
        private int currentLives;
        private int level2Attempts = 0;
        private Core.Player activeCharacter;
        private CameraFollow cameraFollow;

        // Pool de instancias creadas en runtime
        private List<Core.Player> characterPool = new List<Core.Player>();
        // Todos los personajes instanciados
        private List<Core.Player> allInstances = new List<Core.Player>();

        // ─── ESCENAS ───
        private const string SCENE_LEVEL1 = "Nivel1";
        private const string SCENE_LEVEL2 = "Nivel2";

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
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Buscar cámara en la nueva escena
            mainCamera = Camera.main;
            if (mainCamera != null)
                cameraFollow = mainCamera.GetComponent<CameraFollow>();

            // Si hay personajes instanciados, reposicionarlos al spawn
            if (allInstances.Count > 0)
                RepositionCharacters();
        }

        // ────────────────────────────────────────────────────────────────────
        // INICIO DEL JUEGO
        // ────────────────────────────────────────────────────────────────────

        public void StartGame(int characterIndex)
        {
            isGameActive = true;
            currentLives = startingLives;
            level2Attempts = 0;

            // Destruir instancias anteriores si existen
            foreach (Core.Player p in allInstances)
                if (p != null) Destroy(p.gameObject);

            allInstances.Clear();
            characterPool.Clear();

            // Instanciar todos los personajes desde prefabs
            for (int i = 0; i < characterPrefabs.Length; i++)
            {
                Core.Player instance = Instantiate(characterPrefabs[i]);
                DontDestroyOnLoad(instance.gameObject);
                instance.gameObject.SetActive(false);
                allInstances.Add(instance);

                // Todos al pool excepto el elegido
                if (i != characterIndex)
                    characterPool.Add(instance);
            }

            // Activar el personaje elegido en el spawn point
            Vector3 spawnPos = GetSpawnPosition();
            allInstances[characterIndex].transform.position = spawnPos;
            ActivateCharacter(allInstances[characterIndex]);

            UpdateUI();
        }

        // ────────────────────────────────────────────────────────────────────
        // CUANDO EL JUGADOR RECIBE DAÑO
        // ────────────────────────────────────────────────────────────────────

        public void OnPlayerHit()
        {
            // Si el juego no está activo ignorar completamente
            if (!isGameActive) return;

            currentLives--;
            UpdateUI();

            if (currentLives <= 0)
            {
                isGameActive = false;
                HandleNoLives();
                return;
            }

            StartCoroutine(RespawnCurrentCharacter());
        }

        private void HandleNoLives()
        {
            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == SCENE_LEVEL1)
            {
                // Mostrar Game Over y esperar al jugador
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowGameOver();
            }
            else if (currentScene == SCENE_LEVEL2)
            {
                level2Attempts++;

                if (level2Attempts >= 2)
                {
                    // Game Over definitivo — volver a Nivel 1
                    level2Attempts = 0;
                    if (UIManager.Instance != null)
                        UIManager.Instance.ShowGameOver();
                }
                else
                {
                    // Primer intento fallido — mostrar panel y esperar
                    if (UIManager.Instance != null)
                        UIManager.Instance.ShowGameOver();
                }
            }
        }

        private IEnumerator RespawnCurrentCharacter()
        {
            yield return new WaitForSeconds(0.5f);
            activeCharacter.Revive();
            UpdateUI();
        }

        // ────────────────────────────────────────────────────────────────────
        // PICKUP DE VIDA
        // ────────────────────────────────────────────────────────────────────

        public void OnLifePickup()
        {
            if (!isGameActive) return;

            currentLives++;

            Vector3 currentPosition = activeCharacter.transform.position;

            Core.Player previous = activeCharacter;
            previous.gameObject.SetActive(false);
            characterPool.Add(previous);

            int randomIndex = Random.Range(0, characterPool.Count);
            Core.Player next = characterPool[randomIndex];
            characterPool.RemoveAt(randomIndex);

            next.transform.position = currentPosition;
            ActivateCharacter(next);
            UpdateUI();
        }

        // ────────────────────────────────────────────────────────────────────
        // BANDERA - SIGUIENTE NIVEL
        // ────────────────────────────────────────────────────────────────────

        public void OnFlagReached()
        {
            if (!isGameActive) return;

            string currentScene = SceneManager.GetActiveScene().name;

            if (currentScene == SCENE_LEVEL1)
                StartCoroutine(LoadNextLevel    (SCENE_LEVEL2));
            else if (currentScene == SCENE_LEVEL2)
            {
                isGameActive = false;
                if (UIManager.Instance != null)
                    UIManager.Instance.ShowVictory();
            }
        }

        private IEnumerator LoadNextLevel(string sceneName)
        {
            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(sceneName);
        }

        // ────────────────────────────────────────────────────────────────────
        // RESET — llamado por el botón Reiniciar
        // ────────────────────────────────────────────────────────────────────

        public void ResetGame()
        {
            isGameActive = false;

            foreach (Core.Player p in allInstances)
                if (p != null) Destroy(p.gameObject);

            allInstances.Clear();
            characterPool.Clear();
            activeCharacter = null;
            currentLives = startingLives;

            // Si falló dos veces en Nivel 2 volver al Nivel 1
            // Si falló en Nivel 1 o primera vez en Nivel 2 recargar escena actual
            string targetScene = level2Attempts == 0 ? SCENE_LEVEL1 : SCENE_LEVEL2;

            SceneManager.LoadScene(targetScene);
        }

        // ────────────────────────────────────────────────────────────────────
        // HELPERS
        // ────────────────────────────────────────────────────────────────────

        private void ActivateCharacter(Core.Player character)
        {
            activeCharacter = character;
            activeCharacter.gameObject.SetActive(true);
            activeCharacter.Revive();

            if (cameraFollow != null)
                cameraFollow.SetTarget(activeCharacter.transform);

            activeCharacter.RefreshUI();
        }

        private Vector3 GetSpawnPosition()
        {
            GameObject spawn = GameObject.Find("PlayerSpawn");
            if (spawn != null)
                return spawn.transform.position;

            Debug.LogWarning("GameManager: no se encontró PlayerSpawn en la escena.");
            return new Vector3(0, 0, 0);
        }

        private void RepositionCharacters()
        {
            Vector3 spawnPos = GetSpawnPosition();

            foreach (Core.Player p in allInstances)
                if (p != null)
                    p.transform.position = spawnPos;

            if (activeCharacter != null)
            {
                activeCharacter.gameObject.SetActive(true);
                activeCharacter.Revive();

                if (cameraFollow != null)
                    cameraFollow.SetTarget(activeCharacter.transform);

                activeCharacter.RefreshUI();
            }
        }

        private void UpdateUI()
        {
            if (UIManager.Instance != null)
                UIManager.Instance.UpdateLives(currentLives);
        }

        // ─── HABILIDAD ───
        private float lastAbilityUseTime = -999f;
        public float LastAbilityUseTime => lastAbilityUseTime;

        public void RegisterAbilityUse()
        {
            lastAbilityUseTime = Time.time;
        }

        public float AbilityCooldownLeft =>
            Mathf.Max(0f, (lastAbilityUseTime + 40f) - Time.time);

        public bool AbilityOnCooldown =>
            Time.time < lastAbilityUseTime + 40f;

        // ────────────────────────────────────────────────────────────────────
        // PROPIEDADES
        // ────────────────────────────────────────────────────────────────────

        public int CurrentLives => currentLives;
        public Core.Player ActiveCharacter => activeCharacter;
        public bool IsGameActive => isGameActive;
    }
}