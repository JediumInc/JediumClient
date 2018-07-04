using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Logging;
using UnityEngine;
using SQLite4Unity3d;

namespace Jedium.Assets
{
    public class AssetCache
    {
        private SQLiteConnection _fileDB;

        private ILog _log = LogManager.GetLogger(typeof(AssetCache).ToString());
        public AssetCache()
        {
            if (!Directory.Exists(Path.Combine(Application.persistentDataPath, "cache")))
            {
                Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "cache"));
            }

            _fileDB=new SQLiteConnection(Path.Combine(Application.persistentDataPath, "cache","cache.db"));


                _fileDB.CreateTable<FileAsset>();
           _fileDB.Close();

        }

        public void SaveAsset(string hash, string name, byte[] file)
        {
            _fileDB = new SQLiteConnection(Path.Combine(Application.persistentDataPath, "cache", "cache.db"));
            //save file
            string path = Path.Combine(Application.persistentDataPath, "cache", name);

            File.WriteAllBytes(path,file);

            _fileDB.Insert(new FileAsset()
            {
                Hash = hash,
                Path = path
            });
            _fileDB.Close();
            _log.Info($"Stored file {path}, hash {hash}");
        }

        public byte[] LoadAsset(string hash)
        {
            _fileDB = new SQLiteConnection(Path.Combine(Application.persistentDataPath, "cache", "cache.db"));
            FileAsset asset = _fileDB.Table<FileAsset>().FirstOrDefault(x => x.Hash == hash);

            if (asset == null)
                return null;

            if (!File.Exists(asset.Path))
                return null;

            _fileDB.Close();
            _log.Info($"Loading file: {asset.Path}");
            byte[] fbytes = File.ReadAllBytes(asset.Path);
            return fbytes;
        }

       
    }

    public class FileAsset
    {
        [PrimaryKey]
        public string Hash { get; set; }
        public string Path { get; set; }
    }
}
