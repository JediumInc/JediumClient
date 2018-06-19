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

            // UnityMainThreadDispatcher.Instance().Enqueue(() => { StartCoroutine(GetAvatarProps()); });

           

            Debug.Log("__GOT AVATAR PARAMS:" +_jediumGameObject.AvatarProps);

       //    BasicAvatarProps bprops = new BasicAvatarProps()
       //    {
       //        MainColor = Color.red.FromColor(),
       //        SecondaryColor = Color.gray.FromColor()
       //    };
       //    Debug.Log("_________SET PROPS 1");
       //    string sprops = JsonConvert.SerializeObject(bprops);
       //
       //    Debug.Log("_________SET PROPS 2:"+sprops);
       //
       //    _jediumGameObject.AvatarProps = sprops;

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

        IEnumerator GetAvatarProps()
        {
            yield return new WaitForSeconds(5);
           
        }


    }

    public class BasicAvatarProps
    {
        public SerializableColor MainColor;
        public SerializableColor SecondaryColor;

    }
}
