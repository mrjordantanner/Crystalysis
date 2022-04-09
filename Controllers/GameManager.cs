using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using System.Configuration;

namespace Assets.Scripts
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        public static GameManager Instance;
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

        public bool canPause;
        public bool gamePaused;
        public float gameTimer;

        public Transform playerSpawnPoint;
        CameraFollow cameraFollow;
        PlayerCharacter player;

        private void Start()
        {
            //StartReload();
            Initialize();
        }

        public void Initialize()
        {
            var existingPlayer = FindObjectOfType<PlayerCharacter>();
            if (existingPlayer != null)
            {
                Destroy(existingPlayer.parentObject);
            }

            var playerPrefab = Resources.Load<GameObject>("Player/Player");
            var playerObject = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            playerObject.transform.Rotate(0, 90, 0);
            playerObject.name = "Player";
            player = playerObject.GetComponentInChildren<PlayerCharacter>();
            player.InitializeStats();

            Combat.Instance.player = player;
            HUD.Instance.player = player;

            HUD.Instance.Init();
            CrystalController.Instance.Init();
            player.GetComponent<AttackController>().Init();

            cameraFollow = FindObjectOfType<CameraFollow>();
            cameraFollow.target = playerObject.transform;
            cameraFollow.ResetPosition();
            cameraFollow.offset = cameraFollow.transform.position - cameraFollow.target.position;

            canPause = true;
            gameTimer = 0;

            //MenuController.Instance.CloseMenuPanel(MenuController.Instance.gameMenu);

            player.renderer.enabled = true;
            player.isDead = false;
            player.canMove = true;
            player.inputSuspended = false;

        }

        void Update()
        {
            if (player != null)
            {
                if (!player.isDead)
                {
                    gameTimer += Time.deltaTime;
                }
            }

            HandleInput();
        }

        void HandleInput()
        {
            // Toggle HUD
            if (Input.GetKeyDown(KeyCode.LeftAlt) && Input.GetKeyDown(KeyCode.Z) && canPause)
            {
                MenuController.Instance.ToggleHUD();
            }

            // Pause and Show Main Menu
            if (Input.GetKeyDown(InputManager.Instance.menu_gamepad) || Input.GetKeyDown(InputManager.Instance.menu_keyboard)
                && !gamePaused && canPause)
            {
                Pause();
            }
            else if (Input.GetKeyDown(InputManager.Instance.menu_gamepad) || Input.GetKeyDown(InputManager.Instance.menu_keyboard)
                && gamePaused && canPause)
            {
                Unpause();
            }

            if (Input.GetKey(KeyCode.Escape))
            {
                Quit();
            }

            // Reload game
            if (Input.GetKeyDown(KeyCode.R))
            {
                //StartReload();
                Initialize();
            }

        }
        public void SetCursorVisibility(bool visible)
        {
            Cursor.visible = visible;
        }

        public void Pause()
        {
            //MenuController.Instance.CloseMenuPanel(MenuController.Instance.hudCanvasGroup);
           // MenuController.Instance.OpenMenuPanel(MenuController.Instance.gameMenu);

            Message.Instance.message.text = "PAUSE";

            SetCursorVisibility(true);
            gamePaused = true;
            Time.timeScale = 0;
            player.inputSuspended = true;
        }

        public void Unpause()
        {
            //MenuController.Instance.OpenMenuPanel(MenuController.Instance.hudCanvasGroup);
            //MenuController.Instance.CloseMenuPanel(MenuController.Instance.gameMenu);

            Message.Instance.message.text = "";

            SetCursorVisibility(false);
            gamePaused = false;
            Time.timeScale = 1;
            player.inputSuspended = false;

        }

        public void StartGameOver()
        {
            StartCoroutine(GameOver());
        }

        public IEnumerator GameOver()
        {
            var currentGameTime = gameTimer;
            var formattedTime = FormatTime(currentGameTime);
            Message.Instance.message.text = $"Survived for {formattedTime}";
            yield return new WaitForSeconds(6f);
            Message.Instance.message.text = "";
            Initialize();
        }

        public void Quit()
        {

#if UNITY_STANDALONE
                Application.Quit();
#endif

        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        public string FormatTime(float clock)
        {
            int minutes = Mathf.FloorToInt(clock / 60f);
            int seconds = Mathf.FloorToInt(clock - minutes * 60f);
            string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);
            return formattedTime;
        }
    }


}
