using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Domain.BehaviourMessages;
using Jedium.Behaviours.Shared;
using JediumCore;
using UnityEngine;

namespace Jedium.Behaviours
{
    class JediumSitBehaviour : JediumBehaviour
    {

        private ILog _log = LogManager.GetLogger(typeof(JediumSitBehaviour).ToString());

        private Guid person = Guid.Empty;

        public Vector3 AttachPoint { get; private set; }
        public Quaternion AttachRotation { get; private set; }


        public Vector3 LastPosition { get; set; }

        public override void Init(JediumBehaviourSnapshot snapshot)
        {
            base.Init(snapshot);

            Debug.Log("sgdf");

            JediumSitSnapshot sitSnap = (JediumSitSnapshot)snapshot;

            AttachPoint = new Vector3(sitSnap.X, sitSnap.Y, sitSnap.Z);
            AttachRotation = new Quaternion(sitSnap.RotX, sitSnap.RotY, sitSnap.RotZ, sitSnap.RotW);
        }



        public override string GetComponentType()
        {
            return "Sit";
        }

        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {            

            if (Initialized)
            {
                if (message == null)
                    return false;

                if (!(message is JediumSitMessage))
                    return false;


                Debug.Log("Take from server returned:");

                JediumSitMessage inputMessage = (JediumSitMessage)message;

                Debug.Log("Local ID: " + inputMessage.LocalId);

                SittingExecution(inputMessage.ClientId, inputMessage.IsOccupied);

                return true;
            }
            else
            {
                return false;
            }
        }


        private void OnMouseDown()
        {
            OnSitted();
        }


        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                OnStanded();
            }
        }

        private void OnSitted()
        {
            Debug.Log("OnSitted message started");
            JediumSitMessage sitMes = new JediumSitMessage(Test.Instance._clientId, _parent.LocalId, true);
            _updater.AddUpdate(sitMes);

        }

        private void OnStanded()
        {
            if(person == Test.Instance._clientId)
            {
                Debug.Log("OnStanded message started");
                JediumSitMessage sitMes = new JediumSitMessage(Test.Instance._clientId, _parent.LocalId, false);
                _updater.AddUpdate(sitMes);
            }

        }

        public Transform AvatarSit { get; set; }

        private void SittingExecution(Guid id, bool IsOccupied)
        {            

            if (IsOccupied)
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


                    AvatarSit = gobj.GetObj.transform;

                    if (AvatarSit == null)
                    {
                        Debug.Log("Sit transform was not found");
                    }
                    else
                    {


                        Vector3 gameObjPos = this.transform.TransformPoint(AttachPoint);

                        AvatarSit.GetComponent<Rigidbody>().isKinematic = true;

                        LastPosition = AvatarSit.position;

                        AvatarSit.position = gameObjPos;
                        AvatarSit.rotation = AttachRotation;
                        PlaySitAnimation(IsOccupied);
                      //  avatarSit.SetParent(this.transform);
                        
                    }
                }
                else
                {
                    Debug.Log("There is not such object");
                }
                return;
            }
            Debug.Log("Standed");


            AvatarSit.position = LastPosition;
            AvatarSit.GetComponent<Rigidbody>().isKinematic = false;
            PlaySitAnimation(IsOccupied);
            AvatarSit = null;

        }


        private void PlaySitAnimation(bool IsSitting)
        {

            if(IsSitting)
            {
                Debug.Log("SitTrigger");
                AvatarSit.GetComponent<Animator>().SetTrigger("SitTrigger");
            }
            else
            {
                Debug.Log("StandTrigger");
                AvatarSit.GetComponent<Animator>().SetTrigger("StandTrigger");
            }
            

            Debug.Log("Is sitting? " + IsSitting);
        }

    }
}
