using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Domain;
using Domain.BehaviourMessages;
using Jedium.Behaviours.Shared;
using JediumCore;
using Newtonsoft.Json.Bson;
using UnityEngine;
using UnityEngine.Assertions.Must;

namespace Jedium.Behaviours
{

   
    [RequireComponent(typeof(Animator))]
   public class JediumAnimatorBehaviour : JediumBehaviour
    {

        public bool DoAnimStateTracking = false;
        private ILog _log = LogManager.GetLogger(typeof(JediumAnimatorBehaviour).ToString());
        private GameObject obj;
        private Animator _animator;

        

      


        public delegate void AnimatorObserver(JEDIUM_TYPE_ANIMATOR type, string name, object value);
        public event AnimatorObserver AnimatorChanges;

        private Dictionary<string, animatorParamsChange>
            _currrentAnimatorParams = new Dictionary<string, animatorParamsChange>();

        private Dictionary<string, animatorParamsChange>
            _lastAnimatorParams = new Dictionary<string, animatorParamsChange>();

         public bool HasRootMotion
         {
             get { return _animator.applyRootMotion; }
         }



        //TODO
        //HACK
        //Disabling transform for non-root-motion animators
        public override void Init(JediumBehaviourSnapshot snapshot)
        {
            base.Init(snapshot);
            if (!HasRootMotion)
            {
                JediumTransformBehaviour trans = GetComponent<JediumTransformBehaviour>();
                trans.AnimatorBased = true;
            }

        }

        //        delegate 
        void SetReference()
        {
            _animator = GetComponent<Animator>();
            if (!_animator)
            {
                _log.Error("Component Animator is No Found - " + this.name);
                return;
            }

            AddChangeParams(_animator.parameters, _currrentAnimatorParams); //запись ткущих параметров
            AddChangeParams(_animator.parameters, _lastAnimatorParams); // запись предыдущих параметров
        }


        void AddChangeParams(AnimatorControllerParameter[] acp, Dictionary<string, animatorParamsChange> dic)
        {
            foreach (var parameter in acp)
            {
             
                dic.Add(parameter.name, new animatorParamsChange(parameter.type));
            }
        }



        void CheckAllParams()
        {
            GetCurrentValues();

            foreach (var param in _currrentAnimatorParams)
            {

               // Debug.Log("______CHECKING PARAM:" + param.Key + "," + param.Value.GetObjParametr().ToString() + ";" + _lastAnimatorParams[param.Key].ToString());
              //  Debug.Log("__TOTAL PARAMS:"+_currrentAnimatorParams.Count);
                //if (param.Value.GetObjParametr() != _lastAnimatorParams[param.Key].GetObjParametr()) //comparing objects here - wrong!
                if (param.Value!= _lastAnimatorParams[param.Key])
                {

                   // Debug.Log("___SENDING PARAM:" + param.Key);
                   // if (param.Value.Type == JEDIUM_TYPE_ANIMATOR.BOOL)
                   

                    //AnimatorChanges?.Invoke(param.Value.Type, param.Key, param.Value.GetObjParametr());
                    //send it
                    JediumAnimatorMessage par=new JediumAnimatorMessage(param.Value.Type,param.Key,param.Value.GetObjParametr(),false,0,0);

                    Debug.Log("______SENDING ANIMATOR MESSAGE:"+param.Key);
                    _updater.AddUpdate(par);
                }
               
            }
        }


        void GetCurrentValues()
        {
            foreach (var change in _currrentAnimatorParams)
            {

                switch (change.Value.Type)
                {
                    case JEDIUM_TYPE_ANIMATOR.BOOL:
                    {
                        //   Debug.Log("___SETTING BOOL:"+ change.Key+";"+ _animator.GetBool(Animator.StringToHash(change.Key)));
                        change.Value.SetValue(_animator.GetBool(Animator.StringToHash(change.Key))); //TODO: pzd
                    }
                        break;
                    case JEDIUM_TYPE_ANIMATOR.FLOAT:
                        change.Value.SetValue(_animator.GetFloat(Animator.StringToHash(change.Key)));
                        break;
                    case JEDIUM_TYPE_ANIMATOR.INT:
                        change.Value.SetValue(_animator.GetInteger(Animator.StringToHash(change.Key)));
                        break;
                    case JEDIUM_TYPE_ANIMATOR.TRIGGER:
                        change.Value.SetValue(_animator.GetBool(Animator.StringToHash(change.Key)));
                        break;
                }


            }
        }

        void SetAllParams()
        {
            foreach (var change in _lastAnimatorParams)
            {
               
                switch (change.Value.Type)
                {
                    case JEDIUM_TYPE_ANIMATOR.BOOL:
                    {
                     //   Debug.Log("___SETTING BOOL:"+ change.Key+";"+ _animator.GetBool(Animator.StringToHash(change.Key)));
                        change.Value.SetValue(_animator.GetBool(Animator.StringToHash(change.Key))); //TODO: pzd
                    }
                        break;
                    case JEDIUM_TYPE_ANIMATOR.FLOAT:
                        change.Value.SetValue(_animator.GetFloat(Animator.StringToHash(change.Key)));
                        break;
                    case JEDIUM_TYPE_ANIMATOR.INT:
                        change.Value.SetValue(_animator.GetInteger(Animator.StringToHash(change.Key)));
                        break;
                    case JEDIUM_TYPE_ANIMATOR.TRIGGER:
                        change.Value.SetValue(_animator.GetBool(Animator.StringToHash(change.Key)));
                        break;
                }

                
            }

          
        }

       public void SafeSetAnimatorParam(JediumAnimatorMessage value)
       {

          // Debug.Log("___GOT PARAM:"+value.NameParameter+";"+value.Value);
           if (_lastAnimatorParams.ContainsKey(value.NameParameter))
           {
               if (_currrentAnimatorParams[value.NameParameter].Type == value.Type)
               {
                  

                   if (value.Type == JEDIUM_TYPE_ANIMATOR.BOOL || value.Type == JEDIUM_TYPE_ANIMATOR.TRIGGER)
                   {
                      // AnimatorControllerParameterType par = AnimatorControllerParameterType.Bool;
                     //   animatorParamsChange change = new animatorParamsChange(par);
                     //  change.b = (bool) value._value;
                     _animator.SetBool(value.NameParameter, (bool)value.Value);

                       _lastAnimatorParams[value.NameParameter].b = (bool) value.Value;
                       _currrentAnimatorParams[value.NameParameter].b = (bool) value.Value;


                   }

                   if (value.Type == JEDIUM_TYPE_ANIMATOR.FLOAT)
                   {
                    // Debug.Log("__SETTING FLOAT:"+value.NameParameter+";"+(float)value.Value);
                       _animator.SetFloat(value.NameParameter, (float)value.Value,value.DampTime,value.DeltaTime);
                      // _animator.SetFloat("V",1);

                       _lastAnimatorParams[value.NameParameter].f = (float)value.Value;
                       _currrentAnimatorParams[value.NameParameter].f = (float)value.Value;

                    }

                   if (value.Type == JEDIUM_TYPE_ANIMATOR.INT)
                   {
                       _animator.SetInteger(value.NameParameter, (int)value.Value);

                       _lastAnimatorParams[value.NameParameter].i = (int)value.Value;
                       _currrentAnimatorParams[value.NameParameter].i = (int)value.Value;
                    }
               }
               else
               {
                   _log.Info($"Wrong animator param type:{value.NameParameter},{value.Type}");
               }
           }
           else
           {
               _log.Info($"Wrong animator param name:{value.NameParameter}");
           }
       }

        void Awake()
        {
            SetReference();
        }

       


        public override string GetComponentType()
        {
            return "Animation";
        }

       

        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {
           
                if (message == null && DoAnimStateTracking) //empty message update
                {
                    CheckAllParams();
                    return false;
                }

                if (!(message is JediumAnimatorMessage))
                    return false;

                JediumAnimatorMessage amsg = (JediumAnimatorMessage) message;
                SafeSetAnimatorParam(amsg);

                if (DoAnimStateTracking)
                    CheckAllParams();
                return true;
           
        }

       // public void 

        //не работает
        void LateUpdate()
        {
           // CheckAllParams();
//            AddChangeParams(_animator.parameters, _lastAnimatorParams); // запись предыдущих параметров
            if(DoAnimStateTracking)
            SetAllParams();
        }

        #region Direct setters

        public void SetFloat(string name, float value, float dampTime, float deltaTime)
        {
          //  Debug.Log("___DIRECT FLOAT:"+name+";"+value);

            float cval = _animator.GetFloat(name);
            if (Math.Abs(cval - value) > 0.05f||Math.Abs(value)<0.001f)
            {
                JediumAnimatorMessage dfmsg = new JediumAnimatorMessage(JEDIUM_TYPE_ANIMATOR.FLOAT,
                    name, value, true, dampTime, deltaTime);

                _updater.AddUpdate(dfmsg);
            }
        }

        public void SetBool(string name, bool value)
        {
            JediumAnimatorMessage dfmsg = new JediumAnimatorMessage(JEDIUM_TYPE_ANIMATOR.BOOL,
                name, value, true, 0, 0);
            _updater.AddUpdate(dfmsg);
        }

        public bool GetBool(int id)
        {
            return _animator.GetBool(id);
        }

        public bool GetBool(string name)
        {
            return _animator.GetBool(name);
        }

        public float GetFloat(int id)
        {
            return _animator.GetFloat(id);
        }

        public float GetFloat(string name)
        {
            return _animator.GetFloat(name);
        }


        #endregion
    }


    public class animatorParamsChange
    {

        public const float FLOAT_DELTA = 0.05f;
        public animatorParamsChange(AnimatorControllerParameterType parameter)
        {
            switch (parameter)
            {
                case AnimatorControllerParameterType.Float:
                    Type = JEDIUM_TYPE_ANIMATOR.FLOAT;
                    break;
                case AnimatorControllerParameterType.Int:
                    Type = JEDIUM_TYPE_ANIMATOR.INT;
                    break;
                case AnimatorControllerParameterType.Bool:
                    Type = JEDIUM_TYPE_ANIMATOR.BOOL;
                    break;
                case AnimatorControllerParameterType.Trigger:
                    Type = JEDIUM_TYPE_ANIMATOR.TRIGGER;
                    break;
            }
        }

        public object GetObjParametr()
        {
            switch (Type)
            {
                case JEDIUM_TYPE_ANIMATOR.BOOL:
                    return b;
                    break;
                case JEDIUM_TYPE_ANIMATOR.FLOAT:
                    return f;
                    break;
                case JEDIUM_TYPE_ANIMATOR.INT:
                    return i;
                    break;
                case JEDIUM_TYPE_ANIMATOR.TRIGGER:
                    // return t;
                    return b;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetValue(object value)
        {
            switch (Type)
            {
                case JEDIUM_TYPE_ANIMATOR.BOOL:
                    b = (bool)value;
                    break;
                case JEDIUM_TYPE_ANIMATOR.FLOAT:
                    f = (float)value;
                    break;
                case JEDIUM_TYPE_ANIMATOR.INT:
                    i = (int)value;
                    break;
                case JEDIUM_TYPE_ANIMATOR.TRIGGER:
                    //  t = (string) value;
                    b = (bool)value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public JEDIUM_TYPE_ANIMATOR Type;
        public float f;
        public bool b;
        public int i;
        public string t;

        public override string ToString()
        {
            return $"Animator param: {Type},f:{f},b:{b},i:{i},t:{t}";
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(animatorParamsChange))
                return false;
            animatorParamsChange canim = (animatorParamsChange)obj;

            if (Type != canim.Type)
                return false;

            switch (Type)
            {
                case JEDIUM_TYPE_ANIMATOR.BOOL:
                    return b == canim.b;
                    break;

                case JEDIUM_TYPE_ANIMATOR.FLOAT:
                    return Math.Abs(f - canim.f) < FLOAT_DELTA;

                case JEDIUM_TYPE_ANIMATOR.INT:
                    return i == canim.i;

                case JEDIUM_TYPE_ANIMATOR.TRIGGER:
                    return b == canim.b;

                default:
                    return false;
            }

        }

        public static bool operator !=(animatorParamsChange a1, animatorParamsChange a2)
        {
            return !a1.Equals(a2);
        }

        public static bool operator ==(animatorParamsChange a1, animatorParamsChange a2)
        {
            return a1.Equals(a2);
        }
    }
}