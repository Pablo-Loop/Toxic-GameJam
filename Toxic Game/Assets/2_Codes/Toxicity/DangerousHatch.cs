using UnityEngine;

namespace PiroBros.Toxicity
{
    public class DangerousHatch : MonoBehaviour
    {
        [Header("Daño de Toxicidad")]
        [SerializeField] private float toxicityDamage = 25f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            ToxicitySystem toxicity = other.GetComponent<ToxicitySystem>();

            if (toxicity != null)
            {
                toxicity.RemoveToxicity(toxicityDamage);
            }
        }
    }
}