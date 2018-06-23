using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common.Logging;
using Jedium.Assets;
using JediumCore;
using Syrinj;
using UnityEngine;


//TODO - FOR TESTING ONLY
public class RootComponents : MonoBehaviour
{
    public static RootComponents Instance;

    private static ILog _log = LogManager.GetLogger("Root component");

  public string ClientHash;
    void Awake()
    {
        Instance = this;

        AssetLoader=new BundleAssetLoader();

        ClientHash = AssetLoader.CalculateDomainHash();

       
        MainAssetCache=new AssetCache();

       
    }

    void Start()
    {
        //read the settings
        if (!Directory.Exists(Path.Combine(Application.dataPath, @"..\Config")))
        {
            _log.Info($"Creating settings folder: {Path.Combine(Application.dataPath, @"..\Config")}");
        }


        if (File.Exists(Path.Combine(Application.dataPath, @"..\Config\settings.ini")))
        {
            _log.Info($"Loading settings from {Path.Combine(Application.dataPath, @"..\Config\settings.ini")}");
            SettingsLoader.LoadJediumSettings(Test.Instance.MainSettings, Path.Combine(Application.dataPath, @"..\Config\settings.ini"));
        }
        else
        {
            //create settings

            _log.Info($"Creating settings file: {Path.Combine(Application.dataPath, @"..\Config\settings.ini")}");
            SettingsLoader.SaveJediumSettings(Test.Instance.MainSettings, Path.Combine(Application.dataPath, @"..\Config\settings.ini"));
        }
    }

    public BundleAssetLoader AssetLoader;
    public AssetCache MainAssetCache;

}
