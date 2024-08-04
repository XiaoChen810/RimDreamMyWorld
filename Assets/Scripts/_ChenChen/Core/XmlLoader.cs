using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using ChenChen_Thing;

namespace ChenChen_Core
{
    public class XmlLoader : SingletonMono<XmlLoader>
    {
        private readonly string DEF_PATH = "Xml/Defs";
        private readonly string TEXTURE_PATH = "Xml/Texture";

        public static readonly string Def_Appeal = "Appeals";

        private Dictionary<string, object> defDict = new();

        public List<T> Get<T>(string name) where T : Def
        {
            if(defDict.ContainsKey(name))
            {
                List<T> res = defDict[name] as List<T>;
                return res;
            }
            throw new System.Exception("不存在定义");
        }

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            LoadXmlFile_Appeal();
        }

        private void LoadXmlFile_Appeal()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlFile = Path.Combine(Application.streamingAssetsPath, DEF_PATH, "Appeals.xml");

            xmlDoc.Load(xmlFile);

            var xmlNodeList = xmlDoc.GetElementsByTagName("AppealDef");

            List<AppealDef> appealDefs = new List<AppealDef>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                string name = xmlNode["name"].InnerText;
                string description = xmlNode["description"].InnerText;
                string iconPath = xmlNode["iconPath"].InnerText;
                int workload = int.Parse(xmlNode["cost"]["workload"].InnerText);
                int costFabric = int.Parse(xmlNode["cost"]["costFabric"].InnerText);

                AppealDef def = new AppealDef(name, description, LoadSprite(iconPath), workload, costFabric);

                appealDefs.Add(def);
            }

            defDict.Add(Def_Appeal, appealDefs);
        }

        private Sprite LoadSprite(string relativePath)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, TEXTURE_PATH, relativePath + ".png");

            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Texture not found at path: {fullPath}");
                return null;
            }

            byte[] fileData = File.ReadAllBytes(fullPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }
    }
}