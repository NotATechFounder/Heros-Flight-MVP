using System.Collections.Generic;
using HeroesFlight.System.Character.Enum;
using HeroesFlight.System.Gameplay.Data.Animation;
using HeroesFlight.System.Gameplay.Enum;
using UnityEngine;

namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class AttackZoneFilter : AttackZoneEnemiesFilterInterface
    {
        public AttackZoneFilter(CharacterSO data, Transform transform)
        {
            characterData = data;
            characterTransform = transform;
        }

        CharacterSO characterData;
        Transform characterTransform;

        public void FilterEnemies(Vector2 attackPoint, bool characterFacingLeft,
            List<IHealthController> enemies, ref List<IHealthController> enemiesToUpdate,
            AttackAnimationEvent dataAttackType)
        {
            enemiesToUpdate.Clear();

            if (enemies == null || enemies.Count == 0)
                return;
            
            switch (dataAttackType.AttackType)
            {
                case AttackType.Regular:
                    FilterEnemiesForBaseAttack(attackPoint, enemies, ref enemiesToUpdate);

                    break;
                case AttackType.Ultimate:
                    switch (characterData.CharacterType)
                    {
                        case CharacterType.Tagon:
                            FilterEnemiesForBaseUltimate(attackPoint, characterFacingLeft, enemies, ref enemiesToUpdate,
                                dataAttackType);
                            break;
                        case CharacterType.Lancer:
                            FilterEnemiesForLancerUltimate(attackPoint, characterFacingLeft, enemies,
                                ref enemiesToUpdate, dataAttackType);
                            break;
                    }

                    break;
            }
        }

        void FilterEnemiesForBaseAttack(Vector2 attackPoint, List<IHealthController> enemies,
            ref List<IHealthController> enemiesToUpdate)
        {
            foreach (var controller in enemies)
            {
                if (Vector2.Distance(controller.HealthTransform.position, attackPoint) <=
                    characterData.GetPlayerStatData.AttackRange)
                {
                    enemiesToUpdate.Add(controller);
                }
            }
        }

        void FilterEnemiesForLancerUltimate(Vector2 attackPoint, bool characterFacingLeft,
            List<IHealthController> enemies,
            ref List<IHealthController> enemiesToUpdate,
            AttackAnimationEvent dataAttackType)
        {
            foreach (var controller in enemies)
            {
                if (Vector2.Distance(controller.HealthTransform.position, attackPoint) >
                    characterData.GetPlayerStatData.AttackRange * characterData.UltimateData.RangeMultiplier)
                {
                    continue;
                }


                var checkPosition = new Vector2(characterTransform.position.x, attackPoint.y);
                var angle = Vector2.Angle(checkPosition, controller.HealthTransform.position);
                var inAngle = angle <= 45f;

                if (inAngle)
                {
                    enemiesToUpdate.Add(controller);
                }
            }
        }

        void FilterEnemiesForBaseUltimate(Vector2 attackPoint, bool characterFacingLeft,
            List<IHealthController> enemies,
            ref List<IHealthController> enemiesToUpdate,
            AttackAnimationEvent dataAttackType)
        {
            var offsetPosition = characterFacingLeft
                ? attackPoint + Vector2.left * characterData.UltimateData.OffsetMultiplier
                : attackPoint + Vector2.right * characterData.UltimateData.OffsetMultiplier;
            foreach (var controller in enemies)
            {
                if (Vector2.Distance(controller.HealthTransform.position, offsetPosition) <=
                    characterData.GetPlayerStatData.AttackRange * characterData.UltimateData.RangeMultiplier)
                {
                    enemiesToUpdate.Add(controller);
                }
            }


            //     switch (dataAttackType.AttackIndex)
            // {
            //     case 1:
            //         foreach (var controller in enemies)
            //         {
            //             if (characterController.IsFacingLeft)
            //             {
            //                 if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y) <= playerStatData.AttackRange
            //                     && controller.currentTransform.position.x <= transform.position.x)
            //                 {
            //                     enemiesToUpdate.Add(controller);
            //                 }
            //             }
            //             else
            //             {
            //                 if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y) <= playerStatData.AttackRange
            //                     && controller.currentTransform.position.x >= transform.position.x)
            //                 {
            //                     enemiesToUpdate.Add(controller);
            //                 }
            //             }
            //         }
            //
            //         break;
            //     case 2:
            //         foreach (var controller in enemies)
            //         {
            //             if (characterController.IsFacingLeft)
            //             {
            //                 if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
            //                     && controller.currentTransform.position.x <= transform.position.x)
            //                 {
            //                     enemiesToUpdate.Add(controller);
            //                 }
            //             }
            //             else
            //             {
            //                 if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
            //                     && controller.currentTransform.position.x >= transform.position.x)
            //                 {
            //                     enemiesToUpdate.Add(controller);
            //                 }
            //             }
            //         }
            //
            //         break;
            //     case 3:
            //         foreach (var controller in enemies)
            //         {
            //             if (characterController.IsFacingLeft)
            //             {
            //                 if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
            //                     && controller.currentTransform.position.x >= transform.position.x - 2.5f)
            //                 {
            //                     enemiesToUpdate.Add(controller);
            //                 }
            //             }
            //             else
            //             {
            //                 if (Mathf.Abs(controller.currentTransform.position.y - attackPoint.y-1) <= playerStatData.AttackRange
            //                     && controller.currentTransform.position.x <= transform.position.x + 2.5f)
            //                 {
            //                     enemiesToUpdate.Add(controller);
            //                 }
            //             }
            //         }
            //
            //         break;
            // }
        }
    }
}