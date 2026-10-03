using UnityEngine;
using PiroBros.Core;

namespace PiroBros.Player
{
    public class Diomedes : Core.Player
    {
        public override string CharacterName => "Diomedes";

        protected override void Initialize()
        {
            currentWeapon = GetComponentInChildren<Weapon>();
            currentAbility = GetComponentInChildren<DiomedesAbility>();
        }
    }
}
