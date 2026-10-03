// Diablito.cs
// El único enemigo del juego por ahora.
// Hereda toda la IA de Enemy y solo define sus stats.
// Si en el futuro quieres un Diablito especial con otro comportamiento,
// puedes sobreescribir Attack() o HandleAI() aquí.

using UnityEngine;
using PiroBros.Core;

namespace PiroBros.Enemies
{
    public class Diablito : Enemy
    {
        protected override void Initialize()
        {
            maxHealth = 1;
            moveSpeed = 2.5f;
            attackDamage = 15;
            detectionRange = 8f;
            attackRange = 1f;
            attackCooldown = 1.2f;
        }
    }
}
