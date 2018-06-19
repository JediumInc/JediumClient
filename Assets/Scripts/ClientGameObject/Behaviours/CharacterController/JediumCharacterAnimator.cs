using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Jedium.Behaviours.CharacterController
{
    public abstract  class JediumCharacterAnimator:MonoBehaviour
    {
        public abstract void SetVH(float V, float H,bool J);
        public abstract void Init(bool isOwner);
        public abstract void SetJump(bool jump);
    }
}
