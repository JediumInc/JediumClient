using System.Collections;
using System.Collections.Generic;
using Domain;
using Domain.BehaviourMessages;
using Jedium.Behaviours.CharacterController;
using Jedium.Behaviours.Shared;
using JediumCore;
using UnityEngine;

namespace Jedium.Behaviours
{




    public class JediumCharacterController : JediumBehaviour
    {
        public string jumpButton = "Jump";              // Default jump button.

        private float _lastV;
        private float _lastH;
        private bool _lastJump;

        private JediumCharacterAnimator _walkBeh;

       

        public override void Init(JediumBehaviourSnapshot snapshot)
        {
            base.Init(snapshot);
            _walkBeh = GetComponent<JediumCharacterAnimator>();
            if (Test.Instance._clientId == _parent.OwnerId)
            {
            //    JediumTransformBehaviour trans = GetComponent<JediumTransformBehaviour>();
             //   trans.AnimatorBased = true;
            }
            else
            {
                JediumTransformBehaviour trans = GetComponent<JediumTransformBehaviour>();
                trans.AnimatorBased = true;
                Animator a = GetComponent<Animator>();
                a.applyRootMotion = false;
            }

            _walkBeh.Init((Test.Instance._clientId == _parent.OwnerId));
        }

        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {
            if (!Initialized)
                return false;

            if (message == null)
                return false; //no empty

            if (message.GetBehaviourType() != GetComponentTypeIndex())
                return false;

            JediumCharacterControllerMessage msg = (JediumCharacterControllerMessage) message;

            //_charAnimator.SetFloat("V",msg.V);
            //_charAnimator.SetFloat("H",msg.H);
            //_charAnimator.SetBool("Jump",msg.Jump);
            _walkBeh.SetVH(msg.V, msg.H,msg.Jump);
            _lastV = msg.V;
            _lastH = msg.H;
            _lastJump = msg.Jump;
            //_charAnimator.SetBool("Jump",msg.);

            return true;
            // throw new System.NotImplementedException();
        }

        public void SetControllerParams(float v, float h, bool jump)
        {
            if (!Initialized)
                return;

            if (Test.Instance._clientId == _parent.OwnerId)
            {
                if (Mathf.Abs(_lastV - v) > 0.1f || Mathf.Abs(_lastH - h) > 0.1f || _lastJump != jump)
                {
                  
                    JediumCharacterControllerMessage msg = new JediumCharacterControllerMessage(v, h, jump);
                    _updater.AddUpdate(msg);
                }
            }
        }

        

        public override string GetComponentType()
        {
            return "CharacterController";
        }

        void Update()
        {

            if (!Initialized)
                return;

            if (Test.Instance._clientId == _parent.OwnerId)
            {
                float h = Input.GetAxisRaw("Horizontal");
                float v = Input.GetAxisRaw("Vertical");
                SetControllerParams(v, h, Input.GetButtonDown(jumpButton));
               
            }
        }
    }
}
