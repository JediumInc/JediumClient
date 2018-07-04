using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Configuration;
using Akka.Interfaced;
using ClientUI;
using Common.Logging;
using Domain;
using Jedium.Behaviours.Shared;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Logger = UnityEngine.Logger;

namespace JediumCore
{
    public class Test : MonoBehaviour
    {

        //log
        private ILog _log = LogManager.GetLogger(typeof(Test).ToString());

        public JediumSettings MainSettings;

        public static Test Instance;

        public ConnectionRef MainServer;
        private IClientConnection _mainClient;
        private ISceneActor _scene;
        public ActorSystem AkkaSystem;



        public Guid _clientId;
        public Guid UserId = Guid.Empty;

        public string Username;
        public string Password;

        private bool allowQuit = false;

        private string behavioursPath = @"..\BehaviourLoadTest";


        //avatars
        //TODO
        //in fact, using ClientGameObject (not IGameObject) here is a hack to get access to Unity GameObject.
        public ClientGameObject _myAvatar;

        public Dictionary<Guid, ClientGameObject> _otherAvatars;

        //
        // string _localUrl = "akka.tcp://VirtualFramework@expovirtual.ru:18095/user/ServerEndpoint";
      //  string _localUrl = "akka.tcp://VirtualFramework@localhost:18095/user/ServerEndpoint";
        // Use this for initialization

        void Awake()
        {
            Instance = this;

            _otherAvatars=new Dictionary<Guid, ClientGameObject>();
        }

        async void Start()
        {


           
            _log.Info("-------LOG STARTED----");

            MainUI.Instance.ShowLoginWindow("","");

            //load behaviours

            if (Directory.Exists(Path.Combine(Application.dataPath,behavioursPath)))
            {
                BehaviourManager.LoadBehaviours(Path.Combine(Application.dataPath, behavioursPath));
          
            }
            else
            {
                _log.Info($"Path {Path.Combine(Application.dataPath, behavioursPath)} not exists, skipping assembly loading");
            }



            string sconfig = @"
         akka{
                  actor{
                 
              serializers
                           {
                            hyperion = ""Akka.Serialization.HyperionSerializer, Akka.Serialization.Hyperion""
                           }
                         serialization-bindings {
                           ""System.Object"" = hyperion
                           }

    provider=""Akka.Remote.RemoteActorRefProvider,Akka.Remote""
                                                 
                        }
                
                 remote{
                    helios.tcp{
                            transport-class = ""Akka.Remote.Transport.Helios.HeliosTcpTransport, Akka.Remote""
                        port =18098

                        hostname=localhost
                        }
                    }
                }
      ";

            var config = ConfigurationFactory.ParseString(sconfig);

            _clientId = Guid.NewGuid();

            try
            {
                AkkaSystem = ActorSystem.Create("JediumClient" + _clientId, config);
            }
            catch (Exception e)
            {
                Debug.Log(e);
                // throw;
            }

            DeadRequestProcessingActor.Install(AkkaSystem);

         
        }

        public async Task DoLogin(string name, string password)
        {

            Username = name;
            Password = password;
            try
            {
                string serverHash = RootComponents.Instance.AssetLoader.GetServerHash();
                if (serverHash != RootComponents.Instance.ClientHash)
                {
                    MainUI.Instance.ShowMessageBox($"Client/server incompatibility: \n {serverHash} \n {RootComponents.Instance.ClientHash}",
                        () => { MainUI.Instance.ShowLoginWindow(Username,Password); });
                    return;
                }
            }
            catch (Exception e)
            {

                if (e is SocketException)
                {
                    MainUI.Instance.ShowMessageBox($"Can't connect to server: {e.Message}",
                        () => { MainUI.Instance.ShowLoginWindow(Username,Password); });
                    return;
                }
                else
                {
                    MainUI.Instance.ShowMessageBox($"Error: {e.Message}",
                        () => { MainUI.Instance.ShowLoginWindow(Username,Password); });
                    return;
                }
              
            }
          

           

            _log.Info($"Logging in, login {name}, password {password}");

            
            try
            {
                IActorRef asel =
                    AkkaSystem.ActorSelection(MainSettings.ServerUrl)
                        .ResolveOne(TimeSpan.Zero).Result;

                MainServer = asel.Cast<ConnectionRef>();

                Tuple<bool,string, ServerInfo> loginInfo = await MainServer.DoLogin(name,password);



                if (!loginInfo.Item1)
                {
                    MainUI.Instance.ShowMessageBox(loginInfo.Item2,
                        () => { MainUI.Instance.ShowLoginWindow(Username,Password); });
                    return;
                }

                UserId = loginInfo.Item3._loggedInUserId;

                foreach (var atype in loginInfo.Item3.AdditionalRegisteredBehaviours)
                {
                    _log.Info($"Adding behavior type from plugin: {atype.Value}");
                    ///   TYPEBEHAVIOUR.AddRegisteredType();
                    TYPEBEHAVIOUR.AddRegisteredTypeAndIndex(atype.Key, atype.Value);
                }

                //now start the connection process

                _mainClient = AkkaSystem
                    .ActorOf(Props.Create(() => new ClientConnection(MainServer, _clientId,Guid.Parse(MainSettings.InitialScene))), "ClientConnection")
                    .Cast<ClientConnectionRef>();

                //await _mainClient.RegisterConnection(Guid.Empty, "");




            }
            catch (Exception e)
            {
                MainUI.Instance.ShowMessageBox(e.ToString(), () => {MainUI.Instance.ShowLoginWindow(Username,Password); });
                Debug.Log(e);
                //  throw;
            }

            //start updates
           //if (MainSettings.UseUpdateThread)
           //{
           //    _updateInterval = MainSettings.UpdateThreadInterval;
           //    _updateThread = new Thread(this.TickUpdaters);
           //    _updateThread.Start();
           //}
        }
            

      // async void OnDestroy()
      // {
      //
      //     _doUpdates = false;
      //     //Клиент: запрос на выход
      //     if (MainServer != null)
      //     {
      //         _log.Info("Unregistering client");
      //         await MainServer.DoLogout(_clientId);
      //
      //         _log.Info("Shutting down connection");
      //         await _mainClient.KillConnection();
      //     }
      //
      //
      //     _log.Info("Terminating system");
      //     //Клиент: остановка Akka - системы и выход
      //     AkkaSystem.Terminate().Wait();
      //
      //     _log.Info("Shutdown complete");
      // }

        void OnApplicationQuit()
        {
            ProcessShutdown();

            if(!allowQuit)
                Application.CancelQuit();
        }

        void ProcessShutdown()
        {
            _doUpdates = false;

            //Клиент: запрос на выход
            if (MainServer != null&&UserId!=Guid.Empty)
            {
                _log.Info("Unregistering client");
              //  MainServer.DoLogout(_clientId,UserId).Wait();
                MainServer.CastToIActorRef().Tell(new LogoutMessage()
                {
                    ClientId =_clientId,
                    UserId = this.UserId
                },null);

                _log.Info("Shutting down connection");
                _mainClient.KillConnection().Wait();
            }

            //Клиент: остановка Akka - системы и выход
            AkkaSystem.Terminate().Wait();

            _log.Info("Shutdown complete");
            allowQuit = true;
            Application.Quit();
        }

        #region Updates thread

        private bool _doUpdates = true;

        private List<ClientGameObjectUpdater> _updaters;

      // private Thread _updateThread;
      //
      // private Stopwatch _updateWatch;
      //
      // private long _updateInterval; //cache it

        public void RegisterUpdater(ClientGameObjectUpdater upd)
        {
            if(_updaters==null)
                _updaters=new List<ClientGameObjectUpdater>();

            lock (_updaters)
            {
                _updaters.Add(upd);
            }
        }

    // void TickUpdaters()
    // {
    //     _updateWatch=new Stopwatch();
    //
    //     _updateWatch.Start();
    //
    //     while (_doUpdates)
    //     {
    //
    //         if (_updateWatch.ElapsedMilliseconds > _updateInterval)
    //         {
    //            
    //
    //             if (_updaters != null)
    //             {
    //                 for (int i = 0; i < _updaters.Count; i++)
    //                     _updaters[i].SendMessages();
    //             }
    //
    //             _updateWatch.Restart();
    //         }
    //
    //
    //
    //     }
    // }

        #endregion
    }
}

