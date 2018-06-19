using System.Collections;
using System.Collections.Generic;
using System.IO;
using Jedium.Assets;
using JediumCore;
using UnityEngine;


//TODO - FOR TESTING ONLY
public class RootComponents : MonoBehaviour
{
    public static RootComponents Instance;


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
        if (File.Exists(Path.Combine(Application.persistentDataPath, "settings.ini")))
        {
            SettingsLoader.LoadJediumSettings(Test.Instance.MainSettings, Path.Combine(Application.persistentDataPath, "settings.ini"));
        }
        else
        {
            //create settings
            SettingsLoader.SaveJediumSettings(Test.Instance.MainSettings, Path.Combine(Application.persistentDataPath, "settings.ini"));
        }
    }

    public BundleAssetLoader AssetLoader;
    public AssetCache MainAssetCache;

}
