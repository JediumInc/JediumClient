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
    public partial class ClientGameObject : AbstractActor, IGameObject, IGameObjectObserver, IClientGameObject
    {
        #region Direct access



        public Guid GetIdDirect()
        {
            return _localID;
        }

      // public void SetGameObjectDirect(Guid clientId, JediumTransformSnapshot gameObject)
      // {
      //     var tt = _serverGameObject.SetTransform(clientId, gameObject);
      //     tt.Wait();
      // }

        public Guid GetOwnerIdDirect()
        {
            return _ownerID;
        }

        public Guid OwnerId
        {
            get { return _ownerID; }
        }

        public Guid LocalId
        {
            get { return _localID; }
        }


        public string AvatarProps
        {
            get { return _avatarProps; }
            set
            {
                _log.Info($"Setting avatar props:{value}");
                _avatarProps = value;
                _serverGameObject.SetAvatarProps(_avatarProps).Wait();
            }
        }

        public GameObject GetObj
        {
            get { return obj; }

        }

        #endregion
    }
}
