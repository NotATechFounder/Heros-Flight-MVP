using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyHandler : MonoBehaviour
{
    public enum CurrencyType
    {
        Persistent,
        NonPersistent
    }

    public class Currency
    {
        public string key;
        public float amount;    
    }

    public class PersistentCurrency : Currency
    {

    }

    public class NonPersistentCurrency : Currency
    {

    }
}
