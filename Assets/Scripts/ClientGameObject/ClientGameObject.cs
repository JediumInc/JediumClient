using Akka.Interfaced;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientUI;
using Common.Logging;
using Domain.BehaviourMessages;
using Jedium.Behaviours;
using Jedium.Behaviours.Shared;
using Jedium.LocalBehaviours;
using UnityEngine;


namespace JediumCore
{
    public partial class ClientGameObject : AbstractActor, IGameObject, IGameObjectObserver,IClientGameObject
    {
        //log
        private ILog _log = LogManager.GetLogger(typeof(ClientGameObject).ToString());

        private GameObject obj;


        private ClientGameObjectUpdater _updater;

        private IGameObject _serverGameObject;

     
        private Guid _clientId;

        private string _prefab;
        private string _serverAddress;
        private ObjectSnapshot _snapshot;

     

        private Guid _bundleId;
        private Guid _avatarId;

        //special
        private string _avatarProps;

        
        public ClientGameObject(Guid localId, Guid ownerId,Guid clientId, Guid bundleId,Guid avatarId,string prefab, string address, ObjectSnapshot snap) : base(
            localId, ownerId)
        {
            _prefab = prefab;
            _clientId = Test.Instance._clientId;
            _snapshot = snap;
            _bundleId = bundleId;
            _clientId = clientId;
            _avatarId = avatarId;
        
          _log.Info($"Spawning game object {localId}, prefab {prefab}, address {address}, bundle {_bundleId}");


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
        async void SetupObject(string address)
        {
           
      
          
            GetAdressServer(address);
      
            #region Агент обьекта: соединение с серверным обьектом
      
            var asel = Context.ActorSelection(_serverAddress)
                .ResolveOne(TimeSpan.Zero).Result;
      
      
            _serverGameObject = asel.Cast<GameObjectRef>();

            _avatarProps = _serverGameObject.GetAvatarProps().Result;
            #endregion
      
            //Клиент: Регистрация соединения с серверным объектом
            _serverGameObject.RegisterClient(Test.Instance._clientId, CreateObserver<IGameObjectObserver>()).Wait();

            //Агент обьекта: спавн префаба
            await SpawnPrefab();

           
        }

        void GetAdressServer(string address)
        {
            string[] addr = address.Split('#');
            _serverAddress = addr[0].Substring(1);
            _serverAddress = _serverAddress.Replace("akka://VirtualFramework/",
                "akka.tcp://VirtualFramework@localhost:18095/");
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

              // obj.transform.position = new Vector3(_snapshot.X, _snapshot.Y, _snapshot.Z);
              // obj.transform.rotation = new Quaternion(_snapshot.RotX, _snapshot.RotY, _snapshot.RotZ, _snapshot.RotW);
              // obj.transform.localScale = new Vector3(_snapshot.ScaleX, _snapshot.ScaleY, _snapshot.ScaleZ);

                //we need to add the updater here

                _updater = obj.AddComponent<ClientGameObjectUpdater>();
                _updater.SetParent(this);
                // Создание Behaviours object - находимся в потоке Unity
                AddBehaviourComponents(obj);
            }
            else
            {
             
               // Debug.Log("__FROM BUNDLE:"+_bundleId);
                await new WaitForEndOfFrame(); //Переходим в поток Unity
               
                GameObject prot = await RootComponents.Instance.AssetLoader.GetWebAssetAsync<GameObject>(_prefab, _bundleId);

               

              //  Debug.Log("__INSTANTIATE:" + _bundleId);
                obj = GameObject.Instantiate(prot);
                //we need to add the updater here

              // obj.transform.position = new Vector3(_snapshot.X, _snapshot.Y, _snapshot.Z);
              // obj.transform.rotation = new Quaternion(_snapshot.RotX, _snapshot.RotY, _snapshot.RotZ, _snapshot.RotW);
              // obj.transform.localScale = new Vector3(_snapshot.ScaleX, _snapshot.ScaleY, _snapshot.ScaleZ);

                _updater = obj.AddComponent<ClientGameObjectUpdater>();
                _updater.SetParent(this);
                // Создание Behaviours object - находимся в потоке Unity
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

        async Task<Guid> IGameObject.GetBundleId()
        {
            return _bundleId;
        }

        async Task<int> IGameObject.GetMessageCount()
        {
            return 0;
        }

        async Task IGameObject.SaveToDB()
        {
            
        }

        async Task<string> IGameObject.GetNameOfPrefab()
        {
            return "";
        }

        async Task<Guid> IGameObject.GetAvatarId()
        {
            return _avatarId;
        }

        async Task<string> IGameObject.GetNameOfOthersPrefab()
        {
            return "";
        }

        async Task<ObjectSnapshot> IGameObject.GetSnapshot()
        {
            return _snapshot;
        }

        async Task<string> IGameObject.GetServerAddress()
        {
            return _serverAddress;
        }

        async Task<string> IGameObject.GetAvatarProps()
        {
            return "";
        }

        async Task IGameObject.SetAvatarProps(string props)
        {

        }
     

        #region Агент обьекта цикл: отсылка изменений состояния на сервер

    
        async Task IGameObject.SendBehaviourMessageToServer(Guid clientId, JediumBehaviourMessage message)
        {
            await _serverGameObject.SendBehaviourMessageToServer(clientId, message);
        }

        async Task IGameObject.SendBehaviourMessagePackToServer(Guid clientId, JediumBehaviourMessage[] messages)
        {
            await _serverGameObject.SendBehaviourMessagePackToServer(clientId, messages);
        }


        #endregion


        async Task IGameObject.RegisterClient(Guid clientId, IGameObjectObserver client)
        {
            _log.Error(".RegisterClient() should not be called on client!");
        }

        async Task IGameObject.UnregisterClient(Guid clientId)
        {
            _log.Error(".UnRegisterClient() should not be called on client!");
        }

        async Task IGameObject.DestroyObject()
        {
            _log.Error("DestroyObject() should not be called on client!");
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
    }
}