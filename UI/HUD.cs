using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Assets.Scripts
{
    public class HUD : MonoBehaviour
    {
        #region Singleton
        public static HUD Instance;
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

        [Header("Floating Text")]
        public GameObject FloatingText;
        public Vector3 floatingTextOffset;
        [HideInInspector]
        public GameObject FloatingTextContainer;

        [Header("UI Elements")]
        public Text currentHPText;
        public HealthBar HealthBar;
        public Text crystalCountText;
        public Text gameTimer;

        public Button startButton, resumeButton, returnToMenuButton;

        [Header("Screen Effects")]
        public GameObject ScreenFlash;

        [HideInInspector]
        public PlayerCharacter player;

        private void Start()
        {
            FloatingTextContainer = new GameObject("FloatingTextContainer");
        }

        private void Update()
        {
            gameTimer.text = GameManager.Instance.FormatTime(GameManager.Instance.gameTimer);
        }

        public void Init()
        {
            if (player == null) return;
            currentHPText.text = player.currentHP.ToString();
            HealthBar.RefillHealthBar();
            crystalCountText.text = "Crystal Threat Level: 0";
        }

        public void UpdatePlayerHealthBar()
        {
            if (player == null) return;
            currentHPText.text = player.currentHP.ToString();
            HealthBar.Set(player.maxHP, player.currentHP);
        }

        public void UpdateCrystalCount(int number)
        {
            crystalCountText.text = $"Crystal Threat Level: {number.ToString()}";
        }

        public void CreateFloatingText(string textString, int fontSize, Color textColor, float textDuration, Vector3 position, float moveSpeed)
        {
            Vector3 textPosition = new Vector3(position.x + floatingTextOffset.x, position.y + floatingTextOffset.y, position.z + floatingTextOffset.z);
            var NewFloatingText = Instantiate(FloatingText);
            NewFloatingText.transform.position = position;
            //GameObject NewFloatingText = Instantiate(FloatingText, textPosition, Quaternion.identity, FloatingTextContainer);
            //NewFloatingText.transform.SetParent(FloatingTextContainer.transform);
            NewFloatingText.SetActive(true);
            FloatingText newFloatingText = NewFloatingText.GetComponent<FloatingText>();
            newFloatingText.SetFloatingText(textString, fontSize, textColor);
            newFloatingText.moveSpeed = moveSpeed;
            Destroy(NewFloatingText, textDuration);

        }


    }

}