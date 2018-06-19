using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain;
using JediumCore;
using UnityEngine;



public class TestController : MonoBehaviour
{
    //public IGameObject Client;
   // Guid _localId;
   //
   // void Start()
   // {
   //     _localId = _parent.GetIdDirect();
   // }
   //
   // private Transform _buf;

    

    // Update is called once per frame
    void Update()
    {
        //вариант для owned - обьектов - только отправляем или посылаем, не вместе   

     //  if (JediumCore.Test.Instance._clientId !=_parent.GetOwnerIdDirect())
     //  {
     //      //только получить значения
     //    
     //      var ltrans = _parent.GetLastReceivedTransform();
     //
     //      if (ltrans != null)
     //      {
     //         
     //          transform.SetTransformFromGameObjectBox(ltrans);
     //      }
     //
     //      return;
     //  }
        //только отправить значения
        float _h = Input.GetAxis("Horizontal");
        float _v = Input.GetAxis("Vertical");

        if (_h != 0 || _v != 0)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 10);
            transform.position += new Vector3(_h, 0, _v) * Time.deltaTime;

        }

      // if (_parent._LastBox.CheckChangeGameObjectBox(transform, .01f, 0.01f, 0.01f))
      // {
      //     // print("moved");
      //     //_client.SetGameObjectDirect(_localId, transform.SetGameObjectFromTrasform());
      //     _updater.AddUpdate(transform.SetGameObjectFromTrasform());
      // }

        //  print(_client._LastBox.GetPostionFromBox() + " - " + transform.position);

    }






}
