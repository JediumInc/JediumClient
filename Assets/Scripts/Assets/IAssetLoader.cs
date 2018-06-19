using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


namespace Jedium.Assets
{
    public interface IAssetLoader
    {

        T GetFileAssetSync<T>(string name, string path) where T : UnityEngine.Object;

        T GetWebAssetSync<T>(string name, Guid bundle) where T : UnityEngine.Object;
    }

}

