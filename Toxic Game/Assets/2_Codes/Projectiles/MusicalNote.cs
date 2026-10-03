using UnityEngine;
using PiroBros.Interfaces;
using System.Collections.Generic;

namespace PiroBros.Projectiles
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class MusicalNote : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float detectionRadius = 20f;

        private Rigidbody2D rb;
        private Transform target;

        // Lista estática compartida entre todas las notas activas
        // para que no ataquen al mismo enemigo
        private static List<Transform> claimedTargets = new List<Transform>();

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
        }

        private void Start()
        {
            // Buscar enemigo no reclamado por otra nota
            target = FindNearestFreeEnemy();

            if (target != null)
                claimedTargets.Add(target);
            else
                Destroy(gameObject, 3f);
        }

        private void FixedUpdate()
        {
            // Si el enemigo objetivo ya murió, buscar otro
            if (target == null)
            {
                claimedTargets.Remove(target);
                target = FindNearestFreeEnemy();

                if (target == null)
                {
                    Destroy(gameObject);
                    return;
                }

                claimedTargets.Add(target);
            }

            Vector2 direction = ((Vector2)target.position -
                (Vector2)transform.position).normalized;
            rb.linearVelocity = direction * speed;
        }

        private Transform FindNearestFreeEnemy()
        {
            // Limpiar referencias nulas de la lista
            claimedTargets.RemoveAll(t => t == null);

            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position,
                detectionRadius
            );

            Transform nearest = null;
            float minDistance = float.MaxValue;

            foreach (Collider2D hit in hits)
            {
                bool isEnemy = hit.GetComponent<Enemies.Diablito>() != null ||
                               hit.GetComponent<Enemies.DiabloSicario>() != null;

                if (!isEnemy) continue;

                // Ignorar enemigos ya reclamados por otra nota
                if (claimedTargets.Contains(hit.transform)) continue;

                float distance = Vector2.Distance(
                    transform.position,
                    hit.transform.position
                );

                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearest = hit.transform;
                }
            }

            return nearest;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) return;

            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeDamage(999);
                claimedTargets.Remove(target);
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            // Liberar el objetivo al destruirse
            if (target != null)
                claimedTargets.Remove(target);
        }
    }
}