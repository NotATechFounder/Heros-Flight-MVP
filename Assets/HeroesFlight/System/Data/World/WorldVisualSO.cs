using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "World", menuName = "Game/World", order = 1)]
public class WorldVisualSO : ScriptableObject
{
    public WorldType worldType;
    public string worldName;
    public Sprite icon;
    public ParticleSystem portalParcticle;
    private ParticleSystem portalInstance;

    public void SpawnPortal(Transform transform)
    {
        portalInstance = Instantiate(portalParcticle, transform);
        portalInstance.gameObject.SetActive(false);
    }

    public ParticleSystem GetPortalEffect()
    {
        return portalInstance;
    }
}
