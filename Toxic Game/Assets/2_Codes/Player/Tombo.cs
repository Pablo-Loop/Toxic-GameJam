using UnityEngine;
using PiroBros.Core;
using PiroBros.Interfaces;

namespace PiroBros.Player
{
    public class Tombo : Core.Player
    {
        public override string CharacterName => "Tombo";

        private TomboAbility tomboAbility;

        protected override void Initialize()
        {
            currentWeapon = GetComponentInChildren<Weapon>();
            currentAbility = GetComponentInChildren<TomboAbility>();
            tomboAbility = currentAbility as TomboAbility;
        }

        // Detectar contacto físico con enemigos durante la habilidad
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (tomboAbility == null || !tomboAbility.IsActive) return;

            // Verificar si es un enemigo
            IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

            // Asegurarse que no sea otro jugador
            if (damageable != null && collision.gameObject.GetComponent<Core.Player>() == null)
                tomboAbility.OnContactWithEnemy(collision.gameObject);
        }
    }
}