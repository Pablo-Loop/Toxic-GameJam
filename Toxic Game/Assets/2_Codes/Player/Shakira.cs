using PiroBros.Core;

namespace PiroBros.Player
{
    public class Shakira : Core.Player
    {
        public override string CharacterName => "Shakira";

        protected override void Initialize()
        {
            currentWeapon = GetComponentInChildren<Weapon>();
            currentAbility = GetComponentInChildren<ShakiraAbility>();
        }
    }
}
