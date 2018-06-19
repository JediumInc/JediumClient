using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Domain;
using Domain.BehaviourMessages;
using Jedium.Behaviours.Shared;
using Jedium.Utils;
using UnityEngine;

namespace Jedium.Behaviours
{




    public class JediumTransformBehaviour : JediumBehaviour
    {

        private JediumTransformMessage _lastMessage = new JediumTransformMessage(0, 0, 0, 0, 0, 0, 1, 1, 1, 1);

        public bool AnimatorBased = false;


        public override string GetComponentType()
        {
            return "Transform";
        }

        public override void Init(JediumBehaviourSnapshot snapshot)
        {
            base.Init(snapshot);

            if (snapshot.BehaviourType != "Transform")
                return;
            JediumTransformSnapshot t = (JediumTransformSnapshot) snapshot;

            _lastMessage=new JediumTransformMessage(t.X,t.Y,t.Z,t.RotX,t.RotY,t.RotZ,t.RotW,t.ScaleX,t.ScaleY,t.ScaleZ);

            transform.position=new Vector3(t.X,t.Y,t.Z);
            transform.rotation=new Quaternion(t.RotX,t.RotY,t.RotZ,t.RotW);
            transform.localScale=new Vector3(t.ScaleX,t.ScaleY,t.ScaleZ);
        }

        private JediumAnimatorBehaviour _animator;

        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {

            if (Initialized)
            {
                //empty update
                //TODO - animator
             
                //don't process it for animator
                if (message == null)
                {
                    if (!AnimatorBased)
                    {
                        InternalSendMessage();
                    }

                    return false;

                    
                }

               

                //receive
                if (!(message is JediumTransformMessage))
                    return false;

                JediumTransformMessage tmsg = (JediumTransformMessage) message;
                InternalProcessMessage(tmsg);





                return true;
            }

            return false;
        }

        void InternalSendMessage()
        {

            if (_lastMessage.CheckChangeGameObjectBox(transform, .01f, 0.01f, 0.01f))
            {
               
                _lastMessage = transform.SetGameObjectFromTrasform();
                // print("moved");
                //_client.SetGameObjectDirect(_localId, transform.SetGameObjectFromTrasform());
                _updater.AddUpdate(transform.SetGameObjectFromTrasform());
            }
        }

        void InternalProcessMessage(JediumTransformMessage msg)
        {
            //вариант для owned - обьектов - только отправляем или посылаем, не вместе   

            if (JediumCore.Test.Instance._clientId != _parent.OwnerId)
            {
                //только получить значения

               



                transform.SetTransformFromGameObjectBox(msg);


                return;
            }

            //send
            InternalSendMessage();
        }
    }
}
