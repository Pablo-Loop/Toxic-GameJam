using UnityEngine;
using PiroBros.Core;

namespace PiroBros.Toxicity
{
    [RequireComponent(typeof(Core.Player))]
    public class ToxicitySystem : MonoBehaviour
    {
        [Header("Toxicidad")]
        [SerializeField] private float maxToxicity = 100f;
        [SerializeField] private float currentToxicity = 100f;
        [SerializeField] private float drainPerSecond = 5f;

        private Core.Player player;
        private bool isInToxicZone = false;
        private float recoveryPerSecond = 0f;
        private bool toxicityDeathTriggered = false;

        private void Awake()
        {
            player = GetComponent<Core.Player>();
        }

        private void Start()
        {
            currentToxicity = maxToxicity;
            UpdateUI();
        }

        private void Update()

        {
            

            if (player == null || !player.IsAlive)
                return;

            if (isInToxicZone)
            {
                AddToxicity(recoveryPerSecond * Time.deltaTime);
            }
            else
            {
                Debug.Log("DRENANDO TOXICIDAD: " + currentToxicity);
                RemoveToxicity(drainPerSecond * Time.deltaTime);
            }
        }

        public void AddToxicity(float amount)
        {
            currentToxicity += amount;
            currentToxicity = Mathf.Clamp(
                currentToxicity,
                0f,
                maxToxicity
            );
            UpdateUI();
        }

        public void RemoveToxicity(float amount)
        {
            if (toxicityDeathTriggered)
                return;

            currentToxicity -= amount;

            currentToxicity = Mathf.Clamp(
                currentToxicity,
                0f,
                maxToxicity
            );

              UpdateUI();

            if (currentToxicity <= 0f)
            {
                ToxicityDepleted();
            }
        }

        private void ToxicityDepleted()
        {
            if (toxicityDeathTriggered)
                return;

            toxicityDeathTriggered = true;

            // Usamos el sistema de daño que YA tiene el proyecto.
            player.TakeDamage(1);

            // Preparamos la toxicidad para la nueva vida.
            ResetToxicity();
        }

        public void ResetToxicity()
        {
            currentToxicity = maxToxicity;
            toxicityDeathTriggered = false;
        }

        public void EnterToxicZone(float recoveryRate)
        {
            isInToxicZone = true;
            recoveryPerSecond = recoveryRate;
        }

        public void ExitToxicZone()
        {
            isInToxicZone = false;
            recoveryPerSecond = 0f;
             Debug.Log("ToxicitySystem: salí de zona. isInToxicZone = " + isInToxicZone);
        }

        public float CurrentToxicity => currentToxicity;
        public float MaxToxicity => maxToxicity;

        private void UpdateUI()
        {
            if (PiroBros.Managers.UIManager.Instance !=null)
            {
                PiroBros.Managers.UIManager.Instance.UpdateToxicity(

                    currentToxicity,
                    maxToxicity

                );
            }
        }
                        
        
    }
}
