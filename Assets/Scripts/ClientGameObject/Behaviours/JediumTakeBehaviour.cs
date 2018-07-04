using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Domain.BehaviourMessages;
using Domain;
using Jedium.Behaviours.Shared;
using JediumCore;
using UnityEngine;

namespace Jedium.Behaviours
{
    class JediumTakeBehaviour : JediumBehaviour
    {
        private ILog _log = LogManager.GetLogger(typeof(JediumTakeBehaviour).ToString());

        private Guid person = Guid.Empty;

        public Vector3 AttachPoint { get; private set; }
        public Quaternion AttachRotation { get; private set; }

        private void Update()
        {
           

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log("escape pressed");
                OnEscaped();
            }
        }

        public override void Init(JediumBehaviourSnapshot snapshot)
        {
            base.Init(snapshot);
            

            if (snapshot.BehaviourType != "Take")
                return;
            JediumTakeSnapshot t = (JediumTakeSnapshot)snapshot;

            AttachPoint = new Vector3(t.X, t.Y, t.Z);
            AttachRotation = new Quaternion(t.RotX, t.RotY, t.RotZ, t.RotW);

            Debug.Log("AttachP:" + AttachPoint);
            Debug.Log("AttachR: " + AttachRotation);

        }

        private void OnMouseDown()
        {
            OnTaken();
        }

        //Метод обработки при выполнении функции Take
        private void OnTaken()
        {
            Debug.Log("OnTaken entered");
            JediumTakeMessage takeMessage = new JediumTakeMessage(Test.Instance._clientId, _parent.LocalId, true);
            _updater.AddUpdate(takeMessage);
        }

        private void OnEscaped()
        {
            if (this.person == Test.Instance._clientId)
            {
                JediumTakeMessage takeMessage = new JediumTakeMessage(Test.Instance._clientId, _parent.LocalId, false);
                _updater.AddUpdate(takeMessage);
            }
        }

        public override string GetComponentType()
        {
            return "Take";
        }

        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {
            
            if(Initialized)
            {
                if (message == null)
                    return false;

                if (!(message is JediumTakeMessage))
                    return false;


                Debug.Log("Take from server returned:");

                JediumTakeMessage inputMessage = (JediumTakeMessage)message;

                Debug.Log("Local ID: " + inputMessage.LocalId);
                TakingExecuting(inputMessage.ClientId, inputMessage.IsTaken);

                return true;
            }
            else
            {
                return false;
            }
            
        }


        private void TakingExecuting(Guid id, bool isTaken)
        {
            if(isTaken)
            {

                this.person = id;


                ClientGameObject gobj;

                Debug.Log("Client Id: " + Test.Instance._clientId);
                Debug.Log("Input ID " + id);

                foreach (var item in Test.Instance._otherAvatars)
                {
                    Debug.Log(item.Key);
                    Debug.Log(item.Value.OwnerId);
                }

                if (id == Test.Instance._clientId)
                {
                    gobj = Test.Instance._myAvatar;
                }
                else
                {
                    gobj = Test.Instance._otherAvatars.Values.Where(x => x.OwnerId == id).First();
                }

                if (gobj != null)
                {
                    Transform rightAvatarHandTr = gobj.GetObj.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1/mixamorig:Spine2/mixamorig:RightShoulder/mixamorig:RightArm/mixamorig:RightForeArm/mixamorig:RightHand");
                    //rightAvatarHandTr.Find()
                    if (rightAvatarHandTr == null)
                    {
                        Debug.Log("Rigt hand transform was not found");
                    }
                    else
                    {
                        Vector3 gameObjPos = rightAvatarHandTr.TransformPoint(AttachPoint);                        
                        this.gameObject.transform.position = gameObjPos;
                        this.gameObject.transform.rotation = AttachRotation;
                        this.gameObject.transform.SetParent(rightAvatarHandTr);
                    }
                }
                else
                {
                    Debug.Log("There is not such object");
                }
                return;
            }

            this.transform.parent = null;
            
        }

        

    }
}
