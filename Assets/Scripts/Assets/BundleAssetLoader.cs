using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using Jedium.Utils;
using JediumCore;
using UnityEngine;


namespace Jedium.Assets
{
    public class BundleAssetLoader:IAssetLoader
    {
        private ILog _log = LogManager.GetLogger(typeof(BundleAssetLoader).ToString());

        public Dictionary<Guid,AssetBundle> _loadedBundles;

        //public string AssetsBaseUrl = "http://localhost:9080/api/assets/bundle/";

        private MD5 _md5;

        public BundleAssetLoader()
        {
            _loadedBundles=new Dictionary<Guid, AssetBundle>();
            _md5 = MD5.Create();
        }

        ~BundleAssetLoader()
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                foreach (var bundle in _loadedBundles)
                {
                    AssetBundle.UnloadAllAssetBundles(true);
                }
            });
           
        }

        public T GetFileAssetSync<T>(string name, string path) where T:UnityEngine.Object
        {
            AssetBundle bnd = AssetBundle.LoadFromFile(path);

            if (bnd == null)
            {
                _log.Warn($"Failed to load asset bundle from {path}, trying to get asset: {name}");

                return null;
            }

            T ret = bnd.LoadAsset<T>("name");

            return ret;
        }

        public T GetWebAssetSync<T>(string name, Guid  id) where T : UnityEngine.Object
        {

            _log.Info($"Loading asset syncronously:{name},{id}");

            string hash = GetBundleHash(id);
            hash = hash.Substring(1, hash.Length - 2);
            _log.Info("Bundle hash:" + hash);

            if (_loadedBundles.ContainsKey(id))
            {

                _log.Info("Found in loaded bundles");
                var cbnd = _loadedBundles[id];



                T cret = cbnd.LoadAsset<T>(name);

                return cret;
            }
            _log.Info("Loading bundle");

            //try to get from file cache
            byte[] fbytes = RootComponents.Instance.MainAssetCache.LoadAsset(hash);
            if (fbytes != null)
            {
                AssetBundle bnd = AssetBundle.LoadFromMemory(fbytes);
                if (bnd == null)
                {
                    _log.Warn($"Failed to load asset bundle with id {id}, trying to get asset: {name}");

                    return null;
                }
                _loadedBundles.Add(id, bnd);
                T ret = bnd.LoadAsset<T>(name);

                return ret;
            }
            else
            {
                byte[] bbytes =
                    GetURLContents(Test.Instance.MainSettings.WebApiUrl + "assets/bundle/" + id.ToString());
                // Debug.Log("__DOWNLOADED"+bbytes.Length);
                _log.Info("Bundle obtained");

                RootComponents.Instance.MainAssetCache.SaveAsset(hash, Guid.NewGuid().ToString(), bbytes);

                AssetBundle bnd = AssetBundle.LoadFromMemory(bbytes);
                if (bnd == null)
                {
                    _log.Warn($"Failed to load asset bundle with id {id}, trying to get asset: {name}");

                    return null;
                }

                _loadedBundles.Add(id, bnd);
                T ret = bnd.LoadAsset<T>(name);

                return ret;
            }

          }



        public async Task<T> GetWebAssetAsync<T>(string name, Guid id) where T : UnityEngine.Object
        {

            _log.Info($"Loading asset syncronously:{name},{id}");

            string hash = GetBundleHash(id);
            hash = hash.Substring(1, hash.Length - 2);
            _log.Info("Bundle hash:" + hash);

            if (_loadedBundles.ContainsKey(id))
            {

                _log.Info("Found in loaded bundles");
                var cbnd = _loadedBundles[id];



                T cret = await cbnd.LoadAssetAsync<T>(name) as T;

                return cret;
            }
            _log.Info("Loading bundle");

            //try to get from file cache
            byte[] fbytes = RootComponents.Instance.MainAssetCache.LoadAsset(hash);
            if (fbytes != null)
            {
                AssetBundle bnd = await AssetBundle.LoadFromMemoryAsync(fbytes);
                if (bnd == null)
                {
                    _log.Warn($"Failed to load asset bundle with id {id}, trying to get asset: {name}");

                    return null;
                }
                _loadedBundles.Add(id, bnd);
                T ret = bnd.LoadAsset<T>(name);

                return ret;
            }
            else
            {
                byte[] bbytes =
                  await  GetURLContentsAsync(Test.Instance.MainSettings.WebApiUrl + "assets/bundle/" + id.ToString());
                // Debug.Log("__DOWNLOADED"+bbytes.Length);
                _log.Info("Bundle obtained");

                RootComponents.Instance.MainAssetCache.SaveAsset(hash, Guid.NewGuid().ToString(), bbytes);

                AssetBundle bnd = await AssetBundle.LoadFromMemoryAsync(bbytes);
                if (bnd == null)
                {
                    _log.Warn($"Failed to load asset bundle with id {id}, trying to get asset: {name}");

                    return null;
                }

                _loadedBundles.Add(id, bnd);
                T ret = await bnd.LoadAssetAsync<T>(name) as T;

                return ret;
            }

        }

        public string GetBundleHash(Guid bundleId)
        {
            byte[] bbytes =
                GetURLContents(Test.Instance.MainSettings.WebApiUrl + "assets/bundlehash/" + bundleId.ToString());

            string ret = Encoding.Default.GetString(bbytes);
            return ret;
        }


        

        public string LoadSceneFromWebSync(Guid bundleId,string sceneName)
       {
            _log.Info($"Loading scene syncronously:{sceneName},{bundleId}");

           string hash = GetBundleHash(bundleId);
          

           hash = hash.Substring(1, hash.Length - 2);
           _log.Info("Bundle hash:" + hash);
            if (_loadedBundles.ContainsKey(bundleId))
           {

               _log.Info("Found in loaded bundles");
               var cbnd = _loadedBundles[bundleId];



               string[] scenePaths = cbnd.GetAllScenePaths();

              

               return scenePaths[0];
           }
           else
           {
                //try to get from file cache
               byte[] fbytes = RootComponents.Instance.MainAssetCache.LoadAsset(hash);

               if (fbytes != null)
               {


                   AssetBundle bnd = AssetBundle.LoadFromMemory(fbytes);
                   if (bnd == null)
                   {
                       _log.Warn($"Failed to load scene bundle with id {bundleId}, trying to get scene: {sceneName}");

                       return String.Empty;
                   }
                   _loadedBundles.Add(bundleId, bnd);

                   string[] scenePaths = bnd.GetAllScenePaths();

                    _log.Info($"Loaded scene {sceneName} from cache");

                   return scenePaths[0];
                }
               else
               {
                   _log.Info("Loading bundle");

                   byte[] bbytes =
                       GetURLContents(Test.Instance.MainSettings.WebApiUrl + "assets/bundle/" + bundleId.ToString());

                  // string dhash = _md5.ComputeHash(bbytes).ToHex(false);
                //   _log.Info($"Downloaded hash:" + dhash);
                   RootComponents.Instance.MainAssetCache.SaveAsset(hash, Guid.NewGuid().ToString(), bbytes);

                   AssetBundle bnd = AssetBundle.LoadFromMemory(bbytes);
                   if (bnd == null)
                   {
                       _log.Warn($"Failed to load scene bundle with id {bundleId}, trying to get scene: {sceneName}");

                       return String.Empty;
                   }
                   _loadedBundles.Add(bundleId, bnd);

                   string[] scenePaths = bnd.GetAllScenePaths();



                   return scenePaths[0];
                }
             
            }
        }


        public async Task<string> LoadSceneFromWebAsync(Guid bundleId, string sceneName)
        {
            _log.Info($"Loading scene asyncronously:{sceneName},{bundleId}");

            string hash = GetBundleHash(bundleId);


            hash = hash.Substring(1, hash.Length - 2);
            _log.Info("Bundle hash:" + hash);
            if (_loadedBundles.ContainsKey(bundleId))
            {

                _log.Info("Found in loaded bundles");
                var cbnd = _loadedBundles[bundleId];



                string[] scenePaths = cbnd.GetAllScenePaths();



                return scenePaths[0];
            }
            else
            {
                //try to get from file cache
                byte[] fbytes = RootComponents.Instance.MainAssetCache.LoadAsset(hash);

                if (fbytes != null)
                {


                    AssetBundle bnd = await AssetBundle.LoadFromMemoryAsync(fbytes);
                    if (bnd == null)
                    {
                        _log.Warn($"Failed to load scene bundle with id {bundleId}, trying to get scene: {sceneName}");

                        return String.Empty;
                    }
                    _loadedBundles.Add(bundleId, bnd);

                    string[] scenePaths = bnd.GetAllScenePaths();

                    _log.Info($"Loaded scene {sceneName} from cache");

                    return scenePaths[0];
                }
                else
                {
                    _log.Info("Loading bundle");

                    byte[] bbytes =
                      await  GetURLContentsAsync(Test.Instance.MainSettings.WebApiUrl + "assets/bundle/" + bundleId.ToString());

                    // string dhash = _md5.ComputeHash(bbytes).ToHex(false);
                    //   _log.Info($"Downloaded hash:" + dhash);
                    RootComponents.Instance.MainAssetCache.SaveAsset(hash, Guid.NewGuid().ToString(), bbytes);

                    AssetBundle bnd = await AssetBundle.LoadFromMemoryAsync(bbytes);
                    if (bnd == null)
                    {
                        _log.Warn($"Failed to load scene bundle with id {bundleId}, trying to get scene: {sceneName}");

                        return String.Empty;
                    }
                    _loadedBundles.Add(bundleId, bnd);

                    string[] scenePaths = bnd.GetAllScenePaths();



                    return scenePaths[0];
                }

            }
        }

        private byte[] GetURLContents(string url)
        {
            // The downloaded resource ends up in the variable named content.  
            var content = new MemoryStream();

            // Initialize an HttpWebRequest for the current URL.  
            var webReq = (HttpWebRequest)WebRequest.Create(url);

            // Send the request to the Internet resource and wait for  
            // the response.  
            // Note: you can't use HttpWebRequest.GetResponse in a Windows Store app.  
            using (WebResponse response = webReq.GetResponse())
            {
                // Get the data stream that is associated with the specified URL.  
                using (Stream responseStream = response.GetResponseStream())
                {
                    // Read the bytes in responseStream and copy them to content.    
                    responseStream.CopyTo(content);
                }
            }

            // Return the result as a byte array.  
            return content.ToArray();
        }

        private async Task<byte[]> GetURLContentsAsync(string url)
        {
           TaskCompletionSource<byte[]> tcs=new TaskCompletionSource<byte[]>();
            HttpWebRequest req = WebRequest.CreateHttp(url);

            using (HttpWebResponse resp = (HttpWebResponse) (await req.GetResponseAsync()))
            {
                using (Stream stream = resp.GetResponseStream())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        await stream.CopyToAsync(ms);
                        tcs.SetResult(ms.ToArray());
                        return await tcs.Task;
                    }
                }
            }

        }


        //TODO - move it
        public string GetServerHash()
        {
            byte[] bbytes =
                GetURLContents(Test.Instance.MainSettings.WebApiUrl+"FrameworkStats/ServerHash");

            string ret = Encoding.Default.GetString(bbytes);
           
            ret = ret.Substring(1, ret.Length - 2);
            _log.Info($"Obtained server hash: {ret}");
            return ret;
        }

        public string CalculateDomainHash()
        {

#if UNITY_EDITOR
            string path = Path.Combine(Application.dataPath,"Plugins","Domain.dll");
#else
           string path=Path.Combine(Application.dataPath,"Managed","Domain.dll");
#endif



            byte[] dbytes = File.ReadAllBytes(path);
            MD5 md5 = MD5.Create();
            string hash = md5.ComputeHash(dbytes).ToHex(false);
           

            return hash;
        }
    }

}

