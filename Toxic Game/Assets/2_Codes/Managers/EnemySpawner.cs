// EnemySpawner.cs
// Ahora maneja dos tipos de enemigos:
// Diablito — se mueve hacia el jugador
// DiabloSicario — estático, dispara desde lejos

using UnityEngine;

namespace PiroBros.Managers
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private GameObject diablitoPrefab;
        [SerializeField] private GameObject diabloSicarioPrefab;

        [Header("Puntos de Spawn")]
        [SerializeField] private Transform[] spawnPoints;

        [Header("Configuración")]
        [SerializeField] private float spawnInterval = 4f;
        [SerializeField] private int maxEnemies = 8;

        // Probabilidad de que aparezca un DiabloSicario (0 a 1)
        [SerializeField] private float sicarioProbability = 0.3f;

        private float lastSpawnTime = -999f;

        private void Update()
        {
            if (Time.time < lastSpawnTime + spawnInterval) return;
            if (CountEnemiesInScene() >= maxEnemies) return;

            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            if (spawnPoints.Length == 0) return;

            int index = Random.Range(0, spawnPoints.Length);
            Transform spawnPoint = spawnPoints[index];

            // Decidir qué tipo de enemigo spawnear
            bool spawnSicario = Random.value <= sicarioProbability;

            GameObject prefab = spawnSicario ? diabloSicarioPrefab : diablitoPrefab;

            if (prefab == null)
            {
                Debug.LogWarning("EnemySpawner: prefab no asignado.");
                return;
            }

            Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            lastSpawnTime = Time.time;
        }

        private int CountEnemiesInScene()
        {
            int diablitos = FindObjectsByType<Enemies.Diablito>(FindObjectsSortMode.None).Length;
            int sicarios = FindObjectsByType<Enemies.DiabloSicario>(FindObjectsSortMode.None).Length;
            return diablitos + sicarios;
        }
    }
}