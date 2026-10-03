// DiomedesAbility.cs
// Lanza una granada que explota 2 segundos después
// eliminando todos los enemigos en el rango.

using UnityEngine;
using PiroBros.Projectiles;

namespace PiroBros.Player
{
    public class DiomedesAbility : Core.Ability
    {
        [SerializeField] private GameObject grenadePrefab;
        [SerializeField] private float throwForce = 10f;

        protected override void Initialize()
        {
            maxUses = 3;
        }

        protected override void Activate()
        {
            if (grenadePrefab == null)
            {
                Debug.LogWarning("DiomedesAbility: falta grenadePrefab.");
                return;
            }

            float direction = transform.parent.localScale.x > 0 ? 1f : -1f;
            Vector3 spawnPos = transform.parent.position + new Vector3(direction, 0.5f, 0);

            GameObject grenade = Instantiate(grenadePrefab, spawnPos, Quaternion.identity);

            Grenade grenadeScript = grenade.GetComponent<Grenade>();
            if (grenadeScript != null)
                grenadeScript.Initialize(new Vector2(direction * throwForce, 5f));
        }
    }
}
