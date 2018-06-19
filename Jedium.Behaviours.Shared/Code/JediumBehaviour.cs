using System.Collections;
using System.Collections.Generic;

using Domain;
using Domain.BehaviourMessages;

using UnityEngine;


namespace Jedium.Behaviours.Shared
{



    public abstract class JediumBehaviour : MonoBehaviour
    {

        protected IClientGameObjectUpdater _updater;

        protected IClientGameObject _parent;

        private bool _initialized = false;
        public bool Initialized
        {
            get { return _initialized; }
        }

        public void SetParent(IClientGameObject parent)
        {
            _parent = parent;

        }

        //like Awake
        public virtual void Init()
        {
            _initialized = true;
        }

        public void SetUpdater(IClientGameObjectUpdater updater)
        {
            _updater = updater;
            _updater.RegisterComponent(this);
        }

        public abstract bool ProcessUpdate(JediumBehaviourMessage message);

        public abstract string GetComponentType();


        //TODO - optimize it!!
        public virtual int GetComponentTypeIndex()
        {
            return TYPEBEHAVIOUR.GetTypeIndex(GetComponentType());
        }
    }
}
