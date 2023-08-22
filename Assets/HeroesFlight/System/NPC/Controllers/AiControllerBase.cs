using System;
using HeroesFlight.System.NPC.Controllers;
using HeroesFlightProject.System.NPC.Data;
using HeroesFlightProject.System.NPC.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.NPC.Controllers
{
    public abstract class AiControllerBase : MonoBehaviour, AiControllerInterface
    {
        [SerializeField] protected SpriteRenderer buffDebuffIcon;
        [SerializeField] protected AiAgentModel m_Model;
        [SerializeField] protected float wanderDistance = 10f;
        [SerializeField] protected float agroCooldown = 5f;

        [Header("Knockback parameters")]
        [SerializeField] protected bool useKnockback = true;
        [SerializeField] protected float knockbackDuration = 0.1f;
        [SerializeField] protected float knockbackForce = 5f;
        protected FlashEffect hitEffect;
        protected AiViewController viewController;
        protected AiAnimatorInterface animator;
        protected Collider2D attackCollider;
        protected bool isInknockback;
        public event Action OnInitialized;
        public event Action OnDisabled;
        public EnemyType EnemyType => m_Model.EnemyType;
        public AiAgentModel AgentModel => m_Model;
        public Transform CurrentTarget => currentTarget;

        protected Rigidbody2D rigidBody;
        protected Transform currentTarget;
        protected bool isDisabled;
        protected bool isAggravated;
        protected MonsterStatModifier statModifier;
        bool canAttack;
        Vector2 wanderPosition;
        float timeSinceAggravated = Mathf.Infinity;

        public virtual void Init(Transform player, MonsterStatModifier monsterStatModifier, Sprite currentCardIcon)
        {
            statModifier = EnemyType == EnemyType.MiniBoss ? new MonsterStatModifier() : monsterStatModifier;
            rigidBody = GetComponent<Rigidbody2D>();
            attackCollider = GetComponent<Collider2D>();
            animator = GetComponent<AiAnimatorInterface>();
            viewController = GetComponent<AiViewController>();
            hitEffect = GetComponentInChildren<FlashEffect>();
            currentTarget = player;
            viewController.Init();
            OnInit();

            // viewController.StartFadeIn(2f, Enable);
            DisplayModifiyer(currentCardIcon);
            Enable();
        }

        void Update()
        {
            if (isDisabled)
                return;

            if (!IsAggravated() || !canAttack)
            {
                ProcessWanderingState();
            }
            else
            {
                ProcessFollowingState();
            }

            UpdateTimers();
        }

        void UpdateTimers()
        {
            timeSinceAggravated += Time.deltaTime;
        }


        public virtual Vector2 GetVelocity()
        {
            return Vector2.zero;
        }

        public void SetAttackState(bool attackState)
        {
            canAttack = attackState;
        }

        public virtual void ProcessKnockBack()
        {
            animator.PlayHitAnimation();
            hitEffect.Flash();
        }

        public virtual void Enable()
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            gameObject.SetActive(true);
            attackCollider.enabled = true;
            isDisabled = false;
        }

        public virtual void Disable()
        {
            isDisabled = true;
            attackCollider.enabled = false;
            rigidBody.bodyType = RigidbodyType2D.Static;
            animator.PlayDeathAnimation(() =>
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                }

                OnDisabled?.Invoke();
            });
        }


        public virtual void ProcessWanderingState()
        {
        }

        public virtual void ProcessFollowingState()
        {
        }

        protected bool IsAggravated()
        {
            var distance = Vector2.Distance(CurrentTarget.position, transform.position);
            return distance <= m_Model.CombatModel.GetMonsterStatData.AgroDistance ||
                timeSinceAggravated < agroCooldown;
        }

        protected bool InAttackRange()
        {
            return Vector2.Distance(CurrentTarget.position, transform.position)
                <= m_Model.CombatModel.GetMonsterStatData.AttackRange;
        }

        public void DisplayModifiyer(Sprite sprite)
        {
            if (sprite == null)
            {
                buffDebuffIcon.enabled = false;
            }
            else
            {
                buffDebuffIcon.enabled = true;
                buffDebuffIcon.sprite = sprite;
            }
        }

        protected void OnInit()
        {
            OnInitialized?.Invoke();
        }

        public MonsterStatModifier GetMonsterStatModifier()
        {
            return statModifier;
        }

        public void Aggravate()
        {
            timeSinceAggravated = 0;
        }
    }
}