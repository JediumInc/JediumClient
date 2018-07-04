using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Noesis;
using UnityEngine;
using System.IO;


public class CustomUI : UserControl
{

    public CustomUI UIInstance { get; private set; }

    public CustomUI()
    {
        UIInstance = this;
    }

    public CustomUI(string path) : this()
    {
        LoadXamlContentByPath(path);
    }

    public void LoadXamlContentByPath(string path)
    {
        Debug.Log("CustomUI xaml: " + File.Exists(path));
        Noesis.GUI.LoadComponent(this, path);
    }

}

