using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Common.Logging;
using Domain;
using Domain.BehaviourMessages;
using UnityEngine;


namespace Jedium.Behaviours.Shared
{

    public static class BehaviourManager
    {
        private static ILog _log = LogManager.GetLogger("BehaviourManager");


        public static int LoadBehaviours(string path)
        {
            int ret = 0;
            _log.Info("-----------Start loading behaviour plugins---------------");
            _log.Info($"Path:{path}");

            List<Assembly> assemblies = new List<Assembly>();
            foreach (string dll in Directory.GetFiles(path, "*.dll"))
            {
                if (!dll.Contains("Jedium.Behaviours.Shared"))
                {

                    AssemblyName an = AssemblyName.GetAssemblyName(dll);
                    Assembly assembly = Assembly.Load(an);
                    assemblies.Add(assembly);
                    _log.Info($"Loaded assembly:{dll}");
                }
            }

            Type behaviourType = typeof(JediumBehaviour);

            //load behaviours first
            foreach (var assembly in assemblies)
            {
                try
                {
                    List<Type> types = assembly.GetLoadableTypes().ToList();



                    foreach (Type t in types)
                    {
                        if (t.IsAbstract || t.IsInterface)
                        {
                            continue;
                        }
                        else
                        {

                            if (t.BaseType == behaviourType)
                            {

                                // JediumBehaviour jb = (JediumBehaviour)Activator.CreateInstance(t, new object[] { null }); //not possible - behaviour
                                GameObject go = new GameObject();
                                var jb = (JediumBehaviour)go.AddComponent(t);
                                string btype = jb.GetComponentType();
                                GameObject.DestroyImmediate(go);
                                BehaviourTypeRegistry.RegisteredBehaviourTypes.Add(btype, t);

                                _log.Info($"Added behaviour:{btype},{t}");
                                ret++;
                            }


                        }
                    }
                }
                catch (ReflectionTypeLoadException e)
                {
                    var loaderExceptions = e.LoaderExceptions;
                    string estr = "";
                    for (int i = 0; i < loaderExceptions.Length; i++)
                        estr = estr + loaderExceptions[i].Message;
                    Debug.LogError(estr);
                    throw;
                }



            }

            //then snapshots
            Type snapshotType = typeof(JediumBehaviourSnapshot);

            foreach (var assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();

                foreach (Type t in types)
                {
                    if (t.IsAbstract || t.IsInterface)
                    {
                        continue;
                    }
                    else
                    {
                        if (t.BaseType == snapshotType)
                        {
                            JediumBehaviourSnapshot jb = (JediumBehaviourSnapshot)Activator.CreateInstance(t);
                            string btype = jb.GetBehaviourType();
                            BehaviourTypeRegistry.RegisteredSnapshotTypes.Add(btype, t);

                            _log.Info($"Added snapshot:{btype},{t}");

                        }


                    }
                }
            }

            _log.Info("-----------Finished loading behaviour plugins---------------");

            return ret;
        }

        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}

