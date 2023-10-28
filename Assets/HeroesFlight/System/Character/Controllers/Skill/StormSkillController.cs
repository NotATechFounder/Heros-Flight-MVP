using HeroesFlightProject.System.Combat.Controllers;
using Plugins.Audio_System;
using UnityEngine;

public class StormSkillController : MonoBehaviour, ISkillControllerInterface
{
    [SerializeField] private CharacterTimedSKill skillOne;
    [SerializeField] private SkillOrb skillOrbPrefab;

    public CharacterTimedSKill SkillOne => skillOne;

    private SkillOrb skillOrb;
    private CharacterStatController characterStatController;

    private void Awake()
    {
        characterStatController = GetComponent<CharacterStatController>();

        skillOne.OnSkillActivated = OnActivateSkillOne;
        skillOne.OnSkillDeactivated = OnDeactivateSkillOne;
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
