using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Interfaced;
using Domain;
using Domain.BehaviourMessages;
using Jedium.Behaviours.Shared;
using UnityEngine;

namespace JediumCore
{
    public partial class ClientGameObject : InterfacedActor, IGameObject, IGameObjectObserver, IClientGameObject,IAbstractActor
    {
        #region Direct access



        public Guid GetIdDirect()
        {
            return _localId;
        }

    

        public Guid GetOwnerIdDirect()
        {
            return _ownerId;
        }

        public Guid OwnerId
        {
            get { return _ownerId; }
        }

        public Guid LocalId
        {
            get { return _localId; }
        }


        public string AvatarProps
        {
            get { return _avatarProps; }
            set
            {
                _log.Info($"Setting avatar props:{value}");
                _avatarProps = value;
               // _serverGameObject.SetAvatarProps(_avatarProps).Wait();
            }
        }

        public GameObject GetObj
        {
            get { return obj; }

        }

        #endregion
    }
}
