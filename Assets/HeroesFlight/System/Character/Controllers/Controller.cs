using UnityEngine;

namespace HeroesFlight.System.Character
{
    public abstract class Controller : MonoBehaviour
    {
        public abstract Vector3 GetVelocity();
    }
}