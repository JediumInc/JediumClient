using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Interfaced;
using Common.Logging;
using Domain;
using UnityEngine;
using UnityEngine.SceneManagement;
using DelegateCommandImpl;
namespace JediumCore
{
    public class ClientConnection : InterfacedActor, IClientConnection, IConnectionObserver
    {
        //log
        private ILog _log = LogManager.GetLogger(typeof(ClientConnection).ToString());

        private IConnection _server;
        private Guid _clientId;
        private ISceneActor _scene;

        private Guid _sceneId; //"0b5fd70d-a95b-4e0f-b518-17b1e67e73d7"


       


        public Dictionary<Guid, IGameObject> _spawnedObjects;

        public ClientConnection(IConnection server, Guid clientId,Guid sceneId)
        {
            _server = server;
            _clientId = clientId;
            _sceneId = sceneId;

            _spawnedObjects = new Dictionary<Guid, IGameObject>();
            _scene = _server.RegisterClient(_clientId,_sceneId,
                CreateObserver<IConnectionObserver>()).Result;


            string scenename = _scene.GetSceneName().Result;
            Guid scenebunle = _scene.GetBundleId().Result;
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
              LoadScene(scenename,scenebunle);
            });

          
//            Debug.Log(_scene.GetSceneName().Result + "Name of scnce");
        }

        async void LoadScene(string name, Guid bundleId)
        {
            if (bundleId == Guid.Empty)
            {
               await UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_scene.GetSceneName().Result,
                    LoadSceneMode.Additive);
                _server.NotifySceneLoaded(_clientId, _sceneId,Test.Instance.Username).Wait();
            }
            else
            {
                string scene = await RootComponents.Instance.AssetLoader.LoadSceneFromWebAsync(bundleId, name);
                if (!String.IsNullOrEmpty(scene))
                {
                   await SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
                    _server.NotifySceneLoaded(_clientId, _sceneId, Test.Instance.Username).Wait();
                }
                else
                {
                    _log.Error("Can't load scene:"+name);
                }
            }

           
        }


        void IConnectionObserver.ClientLoggedIn(Guid clientid)
        {
            _log.Info("Client logged in:" + clientid.ToString());
        }

        #region Клиент: создание агента обьекта

        void IConnectionObserver.OnSpawnedGameObject(string namePrefab, string nameNotOwnedPrefab, Guid localId, Guid ownerId,Guid bundleId, Guid avatarId,string address,
            ObjectSnapshot snap)
        {
          //TODO - do we need this for avatars only?
            if (ownerId != _clientId && ownerId != Guid.Empty&&avatarId!=Guid.Empty)
                namePrefab = nameNotOwnedPrefab;
            //

            //TODO - address is obtainable here, but not the methods somehow. Maybe Protobuf?
           // IGameObject sObj=Sender.Cast<GameObjectRef>();


       

            
            var obj = Test.Instance.AkkaSystem
                .ActorOf(Props.Create(() => new ClientGameObject(localId, ownerId,_clientId,bundleId,avatarId, namePrefab, address, snap)),
                    localId.ToString()).Cast<GameObjectRef>();


           

            _spawnedObjects.Add(localId, obj);
        }

        

        #endregion

        void IConnectionObserver.KillOwnedObjects(Guid clientId)
        {
            foreach (var obj in _spawnedObjects)
            {
                if (obj.Value.GetOwnerId().Result == clientId)
                {
                    obj.Value.DestroyObject().Wait();
                }
            }
        }

        async Task IClientConnection.RegisterConnection(Guid clientID, string scene)
        {
            await _server.RegisterClient(_clientId, Guid.Parse("0b5fd70d-a95b-4e0f-b518-17b1e67e73d7"),
                CreateObserver<IConnectionObserver>());
        }

        Task IClientConnection.KillConnection()
        {
           Self.Tell(InterfacedPoisonPill.Instance);
            //Self.GracefulStop()
            return Task.FromResult(true);
        }
    }
}