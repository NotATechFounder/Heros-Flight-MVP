using System.Collections;
using System.Collections.Generic;
using HeroesFlight.System.ShrineSystem;
using UnityEngine;

public class ShrineNPCHolder : MonoBehaviour
{
    [SerializeField] private ShrineNPC[] shrineNPCs;
    public Dictionary<ShrineNPCType, ShrineNPC> shrineNPCsCache = new();

    private void Awake()
    {
        foreach (ShrineNPC shrineNPC in shrineNPCs)
        {
            shrineNPCsCache.Add(shrineNPC.GetShrineNPCType(), shrineNPC);
        }
    }

}
