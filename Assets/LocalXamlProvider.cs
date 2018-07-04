using System;
using System.IO;
using Noesis;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Akka.Event;
using UnityEngine;

namespace NoesisApp
{
    public class LocalXamlProvider : XamlProvider
    {
        public static LocalXamlProvider instance=new LocalXamlProvider();
        public LocalXamlProvider() : this("")
        {
            _xamls = new Dictionary<string, Value>();
        }

        public LocalXamlProvider(string basePath)
        {
            _basePath = basePath;
        }

        //TODO - this is refs counter. Need to implement it properly
        public void Register(NoesisXaml xaml)
        {
            string uri = xaml.source;
            Value v;

            if (_xamls.TryGetValue(uri, out v))
            {
                v.refs++;
                v.xaml = xaml;
                _xamls[uri] = v;
            }
            else
            {
                _xamls[uri] = new Value() { refs = 1, xaml = xaml };
            }
        }

        public void Unregister(NoesisXaml xaml)
        {
            string uri = xaml.source;
            Value v;

            if (_xamls.TryGetValue(uri, out v))
            {
                if (v.refs == 1)
                {
                    _xamls.Remove(xaml.source);
                }
                else
                {
                    v.refs--;
                    _xamls[uri] = v;
                }
            }
        }


        public override Stream LoadXaml(string filename)
        {
            
            //string[] pathParts = filename.Split('/');
            //string pathPart = pathParts[pathParts.Length - 1];
            string path = System.IO.Path.Combine(_basePath, filename);

            //to memory
           UnityEngine.Debug.Log("__LOADING XAML FROM:"+filename);
            byte[] fbytes = File.ReadAllBytes(path);
            //TODO - in fact this is memleak
            return new MemoryStream(fbytes);
            // return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        private string _basePath;

        public struct Value
        {
            public int refs;
            public NoesisXaml xaml;
        }

        private Dictionary<string, Value> _xamls;
    }

    
}
