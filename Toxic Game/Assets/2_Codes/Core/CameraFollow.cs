// CameraFollow.cs
// Script simple para que la cámara siga al jugador.
// Más adelante puede reemplazarse con Cinemachine sin tocar nada más.

using UnityEngine;

namespace PiroBros.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float smoothSpeed = 5f;
        [SerializeField] private Vector3 offset = new Vector3(0, 1, -10);

        private void LateUpdate()
        {
            if (target == null) return;

            Vector3 desiredPosition = target.position + offset;
            transform.position = Vector3.Lerp(
                transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );
        }

        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
    }
}
