using HeroesFlightProject.System.Combat.Controllers;
using Plugins.Audio_System;
using UnityEngine;

public class StormSkillController : MonoBehaviour, IActiveAbilityInterface
{
    [SerializeField] private TimedAbility skillOne;
    [SerializeField] private SkillOrb skillOrbPrefab;

    public TimedAbility PassiveAbilityOne => skillOne;

    private SkillOrb skillOrb;
    private CharacterStatController characterStatController;

    private void Awake()
    {
        characterStatController = GetComponent<CharacterStatController>();

        skillOne.OnActivated = OnActivateSkillOne;
        skillOne.OnDeactivated = OnDeactivateSkillOne;
    }

    private void Start()
    {
        skillOne.Init(this);
    }

    public void OnActivateSkillOne()
    {
        AudioManager.PlaySoundEffect("StormSkillActivation", SoundEffectCategory.Hero);

        if (skillOrb == null)
        {
            skillOrb = Instantiate(skillOrbPrefab, transform.position, Quaternion.identity);
            skillOrb.SetTarget(transform);
            if(skillOrb.TryGetComponent(out AutoShooter autoShooter))
            {
                autoShooter.SetDamage((int)characterStatController.CurrentMagicDamage);
            }
        }

        skillOrb.Activate();
    }

    public void OnDeactivateSkillOne()
    {
        skillOrb.Deactivate();
    }
}
