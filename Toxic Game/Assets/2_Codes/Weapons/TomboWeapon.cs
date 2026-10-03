// TomboWeapon.cs
// Disparo lento pero con mucho daño. Un proyectil grande por disparo.
// Refleja el estilo tanque de Tombo.

using UnityEngine;
using PiroBros.Core;

namespace PiroBros.Weapons
{
    public class TomboWeapon : Weapon
    {
        // Tombo dispara más lento pero hace más daño
        // Estos valores sobreescriben los del Inspector si se necesita
        private void Awake()
        {
            damage = 35;
            fireRate = 0.8f;
            projectileSpeed = 10f;
        }

        public override void Attack()
        {
            if (!CanFire) return;

            SpawnProjectile(GetFireDirection());
        }
    }
}
