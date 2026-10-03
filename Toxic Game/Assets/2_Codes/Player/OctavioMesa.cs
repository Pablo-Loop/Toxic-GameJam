using PiroBros.Core;

namespace PiroBros.Player
{
    public class OctavioMesa : Core.Player
    {
        public override string CharacterName => "Octavio Mesa";

        protected override void Initialize()
        {
            currentWeapon = GetComponentInChildren<Weapon>();
            currentAbility = GetComponentInChildren<OctavioAbility>();
        }
    }
}
