// LifePickup.cs
// Objeto que el jugador recoge para ganar una vida
// y cambiar de personaje aleatoriamente.

using UnityEngine;

namespace PiroBros.Pickups
{
    public class LifePickup : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            // Solo reaccionar al jugador
            if (!other.CompareTag("Player")) return;

            Managers.GameManager.Instance.OnLifePickup();
            Destroy(gameObject);
        }
    }
}
