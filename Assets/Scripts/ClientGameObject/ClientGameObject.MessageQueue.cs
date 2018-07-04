using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Akka.Interfaced;
using DelegateCommandImpl;
using Domain;
using Domain.BehaviourMessages;

using DotNetty.Common.Internal;
using UnityEngine;

namespace JediumCore
{
    public partial class ClientGameObject: InterfacedActor, IGameObject, IGameObjectObserver,IAbstractActor
    {

        const int MAX_PACK_SIZE=50;

        internal class TickMessage
        {

        }

        [MessageHandler]
        void HandleTick(TickMessage msg)
        {
        
         
          //sending packs
          foreach (var queue in _sendQueues)
          {
              if (!queue.Value.IsEmpty)
              {
                  List<JediumBehaviourMessage> s_msgs = new List<JediumBehaviourMessage>(50);
                  int i = 0;
                  JediumBehaviourMessage cmsg = null;
                  do
                  {
                      if (queue.Value.TryDequeue(out cmsg))
                      {
                          s_msgs.Add(cmsg);
                          i++;
                      }
                  } while (i < MAX_PACK_SIZE && !queue.Value.IsEmpty);
         
         
                  _serverConnection.WithNoReply().SendMessagePack(s_msgs.ToArray()); 
         
         
         
         
              }
          }
        }


        private Dictionary<int,ConcurrentQueue<JediumBehaviourMessage>> _sendQueues=new Dictionary<int, ConcurrentQueue<JediumBehaviourMessage>>();

   
        //TODO - use circular queue
        private ConcurrentQueue<JediumBehaviourMessage> _messageQueue=new ConcurrentQueue<JediumBehaviourMessage>();

        private Dictionary<int,ConcurrentQueue<JediumBehaviourMessage[]>> _messagePackQueues=new Dictionary<int, ConcurrentQueue<JediumBehaviourMessage[]>>();

        public JediumBehaviourMessage GetLastMessage()
        {
            if (_messageQueue.Count > 0)
            {
                JediumBehaviourMessage ret = null;
                if (_messageQueue.TryDequeue(out ret))
                    return ret;

            }

            return null;
        }



        public JediumBehaviourMessage[] GetLastMessagePack(int type)
        {

            if(_messagePackQueues.ContainsKey(type))
            if (_messagePackQueues[type].Count > 0)
            {
                JediumBehaviourMessage[] ret = null;
                if (_messagePackQueues[type].TryDequeue(out ret))
                    return ret;
            }

            return null;
        }

      


        public void AddSendMessage(JediumBehaviourMessage obj)
        {

            if (!_sendQueues.ContainsKey(obj.GetBehaviourType()))
            {
                //new queue
                ConcurrentQueue<JediumBehaviourMessage> c_q=new ConcurrentQueue<JediumBehaviourMessage>();
                c_q.Enqueue(obj);
                _sendQueues.Add(obj.GetBehaviourType(),c_q);

            }
            else
            {
                _sendQueues[obj.GetBehaviourType()].Enqueue(obj);
            }

            
            
           
        }

        public void AddReceivedMessagePack(JediumBehaviourMessage[] messages)
        {
            if (!_messagePackQueues.ContainsKey(messages[0].GetBehaviourType()))
            {
                ConcurrentQueue<JediumBehaviourMessage[]> cmsg=new ConcurrentQueue<JediumBehaviourMessage[]>();
                cmsg.Enqueue(messages);
                _messagePackQueues.Add(messages[0].GetBehaviourType(),cmsg);
            }
            else
            {
                _messagePackQueues[messages[0].GetBehaviourType()].Enqueue(messages);
            }
        }

        void PackMessages()
        {

        }

       public void SendTick()
       {
          
           //sending packs
          foreach (var queue in _sendQueues)
          {
              if (!queue.Value.IsEmpty)
              {
                  List<JediumBehaviourMessage> s_msgs=new List<JediumBehaviourMessage>(50);
                  int i = 0;
                  JediumBehaviourMessage cmsg = null;
                  do
                  {
                      if (queue.Value.TryDequeue(out cmsg))
                      {
                          s_msgs.Add(cmsg);
                          i++;
                      }
                  } while (i < MAX_PACK_SIZE && !queue.Value.IsEmpty);
         
                
                  UnityMainThreadDispatcher.Instance().Enqueue(()=> { _serverConnection.WithNoReply().SendMessagePack(s_msgs.ToArray()); });
                 
         
                 
         
              }
          }
           
           
       }

    }


}
