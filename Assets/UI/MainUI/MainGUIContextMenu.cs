using System;
using System.Collections;
using System.Collections.Generic;
using Jedium.Behaviours.Shared;
using UnityEngine;

namespace ClientUI
{
    public class MainGUIContextMenu : MonoBehaviour
    {
        

        private List<Tuple<string, string>> _menuActions;
        void Awake()
        {
            //_behaviours=new List<JediumBehaviour>();

            JediumBehaviour[] behaviours = gameObject.GetComponents<JediumBehaviour>();

            _menuActions=new List<Tuple<string, string>>();
            for (int i = 0; i < behaviours.Length; i++)
            {
                if(!String.IsNullOrEmpty(behaviours[i].GetComponentAction().Item1))
                {
                    _menuActions.Add(behaviours[i].GetComponentAction());
                }
            }
        }

        void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            {
               MainUI.Instance.ShowContextMenu(_menuActions);
            }
        }
    }
}
