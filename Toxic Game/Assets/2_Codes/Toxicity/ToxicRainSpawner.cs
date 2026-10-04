using UnityEngine;

namespace PiroBros.Toxicity
{
    public class ToxicRainSpawner : MonoBehaviour
    {
        [Header("Configuración de lluvia")]
        [SerializeField] private GameObject rainDropPrefab;
        [SerializeField] private float spawnInterval = 0.3f;
        [SerializeField] private float spawnWidth = 15f;

        private float timer;

        private void Update()
        {
            timer += Time.deltaTime;

            if (timer >= spawnInterval)
            {
                SpawnDrop();
                timer = 0f;
            }
        }

        private void SpawnDrop()
        {
            float randomX = Random.Range(
                transform.position.x - spawnWidth / 2f,
                transform.position.x + spawnWidth / 2f
            );

            Vector3 spawnPosition = new Vector3(
                randomX,
                transform.position.y,
                transform.position.z
            );

            Instantiate(rainDropPrefab, spawnPosition, Quaternion.identity);
        }
    }
}