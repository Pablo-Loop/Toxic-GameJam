using UnityEngine;

namespace PiroBros.Toxicity
{
    public class ToxicZone : MonoBehaviour
    {
        [Header("Recuperación")]
        [SerializeField] private float recoveryPerSecond = 20f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            ToxicitySystem toxicity = other.GetComponent<ToxicitySystem>();

            if (toxicity != null)
            {
                toxicity.EnterToxicZone(recoveryPerSecond);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

                  Debug.Log("SALÍ DE TOXIC HATCH");

            ToxicitySystem toxicity = other.GetComponent<ToxicitySystem>();

            if (toxicity != null)
            {
                toxicity.ExitToxicZone();
            }
        }
    }
}