using System.Collections;
using System.Collections.Generic;
using Domain;
using Domain.BehaviourMessages;
using Jedium.Behaviours;
using Jedium.Behaviours.Shared;
using JediumCore;
using UnityEngine;

namespace JediumCore
{

    //TODO - think about synced/ECS updates
    public class ClientGameObjectUpdater : MonoBehaviour,IClientGameObjectUpdater
    {


        // Use this for initialization
        private ClientGameObject _parent;
        public float deltaTime = 0.01f;

        public bool UseFixedUpdate = false;

        private float _currTime = 0;

        private Dictionary<int,JediumBehaviour> _registeredComponents;

        private bool _useUpdateThread; //cache the setting

        void Awake()
        {
            _registeredComponents=new Dictionary<int, JediumBehaviour>();
        }

        public void RegisterComponent(JediumBehaviour cmpnt)
        {
            _registeredComponents.Add(cmpnt.GetComponentTypeIndex(),cmpnt);
        }


        public void AddUpdate(JediumBehaviourMessage obj)
        {
            _parent.AddSendMessage(obj);
        }

        public void SetParent(ClientGameObject parent)
        {
            _parent = parent;
        }

        void Start()
        {
            if (Test.Instance.MainSettings != null)
            {
                deltaTime = Test.Instance.MainSettings.UpdateRate;
                UseFixedUpdate = Test.Instance.MainSettings.UseFixedUpdate;
            }

            _useUpdateThread = Test.Instance.MainSettings.UseUpdateThread;
            if(_useUpdateThread)
            Test.Instance.RegisterUpdater(this);
        }

        public bool HaveComponent(int t)
        {
            return _registeredComponents.ContainsKey(t);

        }

        public JediumBehaviour GetComponent(int t)
        {
            if (_registeredComponents.ContainsKey(t))
                return _registeredComponents[t];

            return null;
        }


        public void UpdateAll()
        {
            foreach (var comp in _registeredComponents)
            {
                JediumBehaviourMessage[] pmsg = _parent.GetLastMessagePack(comp.Key);

                if (pmsg != null)
                {
                    for (int i = 0; i < pmsg.Length; i++)
                    {
                        comp.Value.ProcessUpdate(pmsg[i]);
                    }
                }
                else
                {
                    comp.Value.ProcessUpdate(null);
                }
            }


            // JediumBehaviourMessage msg = _parent.GetLastMessage();
            //
            // if (msg != null)
            // {
            //     bool processed = false;
            //     foreach(var comp in _registeredComponents)
            //     {
            //         bool pbyc= comp.Value.ProcessUpdate(msg);
            //         processed = (processed || pbyc);
            //
            //         if(pbyc) //TODO if it works
            //             break;
            //     }
            //
            //     if (!processed)
            //     {
            //         Debug.Log("Unprocessed message of type:"+msg.GetType());
            //     }
            // }
            // else
            // {
            //     //run empty updates
            //     foreach (var comp in _registeredComponents)
            //     {
            //         comp.Value.ProcessUpdate(null);
            //     }
            // }

            if(!_useUpdateThread)
                SendMessages();

           
        }

        public void SendMessages()
        {
            if (_parent == null)
            {
                Debug.LogError("Updater: client game object is null!");
                return;
            }

            if (!_useUpdateThread)
            {
                _currTime = _currTime + Time.deltaTime;

                if (_currTime > deltaTime)
                {

                    //tick update
                    _parent.SendTick();
                }
            }
            else
            {
                //tick update
                _parent.SendTick();
            }
        }

        // Update is called once per frame
        void Update()
        {
            UpdateAll();
            //receive messages

            //process packs

        //   foreach (var comp in _registeredComponents)
        //   {
        //       JediumBehaviourMessage[] pmsg = _parent.GetLastMessagePack(comp.Key);
        //
        //       if (pmsg != null)
        //       {
        //           for (int i = 0; i < pmsg.Length; i++)
        //           {
        //               comp.Value.ProcessUpdate(pmsg[i]);
        //           }
        //       }
        //       else
        //       {
        //           comp.Value.ProcessUpdate(null);
        //       }
        //   }
        //
        //
        //
        //
        //
        //
        //   //send messages
        //   if (!UseFixedUpdate)
        //   {
        //       if (_parent == null)
        //       {
        //           Debug.LogError("Updater: client game object is null!");
        //           return;
        //       }
        //
        //       _currTime = _currTime + Time.deltaTime;
        //
        //       if (_currTime > deltaTime)
        //       {
        //           //tick update
        //           _parent.SendTick();
        //       }
        //   }

        }

        void FixedUpdate()
        {
            if (UseFixedUpdate)
            {
                if (_parent == null)
                {
                    Debug.LogError("Updater: client game object is null!");
                    return;
                }

                _currTime = _currTime + Time.fixedDeltaTime;

                if (_currTime > deltaTime)
                {
                    //tick update
                    _parent.SendTick();
                }
            }
        }
    }
}

