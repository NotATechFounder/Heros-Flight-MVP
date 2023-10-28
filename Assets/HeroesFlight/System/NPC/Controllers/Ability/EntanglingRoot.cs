using System;
using Spine.Unity;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class EntanglingRoot : AreaDamageEntity
    {
        [SerializeField] AnimationReferenceAsset idleAniamtion;
        [SerializeField] AnimationReferenceAsset startAnimation;
        [SerializeField] AnimationReferenceAsset endAnimation;
        [SerializeField] SkeletonAnimation skeletonAnimation;
        [SerializeField] Rigidbody2D playerRigidBody;
        [SerializeField] GameObject staticIndicator;
        
        public override void Init()
        {
            tickWait = new WaitForSeconds(tick);
            skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
        }


        public override void StartDetection(Action onComplete)
        {
            playerRigidBody=null;
            if (overlap.Detect())
            {
                overlap.OnDetect += NotifyTargetDetected;
                skeletonAnimation.gameObject.SetActive(true);
                skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
              
                Debug.Log("Starting to detect ");
                StartDetectionRoutine(() =>
                {
                    skeletonAnimation.AnimationState.SetAnimation(0, idleAniamtion, true);
                    if (playerRigidBody != null)
                    {
                        playerRigidBody.bodyType = RigidbodyType2D.Dynamic;
                        playerRigidBody = null;
                        skeletonAnimation.transform.localPosition =Vector3.zero;
                        Debug.Log("Reseting player");
                    }

                    skeletonAnimation.gameObject.SetActive(false);
                    overlap.OnDetect -= NotifyTargetDetected;
                    onComplete?.Invoke();
                    Debug.Log("FInished checking");
                });
            }
            else
            {
                ToggleIndicator(false);
            }
           
        }

        protected override void NotifyTargetDetected(int count, Collider2D[] targets)
        {
            if (playerRigidBody == null)
            {
                playerRigidBody = targets[0].GetComponent<Rigidbody2D>();
                playerRigidBody.bodyType = RigidbodyType2D.Static;
                skeletonAnimation.AnimationState.SetAnimation(0, startAnimation, false);
                skeletonAnimation.AnimationState.AddAnimation(0, endAnimation, false, 0.2f);
              
            }
            skeletonAnimation.transform.position = playerRigidBody.transform.position + new Vector3(0,-1,0);
            base.NotifyTargetDetected(count, targets);
        }

        public override void ToggleIndicator(bool isEnabled)
        {
            staticIndicator.SetActive(isEnabled);
        }
    }
}