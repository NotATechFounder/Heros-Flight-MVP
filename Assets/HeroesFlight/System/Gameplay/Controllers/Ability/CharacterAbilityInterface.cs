using Spine.Unity;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public interface CharacterAbilityInterface : AbilityInterface
    {
        float CurrentCharge { get; }
        void Init(AnimationReferenceAsset[] animations);
        void UpdateAbilityCharges(int value);
    }
}