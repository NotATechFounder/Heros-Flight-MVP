using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pelumi.Juicer;

public class WorldBarUI : MonoBehaviour
{
    public enum BarType
    {
        AlwaysVisible,
        ToggleVisibilityOnHit
    }

    [SerializeField] private GameObject healthBar;
    [SerializeField] private Image outerFill;
    [SerializeField] private Image innerFill;
    [SerializeField] private BarType healthBarType;
    [SerializeField] private float visibilityTime = 2f;

    private Coroutine visibilityCoroutine;  

    private void Start()
    {
        healthBar.gameObject.SetActive(healthBarType == BarType.AlwaysVisible);
    }

    public void ChangeType(BarType healthBarType)
    {
        this.healthBarType = healthBarType;
        healthBar.gameObject.SetActive(healthBarType == BarType.AlwaysVisible);
    }

    public void ChangeValue(float normalisedValue)
    {
        if (healthBarType == BarType.ToggleVisibilityOnHit && !healthBar.gameObject.activeInHierarchy)
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
