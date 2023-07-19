using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HeroesFlight.System.UI.Controllers
{
    public class LoaderUiController : BaseUiController,IUiLoaderController
    {
        [SerializeField] TextMeshProUGUI m_ProgressLabel;
        [SerializeField] Image m_ProgressBar;
       

        public void UpdateLoader(float currentProgress,int count)
        {
            if (Mathf.RoundToInt(currentProgress) == 0)
                return;
            count++;
            var progress =Mathf.RoundToInt((currentProgress / count) * 100);
            m_ProgressLabel.text = progress + "% / 100%";
            m_ProgressBar.fillAmount = currentProgress / count;
        }

    }
}