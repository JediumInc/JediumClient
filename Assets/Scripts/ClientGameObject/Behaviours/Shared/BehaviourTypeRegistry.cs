using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jedium.Behaviours.Shared
{
   public static class BehaviourTypeRegistry
   {
       public static Dictionary<string, Type> RegisteredBehaviourTypes = new Dictionary<string, Type>()
       {
           {"Transform", typeof(JediumTransformBehaviour)},
           {"Animation", typeof(JediumAnimatorBehaviour)},
           {"Touch", typeof(JediumTouchBehaviour)},
           {"CharacterController", typeof(JediumCharacterController)},
           {"Take", typeof(JediumTakeBehaviour) },
           {"Sit", typeof(JediumSitBehaviour) },
           {"UI",typeof(JediumUIBehaviour) }
           
       };
        public static Dictionary<string, Type> RegisteredSnapshotTypes = new Dictionary<string, Type>();
    }
}
