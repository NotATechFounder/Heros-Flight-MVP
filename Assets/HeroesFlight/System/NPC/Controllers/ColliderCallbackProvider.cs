using System;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public class ColliderCallbackProvider : MonoBehaviour
    {
        [SerializeField] CollisionEvenType collisionType;
        [SerializeField] string targetTag = "Player";

        public event Action OnColliderEventWithTarget;


        void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.gameObject.name);
            if (collisionType != CollisionEvenType.Trigger)
                return;
            if(other.gameObject.CompareTag(targetTag))
                OnColliderEventWithTarget?.Invoke();
        }

        void OnCollisionEnter(Collision collision)
        {
            if (collisionType != CollisionEvenType.Collide)
                return;
            if(collision.gameObject.CompareTag(targetTag))
                OnColliderEventWithTarget?.Invoke();
        }
    }
}