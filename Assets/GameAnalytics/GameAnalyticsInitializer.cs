using GameAnalyticsSDK;
using UnityEngine;

namespace HeroesFlight.GameAnalyticsHandler
{
    public class GameAnalyticsInitializer : MonoBehaviour, IGameAnalyticsATTListener
    {
        /// <summary>
        /// Calling init from start as according to GameAnalytics docs it has to preinit on Awake before calling Initialize();
        /// </summary>
        private void Start()
        {
            if(Application.platform == RuntimePlatform.IPhonePlayer)
            {
                GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
                GameAnalytics.Initialize();
            }
        }

        /// Method: GameAnalyticsATTListenerNotDetermined
        /// Description: This method is used to handle the game analytics ATT listener when authorization status is not determined.
        /// Parameters: None
        /// Returns: None
        /// /
        public void GameAnalyticsATTListenerNotDetermined()
        {
            GameAnalytics.Initialize();
        }

        /// <summary>
        /// This method is used as a listener for the restricted version of GameAnalytics' AppTrackingTransparency (ATT) feature.
        /// It is responsible for initializing the GameAnalytics SDK.
        /// </summary>
        /// <remarks>
        /// The ATT feature aims to collect users' permissions related to app tracking on iOS devices.
        /// This method initializes the GameAnalytics SDK, which includes any necessary setup or configuration steps.
        /// </remarks>
        public void GameAnalyticsATTListenerRestricted()
        {
            GameAnalytics.Initialize();
        }

        /// <summary>
        /// This method is used to handle the event when the user denies App Tracking Transparency (ATT) permission.
        /// It initializes the GameAnalytics instance.
        /// </summary>
        public void GameAnalyticsATTListenerDenied()
        {
            GameAnalytics.Initialize();
        }

        /// <summary>
        /// This method is an event listener for when the user's consent for data tracking is authorized.
        /// It initializes the GameAnalytics SDK.
        /// </summary>
        /// <remarks>
        /// Call this method when the user has authorized the consent for data tracking.
        /// This method ensures that the GameAnalytics SDK is properly initialized and ready to track data.
        /// </remarks>
        public void GameAnalyticsATTListenerAuthorized()
        {
            GameAnalytics.Initialize();
        }
    }
}