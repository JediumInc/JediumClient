using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class FileSystemWorker
{
    private static FileSystemWorker _instance;

    public static FileSystemWorker Instance
    {
        get
        {
            if(_instance==null)
                _instance=new FileSystemWorker();
            return _instance;
        }
    }

    public bool IsFolderExist(string param)
    {
        return Directory.Exists(param);
    }

    public void CreateFolder(string param)
    {
        Directory.CreateDirectory(param);
    }

    public void DeleteFolder(string param)
    {
        Directory.Delete(param, true);
    }

    public bool IsFileExist(string param)
    {
        return File.Exists(param);
    }
}
