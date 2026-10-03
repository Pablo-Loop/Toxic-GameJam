// ShakiraWeapon.cs
// Disparo rápido en ráfaga de 3 balas consecutivas.
// Poco daño por bala pero alta cadencia. Refleja la agilidad de Shakira.

using UnityEngine;
using System.Collections;
using PiroBros.Core;

namespace PiroBros.Weapons
{
    public class ShakiraWeapon : Weapon
    {
        [SerializeField] private int burstCount = 3;        // balas por ráfaga
        [SerializeField] private float burstDelay = 0.08f;  // tiempo entre cada bala

        private bool isFiring = false;

        private void Awake()
        {
            damage = 20;
            fireRate = 0.5f;      // cooldown entre ráfagas completas
            projectileSpeed = 20f; // balas muy rápidas
        }

        public override void Attack()
        {
            if (!CanFire || isFiring) return;

            lastFireTime = Time.time;
            StartCoroutine(FireBurst());
        }

        // Coroutine: dispara varias balas con un pequeño delay entre cada una
        private IEnumerator FireBurst()
        {
            isFiring = true;

            for (int i = 0; i < burstCount; i++)
            {
                SpawnProjectile(GetFireDirection());
                yield return new WaitForSeconds(burstDelay);
            }

            isFiring = false;
        }
    }
}
