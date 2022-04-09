using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class InputManager : MonoBehaviour
    {
        #region Singleton
        public static InputManager Instance;
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

        [Header("Keyboard Input")]
        public KeyCode jump_keyboard;
        public KeyCode meleeAttack_keyboard, skill_keyboard, dash_keyboard, menu_keyboard;

        [Header("Gamepad Input")]
        public KeyCode jump_gamepad;
        public KeyCode meleeAttack_gamepad, skill_gamePad, dash_gamepad, menu_gamepad;

        private float chargeCounter;

        //public bool ButtonHoldCheck(KeyCode button, float chargeDuration)
        //{
        //    if (Input.GetKey(button))
        //    {
        //        chargeCounter += Time.deltaTime;

        //        if (chargeCounter < chargeDuration)
        //        {
        //            // Charging...
        //            return false;
        //        }
        //        else
        //        {
        //            // Charged
        //            return true;
        //        }
        //    }

        //    if (Input.GetKeyUp(button))
        //    {
        //        // Charged attack
        //        //  if (chargeCounter >= chargeDuration)
        //        //  {
        //        //
        //        //  }

        //        // Button released
        //        chargeCounter = 0;

        //    }

        //    return false;
        //}




    }
}
