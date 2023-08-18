using System.Collections.Generic;
using Cinemachine;
using HeroesFlightProject.System.Gameplay.Controllers.ShakeProfile;
using UnityEngine;


namespace HeroesFlightProject.System.Gameplay.Controllers
{
    public class CinemachineCameraShaker : CameraShakerInterface
    {
        public CinemachineCameraShaker(CinemachineImpulseSource bumbSource, CinemachineImpulseSource explosionSource,
            CinemachineImpulseSource rumbleSource, CinemachineImpulseSource recoilSource,CinemachineImpulseSource genericSource,CinemachineImpulseListener listener)
        {
            impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Bump, bumbSource);
            impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Explosion, explosionSource);
            impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Rumble, rumbleSource);
            impulseSources.Add(CinemachineImpulseDefinition.ImpulseShapes.Recoil, recoilSource);
            source = genericSource;
            impulseListener = listener;
        }


        Dictionary<CinemachineImpulseDefinition.ImpulseShapes, CinemachineImpulseSource> impulseSources = new();

        CinemachineImpulseSource source;
        CinemachineImpulseListener impulseListener;
        public void ShakeCamera(CinemachineImpulseDefinition.ImpulseShapes shape, float duration = 0.1f,
            float force = 1f,
            Vector2 velocity = default)
        {
            if (impulseSources.TryGetValue(shape, out var source))
            {
                source.m_ImpulseDefinition.m_ImpulseDuration = duration;
                if (velocity.Equals(Vector2.zero))
                {
                    source.GenerateImpulseWithForce(force);
                }
                else
                {
                    source.GenerateImpulseWithVelocity(velocity);
                }
            }
        }

        public void ShakeCamera(ScreenShakeProfile profile)
        {
            SetupSettingsForGenericSource(profile);
            source.GenerateImpulseWithForce(profile.shakeForce);
        }

        void SetupSettingsForGenericSource(ScreenShakeProfile profile)
        {
            var definition = source.m_ImpulseDefinition;
            definition.m_ImpulseDuration = profile.shakeTime;
            source.m_DefaultVelocity = profile.defaultvelocity;
            definition.m_CustomImpulseShape = profile.impulseCurve;

            impulseListener.m_ReactionSettings.m_AmplitudeGain = profile.listenerAmplitude;
            impulseListener.m_ReactionSettings.m_FrequencyGain = profile.listenerFrequency;
            impulseListener.m_ReactionSettings.m_Duration = profile.listenerDuration;

        }
    }
}