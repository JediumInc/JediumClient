using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using JediumCore;
using UnityEngine;

namespace Jedium.LocalBehaviours
{
   public abstract class JediumLocalBehaviour:MonoBehaviour
   {
       public bool Initialized = false;

       protected ClientGameObject _jediumGameObject;

       public virtual void Init(ClientGameObject jgo)
       {
           _jediumGameObject = jgo;
           Initialized = true;
       }

   }
}
