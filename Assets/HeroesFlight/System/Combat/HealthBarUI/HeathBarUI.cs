using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pelumi.Juicer;

public class HeathBarUI : MonoBehaviour
{
    public enum HealthBarType
    {
        AlwaysVisible,
        ToggleVisibilityOnHit
    }

    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image outerFill;
    [SerializeField] private Image innerFill;
    [SerializeField] private HealthBarType healthBarType;
    [SerializeField] private float visibilityTime = 2f;

    private Coroutine visibilityCoroutine;  

    private void Start()
    {
        healthBar.gameObject.SetActive(healthBarType == HealthBarType.AlwaysVisible);
    }

    public void ChangeType(HealthBarType healthBarType)
    {
        this.healthBarType = healthBarType;
        healthBar.gameObject.SetActive(healthBarType == HealthBarType.AlwaysVisible);
    }

    public void ChangeValue(float normalisedValue)
    {
        if (healthBarType == HealthBarType.ToggleVisibilityOnHit && !healthBar.gameObject.activeInHierarchy)
        {
            healthBar.gameObject.SetActive(true);
        }

        innerFill.JuicyFillAmount(normalisedValue, 0.5f).Start();
        outerFill.fillAmount = normalisedValue;
    }

    public IEnumerator VisibilityCoroutine()
    {
        yield return new WaitForSeconds(visibilityTime);
        healthBar.gameObject.SetActive(false);
        visibilityCoroutine = null;
    }
}
