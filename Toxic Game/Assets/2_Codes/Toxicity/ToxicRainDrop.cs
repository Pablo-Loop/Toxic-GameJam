using UnityEngine;

namespace PiroBros.Toxicity
{
    public class ToxicRainDrop : MonoBehaviour
    {
        [Header("Lluvia Tóxica")]
        [SerializeField] private float toxicityDamage = 10f;
        [SerializeField] private float fallSpeed = 7f;
        [SerializeField] private float lifeTime = 5f;

        private void Start()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Update()
        {
            transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                ToxicitySystem toxicity = other.GetComponent<ToxicitySystem>();

                if (toxicity != null)
                {
                    toxicity.RemoveToxicity(toxicityDamage);
                }

                Destroy(gameObject);
                return;
            }

            if (other.CompareTag("Ground"))
            {
                Destroy(gameObject);
            }
        }
    }
}