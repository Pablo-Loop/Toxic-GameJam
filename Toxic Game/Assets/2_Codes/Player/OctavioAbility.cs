// OctavioAbility.cs
// Lanza 3 notas musicales simultáneas que buscan
// al enemigo más cercano y lo eliminan al impactar.

using UnityEngine;

namespace PiroBros.Player
{
    public class OctavioAbility : Core.Ability
    {
        [SerializeField] private GameObject notePrefab;

        protected override void Initialize()
        {
            maxUses = 4;
        }

        protected override void Activate()
        {
            if (notePrefab == null)
            {
                Debug.LogWarning("OctavioAbility: falta notePrefab.");
                return;
            }

            // Spawnear 3 notas con offsets verticales distintos
            SpawnNote(0f);
            SpawnNote(0.6f);
            SpawnNote(-0.6f);
        }

        private void SpawnNote(float verticalOffset)
        {
            Vector3 spawnPos = transform.parent.position + new Vector3(0, verticalOffset, 0);
            Instantiate(notePrefab, spawnPos, Quaternion.identity);
        }
    }
}
