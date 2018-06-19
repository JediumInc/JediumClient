using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jedium.Behaviours.Shared
{
  public  interface IClientGameObject
    {

        Guid OwnerId { get; }
       Guid LocalId { get; }
    }
}
