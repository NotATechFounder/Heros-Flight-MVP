using System;
using Pelumi.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class ShootingHazard : EnironmentHazard
{
    [Header("Shooting Spike Settings")]
    [SerializeField] private float warningLineDuration;
    [SerializeField] private float warningLineWidth;
    [SerializeField] private float healthPercentage = 15;
    [SerializeField] private WarningLine warningLine;
    [SerializeField] private HazardArrow[] hazardArrow;
    private bool isTriggered;
    private HazardArrow currentProjectile;

    private void Start()
    {
        StartCoroutine(Runtime());
    }

    private void Trigger()
    {
        isTriggered = true;

        HazardArrow arrowPrefab = null;
        if (hazardArrow.Length > 1)
        {
            arrowPrefab = hazardArrow[Random.Range(0, hazardArrow.Length)];
        }
        else
        {
            arrowPrefab = hazardArrow[0];
        }
        var width = arrowPrefab.GetComponent<BoxCollider2D>().size.x;
        warningLine.Trigger(() =>
        {
            currentProjectile = ObjectPoolManager.SpawnObject(arrowPrefab, warningLine.transform.position, Quaternion.identity);
          
            currentProjectile.SetupArrow(healthPercentage, warningLine.GetFowardDirection);
            isTriggered = false;
        }, warningLineDuration,width);
    }


    private IEnumerator Runtime()
    {
        while (true)
        {
            yield return ActivateCooldown();
            Trigger();
            yield return new WaitUntil(() => !isTriggered);
            yield return null;
        }
    }

    private void OnDestroy()
    { 
        Debug.Log("releasing projectile");
        ObjectPoolManager.ReleaseObject(currentProjectile);
      }
}
