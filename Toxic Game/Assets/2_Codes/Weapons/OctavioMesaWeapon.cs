// OctavioWeapon.cs
// Disparo doble: dos balas ligeramente separadas en diagonal.
// Cubre más área que un disparo simple. Estilo equilibrado.

using UnityEngine;
using PiroBros.Core;

namespace PiroBros.Weapons
{
    public class OctavioWeapon : Weapon
    {
        [SerializeField] private float spreadAngle = 10f; // grados de separación

        private void Awake()
        {
            damage = 12;
            fireRate = 0.35f;
            projectileSpeed = 16f;
        }

        public override void Attack()
        {
            if (!CanFire) return;

            Vector2 baseDirection = GetFireDirection();

            // Calcular dos direcciones con un pequeño ángulo de separación
            Vector2 dirUp = RotateVector(baseDirection, spreadAngle);
            Vector2 dirDown = RotateVector(baseDirection, -spreadAngle);

            SpawnProjectile(dirUp);
            SpawnProjectile(dirDown);

            lastFireTime = Time.time;
        }

        // Rota un Vector2 un número de grados
        private Vector2 RotateVector(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            return new Vector2(
                v.x * Mathf.Cos(rad) - v.y * Mathf.Sin(rad),
                v.x * Mathf.Sin(rad) + v.y * Mathf.Cos(rad)
            );
        }
    }
}
