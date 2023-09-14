using TMPro;
using UnityEngine;

namespace UISystem.Entries
{
    public class RewardEntry : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI rewardText;


        public void SetupReward(string text)
        {
            rewardText.text = text;
        }
    }
}