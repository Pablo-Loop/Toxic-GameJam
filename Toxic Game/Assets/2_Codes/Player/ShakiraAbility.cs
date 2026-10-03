using UnityEngine;
using System.Collections;
using PiroBros.Projectiles;

namespace PiroBros.Player
{
    public class ShakiraAbility : Core.Ability
    {
        [SerializeField] private GameObject wolfPrefab;
        [SerializeField] private float wolfSpeed = 8f;

        protected override void Initialize()
        {
            maxUses = 3;
        }

        protected override void Activate()
        {
            if (wolfPrefab == null)
            {
                Debug.LogWarning("ShakiraAbility: falta wolfPrefab.");
                return;
            }

            float direction = transform.parent.localScale.x > 0 ? 1f : -1f;

            // Primer lobo — altura normal
            SpawnWolf(direction, 0f);

            // Segundo lobo con pequeño delay y offset vertical
            StartCoroutine(SpawnSecondWolf(direction));
        }

        private IEnumerator SpawnSecondWolf(float direction)
        {
            yield return new WaitForSeconds(0.15f);
            SpawnWolf(direction, 0.8f);
        }

        private void SpawnWolf(float direction, float verticalOffset)
        {
            Vector3 spawnPos = transform.parent.position +
                new Vector3(direction * 0.5f, verticalOffset, 0);

            GameObject wolf = Instantiate(wolfPrefab, spawnPos, Quaternion.identity);

            Wolf wolfScript = wolf.GetComponent<Wolf>();
            if (wolfScript != null)
                wolfScript.Initialize(direction, wolfSpeed);
        }
    }
}