using System.Collections;
using System.Collections.Generic;
using Jedium.LocalBehaviours;
using JediumCore;
using UnityEngine;

   public abstract class JediumLocalBehaviour:MonoBehaviour
   {
    [SerializeField]
    protected bool Initialized = false;

    public bool RunUpdateUninitialized = false;

       protected ClientGameObject _jediumGameObject;

       public virtual void Init(ClientGameObject jgo)
       {
           _jediumGameObject = jgo;
           Initialized = true;
       }

    protected virtual void Awake()
    {
        //empty - moved to init
    }
    protected virtual void Start()
    {
        //empty - moved to init
    }

    protected virtual void Update()
    {
        if (RunUpdateUninitialized)
        {
            OnUpdate();
        }
        else if (Initialized)
        {
            OnUpdate();
        }
    }

    protected virtual void OnUpdate()
    {

   }
}
