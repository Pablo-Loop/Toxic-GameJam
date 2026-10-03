// DiomedesWeapon.cs
// Disparo estándar. Un proyectil por disparo, cadencia media.
// El arma más balanceada del juego.

using UnityEngine;
using PiroBros.Core;

namespace PiroBros.Weapons
{
    public class DiomedesWeapon : Weapon
    {
        public override void Attack()
        {
            if (!CanFire) return;

            SpawnProjectile(GetFireDirection());
        }
    }
}
