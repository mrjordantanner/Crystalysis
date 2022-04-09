using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace Assets.Scripts
{
    /// <summary>
    /// Controls weapon attacks; receives input, instantiates AttackObject
    /// </summary>
    public class AttackController : MonoBehaviour
    {
        //public bool isCharging;
        //public bool meleeAttackCharged;
        //public float chargeTime = 2f;
        //public float chargeDelayTime = 0.5f;
        public float attackSpeed = 1f;
        public float attackSlowdown = 3f;
        public float baseAttackDuration = 0.1f;
        public float attackDuration = 0.1f;
        public bool isAttacking;

        public Transform startPosition, endPosition;

        public GameObject weaponObject;

        PlayerCharacter player;

        public Weapon weapon;

        private void Start()
        {
            player = FindObjectOfType<PlayerCharacter>();
            Init();
        }

        public void Init()
        {
            weapon.gameObject.SetActive(false);
            attackDuration = baseAttackDuration;
        }

        void Update()
        {
            HandleAttackInput();
            // HandleButtonCharging();
        }

        void HandleAttackInput()
        {
            if (Input.GetKeyDown(InputManager.Instance.meleeAttack_keyboard) || Input.GetKeyDown(InputManager.Instance.meleeAttack_gamepad))
            {
                if (
                    !player.inputSuspended && 
                    player.canAttack && 
                    player.canMove && 
                    !player.isDead &&
                    !GameManager.Instance.gamePaused)
                {
                    StartCoroutine(Attack());
                }
            }
        }

        IEnumerator Attack()
        {
             // Limit player movement during attack
            player.canAttack = false;
            isAttacking = true;
            if (player.isGrounded)
            {
                player.canMove = false;
            }

            player.animator.SetBool("isAttacking", isAttacking);
            player.animator.SetTrigger("Attack");

            // Wait for animation to begin before activating weapon object 
            //yield return new WaitForSeconds(attackDuration / 2);

            weapon.gameObject.SetActive(true);

            yield return new WaitForSeconds(attackDuration);

            weapon.gameObject.SetActive(false);

            //yield return new WaitForSeconds(attackReset);

            player.canAttack = true;
            player.canMove = true;
            isAttacking = false;

            player.animator.SetBool("isAttacking", isAttacking);
            player.animator.ResetTrigger("Attack");

        }

        #region Button Charging
        //IEnumerator ChargeDelay(float delay)
        //{
        //    isCharging = false;
        //    yield return new WaitForSeconds(delay);
        //    isCharging = true;
        //}

        //void HandleButtonCharging()
        //{
        //    if (Input.GetKeyDown(InputManager.Instance.meleeAttack_gamepad))
        //    {
        //        StartCoroutine(ChargeDelay(chargeDelayTime));
        //    }

        //    player.anim.SetBool("Charging", isCharging);

        //    if (InputManager.Instance.ButtonHoldCheck(InputManager.Instance.meleeAttack_gamepad, chargeTime - chargeDelayTime))
        //        meleeAttackCharged = true;

        //    if (meleeAttackCharged)
        //    {
        //        print("Melee Attack Charged");
        //        // TODO:  Play charged VFX and SFX

        //        // Charge released
        //        if (Input.GetKeyUp(InputManager.Instance.meleeAttack_gamepad))
        //        {

        //            if (player.grounded)
        //            {
        //                print("CHARGE ATTACK!!");
        //                // StartCoroutine(ChargedAttackGround());
        //                StartCoroutine(Attack());
        //            }
        //            else
        //            {
        //                print("AERIAL CHARGE ATTACK!!");
        //                // StartCoroutine(ChargedAttackAir());
        //                StartCoroutine(Attack());
        //            }

        //            meleeAttackCharged = false;
        //        }

        //    }

        //}
        #endregion
    }


}
