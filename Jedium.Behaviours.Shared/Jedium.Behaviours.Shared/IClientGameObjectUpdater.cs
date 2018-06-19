using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.BehaviourMessages;


namespace Jedium.Behaviours.Shared
{
   public interface IClientGameObjectUpdater
   {
       void RegisterComponent(JediumBehaviour behaviour);

       void AddUpdate(JediumBehaviourMessage message);
   }
}
