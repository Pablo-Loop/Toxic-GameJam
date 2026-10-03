// Weapon.cs
// Clase base abstracta para todas las armas.
// Contiene la lógica común: cooldown, referencia al proyectil, spawn.
// Cada arma hija define su propio tipo de disparo en Attack().

using PiroBros.Interfaces;
using PiroBros.Projectiles;
using UnityEngine;

namespace PiroBros.Core
{
    public abstract class Weapon : MonoBehaviour, IAttacker
    {
        [Header("Configuración Base")]
        [SerializeField] protected int damage = 50;
        [SerializeField] protected float fireRate = 0.3f;
        [SerializeField] protected float projectileSpeed = 15f;
        [SerializeField] protected Transform firePoint;

        [Header("Proyectil")]
        [SerializeField] private GameObject projectilePrefab;

        protected float lastFireTime = -999f;

        // ¿Puede disparar ahora según el cooldown?
        protected bool CanFire => Time.time >= lastFireTime + fireRate;

        // Cada arma implementa su propia lógica de disparo
        public abstract void Attack();

        // Método reutilizable para spawnear un proyectil en cualquier dirección.
        // Las armas hijas lo llaman pasando la dirección que necesitan.
        protected void SpawnProjectile(Vector2 direction)
        {
            if (projectilePrefab == null || firePoint == null)
            {
                Debug.LogWarning($"{name}: falta projectilePrefab o firePoint.");
                return;
            }

            GameObject obj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            PiroBros.Projectiles.Projectile projectile = obj.GetComponent<PiroBros.Projectiles.Projectile>();

            if (projectile != null)
                projectile.Initialize(damage, direction, projectileSpeed);

            lastFireTime = Time.time;
        }

        // Devuelve la dirección actual del personaje que porta el arma.
        // Mira la escala X del padre para saber hacia dónde mira.
        protected Vector2 GetFireDirection()
        {
            return transform.parent.localScale.x > 0 ? Vector2.right : Vector2.left;
        }

        public int Damage => damage;
    }
}
