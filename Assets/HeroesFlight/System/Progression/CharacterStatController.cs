using System;
using UnityEngine;

public class CharacterStatController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer currentCardIcon;

    [SerializeField] PlayerStatData playerCombatModel;

    public PlayerStatData PlayerStatData => playerCombatModel;
    public Action<float, bool> OnHealthModified;
    public Action<float> OnMaxHealthChanged;
    public Func<float> GetCurrentHealth;
    public HeroType GetHeroType => playerCombatModel.HeroType;

    private StatModel statModel;
    public StatModel GetStatModel => statModel;

    public bool debug = false;


    private void Start()
    {
        if (debug)
        {
            Initialize(new StatModel(playerCombatModel));    
        }
    }

    public void Initialize(StatModel statModel)
    {
        this.statModel = statModel;
        this.playerCombatModel = statModel.GetPlayerStatData;
    }

    public void SetCurrentCardIcon(Sprite sprite)
    {
        if (sprite != null)
        {
            currentCardIcon.sprite = sprite;
            currentCardIcon.enabled = true;
        }
        else
        {
            currentCardIcon.enabled = false;
        }
    }

    public void ModifyHealth(float percentageAmount, bool increase)
    {
        float value = StatCalc.GetPercentage(statModel.GetBaseStatValue(StatType.MaxHealth), percentageAmount);
        OnHealthModified?.Invoke(value, increase);
    }

    public float GetHealthPercentage()
    {
        if (GetCurrentHealth != null)
        {
            return (GetCurrentHealth() / statModel.GetCurrentStatValue(StatType.MaxHealth)) * 100;
        }
        return 0;
    }

    public void ResetStats()
    {
        statModel = new StatModel(playerCombatModel);   
    }
}
