using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodsBenevolenceSocket : MonoBehaviour
{
    [SerializeField] private Transform topSocket;
    [SerializeField] private Transform bottomSocket;

    public Transform TopSocket => topSocket;
    public Transform BottomSocket => bottomSocket;
}
