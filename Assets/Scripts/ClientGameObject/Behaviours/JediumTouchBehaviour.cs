using System;
using System.Collections;
using System.Collections.Generic;
using Common.Logging;
using Domain;
using Domain.BehaviourMessages;
using Jedium.Behaviours.Shared;
using JediumCore;
using UnityEngine;

namespace Jedium.Behaviours
{




    public class JediumTouchBehaviour : JediumBehaviour
    {
        private ILog _log = LogManager.GetLogger(typeof(JediumTouchBehaviour).ToString());




        //Simple touch implementation
        //TODO - for tests only!
        void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {

                JediumTouchMessage touch = new JediumTouchMessage(Test.Instance._clientId, 0f, 0f);
                _updater.AddUpdate(touch);
            }
        }

        public override string GetComponentType()
        {
            return "Touch";
        }

        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {
            if (Initialized)
            {
                if (message == null)
                    return false; //no empty update part

                if (!(message is JediumTouchMessage))
                    return false;

                JediumTouchMessage tmsg = (JediumTouchMessage) message;

                _log.Info("________TOUCHED:" + this.name);
                return true;
            }

            return false;
        }

        //to build context menu
        public override Tuple<string, string> GetComponentAction()
        {
            return Tuple.Create("Touch", "Touch");
        }
    }
}
