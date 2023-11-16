using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Plugins.Audio_System;
using Pelumi.Juicer;

public class HeroAttributeUI : MonoBehaviour
{
    public Action<HeroAttributeUI> OnAddSpEffectStart;
    public Action<HeroAttributeUI> OnAddSpEffectCompleted;

    public Action<StatPointSO> OnInfoButtonClickedEvent;

    //[SerializeField] private AdvanceButton upButton;
    [SerializeField] private AdvanceButton downButton;
    //[SerializeField] private AdvanceButton infoButton;
    [SerializeField] private TextMeshProUGUI attributeName;
    [SerializeField] private TextMeshProUGUI attributeValue;
    [SerializeField] private TextMeshProUGUI attributeInfoText;
    [SerializeField] private Image icon;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Image levelUpFill;

    JuicerRuntime levelUpEffect;

    private void Awake()
    {
        //upButton.onClick.AddListener(OnUpButtonClicked);
        downButton.onClick.AddListener(OnDownButtonClicked);
        //infoButton.onClick.AddListener(OnInfoButtonClicked);

        levelUpEffect = levelUpFill.JuicyFillAmount(1, 0.15f);
    }

    public void SetAttribute()
    {
        //this.attributeInfo = attribute;
        //attributeName.text = attribute.AttributeSO.Attribute.ToString();
        //attributeValue.text = "LV. " + attribute.CurrentSP.ToString();
        //attributeInfoText.text = attribute.AttributeSO.Description;
        //icon.sprite = attribute.AttributeSO.Icon;
        //attribute.OnSPChanged = OnSPChanged;
        //attribute.OnModified = OnModified;
    }

    public void OnSPChanged(int sp)
    {
        levelUpEffect.SetOnCompleted(() =>
        {
            attributeValue.text = "LV. " + sp.ToString();
            OnAddSpEffectCompleted?.Invoke(this);
        });

        AudioClip clip = AudioManager.GetSoundEffectClip("HeroProgressionGain");
        levelUpEffect.ChangeDuration(clip.length - 1.5f);
        AudioManager.PlaySoundEffect("HeroProgressionGain", SoundEffectCategory.UI);
        levelUpEffect.Start(()=>levelUpFill.fillAmount = 0);
        OnAddSpEffectStart?.Invoke(this);
    }

    public void OnModified(bool modified)
    {
        attributeValue.color = modified ? Color.green : Color.white;
    }

    public void OnUpButtonClicked()
    {

    }

    public void OnDownButtonClicked()
    {
   
    }

    public void OnInfoButtonClicked()
    {

    }

    public void ResetSpText()
    {
        attributeValue.text = "LV. 0";
    }

    public void ToggleButtonActive(bool active)
    {
        downButton.interactable = active;
        buttonImage.color = active ? Color.white : Color.grey;
    }
}
