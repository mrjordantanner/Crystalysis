using System.Collections;
using UnityEngine;
using TMPro;

namespace Assets.Scripts
{
    public class Message : MonoBehaviour
    {
        #region Singleton
        public static Message Instance;
        private void Awake()
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
        #endregion

        [Header("Messages")]
        public TextMeshProUGUI message;
        public TextMeshProUGUI messageCenter;

        private void Start()
        {
            ClearAll();
        }

        public IEnumerator ShowMessage(string text, Color color, float duration)
        {
            message.color = color;
            message.text = text;
            yield return new WaitForSeconds(duration);
            message.text = " ";

        }

        public IEnumerator ShowMessageCenter(string text, Color color, float duration)
        {

            messageCenter.color = color;
            messageCenter.text = text;
            yield return new WaitForSeconds(duration);
            messageCenter.text = " ";

        }

        public void ClearAll()
        {
            message.text = "";
            messageCenter.text = "";
        }
    }
}
