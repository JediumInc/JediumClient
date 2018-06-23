using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using IniParser;
using IniParser.Model;

[Serializable]
[CreateAssetMenu(fileName = "JediumSettings", menuName = "Jedium Settings", order = 1)]
public class JediumSettings : ScriptableObject
{
    public float UpdateRate = 0.01f;
    public bool UseFixedUpdate = false;
    public bool UseUpdateThread = false;
    public long UpdateThreadInterval = 2;
    public string WebApiUrl = "http://localhost:9080/api/";
    public string ServerUrl = "akka.tcp://VirtualFramework@localhost:18095/user/ServerEndpoint";

    public string InitialScene = "0b5fd70d-a95b-4e0f-b518-17b1e67e73d7";

    public void OnValidate()
    {
     //   SettingsLoader.SaveJediumSettings(this, Path.Combine(Application.persistentDataPath, "settings.ini"));
    }
}

public static class SettingsLoader
{
    public static void SaveJediumSettings(JediumSettings settings,string path)
    {
        Debug.Log($"Saving Settings: {path}");
        IniData data=new IniData();
        data.Sections.Add(new SectionData("Main"));
        data["Main"].AddKey(new KeyData("UpdateRate"));
        data["Main"]["UpdateRate"] = settings.UpdateRate.ToString();
        data["Main"].AddKey(new KeyData("UseFixedUpdate"));
        data["Main"]["UseFixedUpdate"] = settings.UseFixedUpdate.ToString();

        data["Main"].AddKey(new KeyData("UseUpdateThread"));
        data["Main"]["UseUpdateThread"] = settings.UseUpdateThread.ToString();

        data["Main"].AddKey(new KeyData("UpdateThreadInterval"));
        data["Main"]["UpdateThreadInterval"] = settings.UpdateThreadInterval.ToString();

        data["Main"].AddKey(new KeyData("WebApiUrl"));
        data["Main"]["WebApiUrl"] = settings.WebApiUrl.ToString();

        data["Main"].AddKey(new KeyData("ServerUrl"));
        data["Main"]["ServerUrl"] = settings.ServerUrl;

        data["Main"].AddKey(new KeyData("InitialScene"));
        data["Main"]["InitialScene"] = settings.InitialScene;


        var parser=new FileIniDataParser();
        parser.WriteFile(path,data);
    }

    public static void LoadJediumSettings(JediumSettings settings, string filename)
    {
        var parser = new FileIniDataParser();
        IniData data = parser.ReadFile(filename);
        settings.UpdateRate = float.Parse(data["Main"]["UpdateRate"]);
        settings.UseFixedUpdate=bool.Parse(data["Main"]["UseFixedUpdate"]);
        settings.UseUpdateThread = bool.Parse(data["Main"]["UseUpdateThread"]);
        settings.UpdateThreadInterval = long.Parse(data["Main"]["UpdateThreadInterval"]);
        settings.WebApiUrl = data["Main"]["WebApiUrl"];
        settings.ServerUrl = data["Main"]["ServerUrl"];
    }


}
