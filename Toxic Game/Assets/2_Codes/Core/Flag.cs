// Flag.cs
// Objeto al final de cada nivel.
// Al tocarlo el jugador pasa al siguiente nivel o gana el juego.

using UnityEngine;

namespace PiroBros.Core
{
    public class Flag : MonoBehaviour
    {
        private bool alreadyTriggered = false;
        [SerializeField] private Animator animator;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (alreadyTriggered) return;
            if (!other.CompareTag("Player")) return;
            alreadyTriggered = true;

            if (animator != null)
            {
                animator.SetBool("Finish", true);
            }

            Managers.GameManager.Instance.OnFlagReached();
        }
    }
}
