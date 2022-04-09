using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Scripts
{
    public class MenuController : MonoBehaviour
    {
        #region Singleton
        public static MenuController Instance;
        private void Awake()
        {
            if (Application.isEditor)
                Instance = this;
            else
            {
                if (Instance == null)
                    Instance = this;
                else
                {
                    Destroy(gameObject);
                    return;
                }

                DontDestroyOnLoad(gameObject);
            }
        }
        #endregion

        [Header("Menu Panels")]
        public CanvasGroup hudCanvasGroup;
        public CanvasGroup gameMenu;

       // public CanvasGroup gameMenu_Title;
       // public CanvasGroup gameMenu_Pause;

        private void Start()
        {
            Init();
        }

        public void Init()
        {
            hudCanvasGroup.alpha = 1;

            // TODO
            //gameMenu.alpha = 0;
            //gameMenu.interactable = false;
            //gameMenu.blocksRaycasts = false;
        }

        public void ToggleHUD()
        {
            if (hudCanvasGroup.alpha < 1)
            {
                hudCanvasGroup.DOFade(1f, 0.2f).SetUpdate(UpdateType.Normal, true);
            }
            else
            {
                hudCanvasGroup.DOFade(0f, 0.2f).SetUpdate(UpdateType.Normal, true);
            }
        }


        public void OpenMenuPanel(CanvasGroup panel)
        {
            StartCoroutine(OpenPanel(panel, 0.5f));
        }

        IEnumerator OpenPanel(CanvasGroup panel, float duration)
        {
            Tween fadeIn = panel.DOFade(0.8f, duration).SetUpdate(UpdateType.Normal, true);
            yield return new WaitForSecondsRealtime(duration);
            fadeIn.Kill();
            panel.interactable = true;
            panel.blocksRaycasts = true;
        }

        public void CloseMenuPanel(CanvasGroup panel)
        {
            StartCoroutine(ClosePanel(panel, 0.2f));
        }

        IEnumerator ClosePanel(CanvasGroup panel, float duration)
        {
            Tween fadeOut = panel.DOFade(0f, duration).SetUpdate(UpdateType.Normal, true);
            yield return new WaitForSecondsRealtime(duration);
            fadeOut.Kill();
            panel.interactable = false;
            panel.blocksRaycasts = false;
        }

        public void SwapButtons(GameObject oldButton, GameObject newButton)
        {
            oldButton.gameObject.SetActive(false);
            newButton.gameObject.SetActive(true);
        }

    }
}
