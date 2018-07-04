using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jedium.Utils;
using Domain;
using JediumCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;


namespace Jedium.LocalBehaviours
{
   public  class BasicAvatarSettings:JediumLocalBehaviour
   {

       public SkinnedMeshRenderer MainMaterial;
       public SkinnedMeshRenderer SecondaryMaterial;
        public override void Init(ClientGameObject jgo)
        {
            base.Init(jgo);


            Debug.Log("__GOT AVATAR PARAMS:" +_jediumGameObject.AvatarProps);


            BasicAvatarProps avProps = JsonConvert.DeserializeObject<BasicAvatarProps>(_jediumGameObject.AvatarProps);

            if (avProps != null)
            {

                if (MainMaterial != null)
                {
                    Material mat=new Material(MainMaterial.material);
                    mat.color= avProps.MainColor.ToColor();
                    MainMaterial.material = mat;
                }

                if (SecondaryMaterial != null)
                {
                    Material mat = new Material(SecondaryMaterial.material);
                    mat.color = avProps.SecondaryColor.ToColor();
                    SecondaryMaterial.material = mat;
                }
            }


        }

       protected override void OnUpdate()
        {
           
        }


       


    }

    public class BasicAvatarProps
    {
        public SerializableColor MainColor;
        public SerializableColor SecondaryColor;

    }
}
