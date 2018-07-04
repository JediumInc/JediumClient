using Akka.Interfaced;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using ClientUI;
using Common.Logging;
using DelegateCommandImpl;
using Domain.BehaviourMessages;

using Jedium.Behaviours;
using Jedium.Behaviours.Shared;
using Jedium.LocalBehaviours;
using UnityEngine;


namespace JediumCore
{
    public partial class ClientGameObject : InterfacedActor, IGameObject, IGameObjectObserver,IClientGameObject,IAbstractActor
    {
        //log
        private ILog _log = LogManager.GetLogger(typeof(ClientGameObject).ToString());

        private GameObject obj;


        private ClientGameObjectUpdater _updater;


        private ClientConnectionHolderRef _serverConnection;
     
        private Guid _clientId;
        private Guid _ownerId;
        private Guid _localId;

        private string _prefab;
        private string _serverAddress;
        private ObjectSnapshot _snapshot;

     

        private Guid _bundleId;
        private Guid _avatarId;

        //special
        private string _avatarProps;

        private readonly TickMessage _tick = new TickMessage();
      
        
        public ClientGameObject(Guid localId, Guid ownerId,Guid clientId, Guid bundleId,Guid avatarId,string prefab, string address, ObjectSnapshot snap) 
        {
            _prefab = prefab;
            _clientId = Test.Instance._clientId;
            _snapshot = snap;
            _bundleId = bundleId;
            _clientId = clientId;
            _avatarId = avatarId;
            _serverAddress = address;
            _ownerId = ownerId;
            _localId = localId;
          
           
        
          _log.Info($"Spawning game object {localId}, prefab {prefab}, address {address}, bundle {_bundleId}");


            //and start ticking
            if(Test.Instance.MainSettings.UseUpdateThread)
            Context.System.Scheduler.ScheduleTellRepeatedly(TimeSpan.FromSeconds(0), TimeSpan.FromMilliseconds(Test.Instance.MainSettings.UpdateThreadInterval), Self, _tick, ActorRefs.NoSender);


            if (avatarId != Guid.Empty)
            {
                if (_clientId == localId)
                {
                    _log.Info($"Spawned player avatar");
                    Test.Instance._myAvatar = this;
                }
                else
                {
                    _log.Info($"Spawned other player avatar: {localId}");
                    Test.Instance._otherAvatars.Add(localId, this);
                }
            }

            SetupObject(address);
           
        }



        //REQUIRED - runs async code
         void SetupObject(string address)
        {
           
      
          
       
      
            #region Агент обьекта: соединение с серверным обьектом
      
           
         var asel = Context.ActorSelection(_serverAddress)
             .ResolveOne(TimeSpan.Zero).Result;
        
        IGameObject _serverGameObject = asel.Cast<GameObjectRef>();
            

            _avatarProps = _serverGameObject.GetAvatarProps().Result;
            #endregion
      
            //Клиент: Регистрация соединения с серверным объектом
            _serverGameObject.RegisterClient(Test.Instance._clientId, CreateObserver<IGameObjectObserver>()).Wait();

          

           
        }
     

       async void IGameObjectObserver.GotAddress()
        {
            string address = Sender.Path.ToString();

            var asel = Context.ActorSelection(address)
                .ResolveOne(TimeSpan.Zero).Result;
            _serverConnection = asel.Cast<ClientConnectionHolderRef>().WithRequestWaiter(this);

            await SpawnPrefab();
        }

       

        /// <summary>
        /// Агент обьекта: спавн префаба
        /// </summary>
      async Task SpawnPrefab()
        {

          
            if (_bundleId == Guid.Empty)//не бандл
            {
                //Переходим в поток Unity
                await new WaitForEndOfFrame();

                GameObject prot = Resources.Load<GameObject>(_prefab);
               
                
                obj = GameObject.Instantiate(prot);

             
              
                _updater = obj.AddComponent<ClientGameObjectUpdater>();
                _updater.SetParent(this);
                // Создание Behaviours object - находимся в потоке Unity
                AddBehaviourComponents(obj);
            }
            else
            {
             
              
                await new WaitForEndOfFrame(); //Переходим в поток Unity
               
                GameObject prot = await RootComponents.Instance.AssetLoader.GetWebAssetAsync<GameObject>(_prefab, _bundleId);

               

             
                obj = GameObject.Instantiate(prot);



              
                _updater = obj.AddComponent<ClientGameObjectUpdater>();
                _updater.SetParent(this);
                // Создание Behaviours object - находимся в потоке Unity
               // await AddBehaviourComponents(obj);
                AddBehaviourComponents(obj);

                
            }
        }

        /// <summary>
        /// Создание Behaviours object
        /// </summary>
       void AddBehaviourComponents(GameObject pobj)
        {

        //process already added components
            List<JediumBehaviour> componentList = new List<JediumBehaviour>();
            componentList = pobj.gameObject.GetComponentsInChildren<JediumBehaviour>().ToList();

            foreach (JediumBehaviour o in componentList) 
            {
                o.SetParent( this);
                o.SetUpdater(_updater);
            }
           
           
            //NEW WAY

            foreach (var snap in _snapshot.Snapshots)
            {
                if (BehaviourTypeRegistry.RegisteredBehaviourTypes.ContainsKey(snap.Key))
                {
                    Type ctype = BehaviourTypeRegistry.RegisteredBehaviourTypes[snap.Key];
                    JediumBehaviour oBeh = (JediumBehaviour) pobj.AddComponent(ctype);
                    oBeh.SetParent(this);
                    oBeh.SetUpdater(_updater);
                    componentList.Add(oBeh);
                }
                else
                {
                    _log.Error($"Can't find type for behaviour with name {snap.Key}");
                }
            }

            ////run Init() on them
             foreach (var comp in componentList)
             {
                 //JediumBehaviourSnapshot snap = null;
                
                // if (_snapshot.Snapshots.ContainsKey(comp.GetComponentType()))
                  JediumBehaviourSnapshot   snap = _snapshot.Snapshots[comp.GetComponentType()];
                 comp.Init(snap);
            
               
             }

            var localBehaviours = pobj.gameObject.GetComponents<JediumLocalBehaviour>();

            foreach (var lcomp in localBehaviours)
            {
                lcomp.Init(this);
            }

            //add context menu
            pobj.gameObject.AddComponent<MainGUIContextMenu>();

            
           
        }




        #region Implement IGameObject interface

        Task IGameObject.TickBehaviours()
        {
            return Task.FromResult(true);
        }

        //editor-related
        public Task SetBehaviourSnapshot(JediumBehaviourSnapshot snapshot)
        {
            return Task.FromResult(false);
        }

         Task<Guid> IGameObject.GetBundleId()
        {
            return Task.FromResult(_bundleId);
        }

         Task<int> IGameObject.GetMessageCount()
        {
            return Task.FromResult(0);
        }

        Task IGameObject.SaveToDB()
        {
            return Task.FromResult(false);
        }

         Task<string> IGameObject.GetNameOfPrefab()
        {
            return Task.FromResult("");
        }

         Task<Guid> IGameObject.GetAvatarId()
        {
            return Task.FromResult( _avatarId);
        }

        Task<string> IGameObject.GetNameOfOthersPrefab()
        {
            return Task.FromResult("");
        }

         Task<ObjectSnapshot> IGameObject.GetSnapshot()
        {
            return Task.FromResult(_snapshot);
        }

        Task<string> IGameObject.GetServerAddress()
        {
            return Task.FromResult(_serverAddress);
        }

        Task<string> IGameObject.GetAvatarProps()
        {
            return Task.FromResult("");
        }

        Task IGameObject.SetAvatarProps(string props)
        {
            return Task.FromResult(false);
        }
     

        #region Агент обьекта цикл: отсылка изменений состояния на сервер

    
        Task IGameObject.SendBehaviourMessageToServer(Guid clientId, JediumBehaviourMessage message)
        {
            // _serverGameObject.SendBehaviourMessageToServer(clientId, message).Wait(100); //TODO - cancellation time?
           // _serverConnection
            return Task.FromResult(true);
        }

        Task IGameObject.SendBehaviourMessagePackToServer(Guid clientId, JediumBehaviourMessage[] messages)
        {
           // _serverGameObject.SendBehaviourMessagePackToServer(clientId, messages).Wait(100);

          //  Debug.Log("___SENDING 2");
          //  _serverConnection.CastToIActorRef().Tell(new BehaviourMessagePack()
          //  {
          //      Messages = messages
          //  }, Self);//SendMessagePack(messages);
            _serverConnection.WithNoReply().SendMessagePack(messages);
            return Task.FromResult(true);
        }


        #endregion


        Task IGameObject.RegisterClient(Guid clientId, IGameObjectObserver client)
        {
            _log.Error(".RegisterClient() should not be called on client!");
            return Task.FromResult(false);
        }

         Task IGameObject.UnregisterClient(Guid clientId)
        {
            _log.Error(".UnRegisterClient() should not be called on client!");
            return Task.FromResult(false);
        }

     

        Task IGameObject.DestroyObject()
        {
          
            if (Test.Instance._otherAvatars.ContainsKey(LocalId))
            {
                _log.Info($"User {LocalId} logged out");
                Test.Instance._otherAvatars.Remove(LocalId);
            }

            UnityMainThreadDispatcher.Instance().Enqueue(() => { GameObject.DestroyImmediate(obj); });

            Self.Tell(InterfacedPoisonPill.Instance, Self);

            return Task.FromResult(true);
        }

        #endregion


        #region Implement IGameObjectObserver  

        #region Агент обьекта: получение состояния с сервера

     

        void IGameObjectObserver.SendBehaviourMessageToClient(JediumBehaviourMessage message)
        {
            _messageQueue.Enqueue(message);
        }

        void IGameObjectObserver.SendBehaviourMessagePackToClient(JediumBehaviourMessage[] messages)
        {
            //_messagePackQueue.Enqueue(messages);
            AddReceivedMessagePack(messages);
        }

        

        #endregion





        void IGameObjectObserver.DestroyObject()
        {

            if (Test.Instance._otherAvatars.ContainsKey(LocalId))
            {
                _log.Info($"User {LocalId} logged out");
                Test.Instance._otherAvatars.Remove(LocalId);
            }

            UnityMainThreadDispatcher.Instance().Enqueue(() => { GameObject.DestroyImmediate(obj); });

            Self.Tell(InterfacedPoisonPill.Instance, Self);
        }

        #endregion

        Task<Guid> IAbstractActor.GetGuid()
        {
            return Task.FromResult(_localId);
        }

        Task<Guid> IAbstractActor.GetOwnerId()
        {
            return Task.FromResult(_ownerId);
        }
    }
}