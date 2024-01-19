using System;
using System.Collections.Generic;
using HeroesFlight.System.Achievement_System.ProgressionUnlocks.UnlockRewards;

namespace HeroesFlight.System.Gameplay.Controllers
{
    /// <summary>
    /// Represents a Run Tracker that keeps track of rewards received during a run.
    /// </summary>
    public class RunTracker
    {
        /// <summary>
        /// List of received rewards.
        /// </summary>
        private List<UnlockReward> receivedRewards = new();

        /// <summary>
        /// Gets or sets the start time of the run.
        /// </summary>
        /// <value>
        /// The start time of the run.
        /// </value>
        /// <remarks>
        /// This variable holds the date and time when the run starts.
        /// </remarks>
        private DateTime runStartTime;

        /// <summary>
        /// Gets the list of received rewards.
        /// </summary>
        /// <remarks>
        /// This property represents the rewards that have been received by the user.
        /// </remarks>
        public List<UnlockReward> ReceivedRewards => receivedRewards;

        /// <summary>
        /// Registers the start time of a run.
        /// </summary>
        public void RegisterRunStart() => runStartTime = DateTime.Now;

        /// <summary>
        /// Adds a reward to the list of received rewards.
        /// </summary>
        /// <param name="reward">The reward to be added.</param>
        public void AddReward(UnlockReward reward) => receivedRewards.Add(reward);

        /// <summary>
        /// Resets the state of the system by clearing the received rewards.
        /// </summary>
        public void Reset() => receivedRewards.Clear();

        /// <summary>
        /// Returns the amount of time passed since the start of the method.
        /// </summary>
        /// <returns>
        /// The amount of time passed as a TimeSpan.
        /// </returns>
        public TimeSpan GetTimePassed()
        {
            return DateTime.Now - runStartTime;
        }
    }
}