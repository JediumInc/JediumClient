using Domain.BehaviourMessages;
using Jedium.Behaviours.Shared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Commands;
using ClientUI;
using System.IO.Compression;

namespace Jedium.Behaviours
{
    public class JediumUIBehaviour : JediumBehaviour
    {

        private string dllName;
        private string xamlName;
        private Guid bundleId;
        private string archiveName;

        private IModel model;
        private string xamlPath;

        private string defaultUIPath;

        public override void Init(JediumBehaviourSnapshot snapshot)
        {
            base.Init(snapshot);

            defaultUIPath = Path.Combine(Application.persistentDataPath, "UI");

            if (snapshot.BehaviourType != "UI")
                return;
            JediumUISnapshot t = (JediumUISnapshot)snapshot;

            this.dllName = t.dllName;
            this.bundleId = t.bundleId;
            this.xamlName = t.xamlName;
            this.archiveName = t.archiveName;

            xamlPath = String.Empty;

            //check if base path exists
            if (!Directory.Exists(defaultUIPath))
            {
                Debug.Log("___CREATING UI PATH:"+defaultUIPath);
                Directory.CreateDirectory(defaultUIPath);
            }


            // string localPath = Path.Combine(Application.dataPath, "Resources", xamlName);

            string localPath = Path.Combine(defaultUIPath, xamlName);
            
            Debug.Log("___PATH 1:"+localPath);
            if(LoadXamlFromBundle(localPath))
            {
                xamlPath = Path.ChangeExtension(localPath, ".xaml");
            }
            Debug.Log("___PATH 2:" + xamlPath);
            // string archivePath = Path.Combine(Application.dataPath, "Resources", archiveName);

            string archivePath = Path.Combine(defaultUIPath,archiveName);

            string changedArchiveName = Path.ChangeExtension(archivePath, ".zip");

            Debug.Log("___PATH 3:" + changedArchiveName);

            LoadArchive(Path.Combine(changedArchiveName));

            Debug.Log("___Init");
            BindDependencies();


        }


        public void LoadArchive(string archiveLoadPath)
        {
            TextAsset txt = RootComponents.Instance.AssetLoader.GetWebAssetSync<TextAsset>(archiveName, bundleId);

            if(txt!=null)
            {

                using (FileStream fileWr = File.Create(archiveLoadPath))
                {
                    fileWr.Write(txt.bytes, 0, txt.bytes.Length);
                }


                 
                using (ZipArchive archive = ZipFile.Open(archiveLoadPath, ZipArchiveMode.Read))
                {

                    var entries = archive.Entries;

                    foreach (var entry in entries)
                    {
                        Debug.Log(entry.Name);

                        // if(!File.Exists(Path.Combine(Application.dataPath, "Resources", entry.Name)))
                        if (!File.Exists(Path.Combine(defaultUIPath, entry.Name)))
                        {
                            // entry.ExtractToFile(Path.Combine(Application.dataPath, "Resources", entry.Name));
                            entry.ExtractToFile(Path.Combine(defaultUIPath,  entry.Name));
                        }
                            
                    }                    
                                          
                }    

                if(File.Exists(archiveLoadPath))
                {
                    File.Delete(archiveLoadPath);
                }
                

                
            }
            else
            {
                Debug.Log("Archive was loaded");
            }



        }



        public void Extractstring()
        {

        }

        public bool LoadXamlFromBundle(string localPath)
        {
            TextAsset txt = RootComponents.Instance.AssetLoader.GetWebAssetSync<TextAsset>(xamlName, bundleId);

            if (txt != null)
            {

                string path = Path.ChangeExtension(localPath, ".xaml");

                FileStream xamlFile = File.Create(path);
                String str = Encoding.Default.GetString(txt.bytes);
                Debug.Log(str);
                using (StreamWriter writer = new StreamWriter(xamlFile))
                {
                    writer.Write(str);
                }
                xamlFile.Close();
                return true;
            }
            else
            {
                Debug.Log("Failed to load .bytes file");
                return false;
            }
        }

        private bool k;

        private void Update()
        {
            //   if(Input.GetKeyDown(model.GetKeyCode()))
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                k = !k;
                ShowCustomUI(k);
            }
        }


        private CustomUI custom;

        private void ShowCustomUI(bool k)
        {
            if(k)
            {
                custom = new CustomUI(xamlPath);
                MainUI.Instance.ShowNewUI(custom);
                BindDependencies();
            }
            else
            {
                MainUI.Instance.RemoveUI(custom);
            }
            
            
        }

        private void BindDependencies()
        {
            ViewModel viewModel = new ViewModel(Camera.main.gameObject.GetComponent<NoesisView>());

            TextAsset txt = RootComponents.Instance.AssetLoader.GetWebAssetSync<TextAsset>(dllName, bundleId);


            System.Reflection.Assembly assembly = System.Reflection.Assembly.Load(txt.bytes);

            IEnumerable<Type> type = assembly.GetTypes().Where(m => m.IsClass && m.GetInterface("IModel") != null);

            var obj = Activator.CreateInstance(type.First());

            viewModel.Subscribe(obj);

            model = obj as IModel;

            Debug.Log("Mode: " + model);
        }
      

        public override string GetComponentType()
        {
            return "UI";
        }

        public override bool ProcessUpdate(JediumBehaviourMessage message)
        {
            if (Initialized)
            {
                if (message == null)
                    return false;

                if (!(message is JediumUIMessage))
                    return false;


                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
