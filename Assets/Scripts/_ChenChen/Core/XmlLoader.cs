using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ChenChen_Core
{
    public class XmlLoader : SingletonMono<XmlLoader>
    {
        private readonly string DEF_PATH = "Xml/Defs";
        private readonly string TEXTURE_PATH = "Xml/Texture";

        public static readonly string Def_Apparel = "Apparels";
        public static readonly string Def_PawnBody = "Bodies";
        public static readonly string Def_PawnHair = "Hairs";
        public static readonly string Def_PawnHead = "Heads";
        public static readonly string Def_Material = "Materials";
        public static readonly string Def_Food = "Foods";
        public static readonly string Def_Medicine = "Medicines";
        public static readonly string Def_Weapon = "Weapons";
        public static readonly string Def_Stuff = "Stuff";

        private Dictionary<string, object> defDict = new();
        private List<Def> defList = new();

        public List<T> Get<T>(string name) where T : Def
        {
            if(defDict.ContainsKey(name))
            {
                List<T> res = defDict[name] as List<T>;
                return res;
            }
            throw new System.Exception("不存在定义");
        }

        public T GetRandom<T>(string name) where T : Def
        {
            if (defDict.ContainsKey(name))
            {
                List<T> res = defDict[name] as List<T>;
                return res[Random.Range(0, res.Count)];
            }
            throw new System.Exception("不存在定义");
        }

        public Def GetDef(string label)
        {
            return defList.FirstOrDefault(x => x.label == label);
        }

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            LoadXmlFile<ApparelDef>(Def_Apparel);
            LoadXmlFile<BodyDef>(Def_PawnBody);
            LoadXmlFile<HairDef>(Def_PawnHair);
            LoadXmlFile<HeadDef>(Def_PawnHead);
            LoadXmlFile<MaterialDef>(Def_Material);
            LoadXmlFile<FoodDef>(Def_Food);
            LoadXmlFile<MedicineDef>(Def_Medicine);
            LoadXmlFile<WeaponDef>(Def_Weapon);
            LoadXmlFile<StuffDef>(Def_Stuff);
        }

        private void LoadXmlFile<T>(string fileName) where T : Def, new()
        {
            XmlDocument xmlDoc = new XmlDocument();
            string xmlFile = Path.Combine(Application.streamingAssetsPath, DEF_PATH, fileName + ".xml");

            if (!File.Exists(xmlFile))
            {
                Debug.LogError($"XML file not found at path: {xmlFile}");
                return;
            }

            xmlDoc.Load(xmlFile);
            var xmlNodeList = xmlDoc.GetElementsByTagName(typeof(T).Name);

            List<T> defs = new List<T>();

            foreach (XmlNode xmlNode in xmlNodeList)
            {
                T def = new T();
                foreach (XmlNode childNode in xmlNode.ChildNodes)
                {
                    string fieldName = childNode.Name != "iconPath" ? childNode.Name : "sprite";
                    FieldInfo property = def.GetType().GetField(fieldName);
                    if (property != null)
                    {
                        if (property.FieldType == typeof(int))
                        {
                            property.SetValue(def, int.Parse(childNode.InnerText));
                        }
                        else if (property.FieldType == typeof(float))
                        {
                            property.SetValue(def, float.Parse(childNode.InnerText));
                        }
                        else if (property.FieldType == typeof(string))
                        {
                            property.SetValue(def, childNode.InnerText);
                        }
                        else if (property.FieldType == typeof(bool))
                        {
                            property.SetValue(def, bool.Parse(childNode.InnerText));
                        }
                        else if (property.FieldType == typeof(Sprite))
                        {
                            property.SetValue(def, LoadSprite(childNode.InnerText));
                        }
                        else if (property.FieldType == typeof(Color))
                        {
                            if (ColorUtility.TryParseHtmlString(childNode.InnerText, out Color col))
                            {
                                property.SetValue(def, col);
                            }
                        }
                        else if (property.FieldType == typeof(Vector2Int))
                        {
                            property.SetValue(def, LoadSize(childNode.InnerText));
                        }
                        else if (property.FieldType == typeof(List<Need>))
                        {
                            List<Need> needs = new List<Need>();
                            foreach (XmlNode needNode in childNode.ChildNodes)
                            {
                                Need need = new Need();
                                foreach (XmlNode needChildNode in needNode.ChildNodes)
                                {
                                    var needProperty = need.GetType().GetField(needChildNode.Name);
                                    if (needProperty != null)
                                    {
                                        if (needProperty.FieldType == typeof(int))
                                        {
                                            needProperty.SetValue(need, int.Parse(needChildNode.InnerText));
                                        }
                                        else if (needProperty.FieldType == typeof(string))
                                        {
                                            needProperty.SetValue(need, needChildNode.InnerText);
                                        }
                                    }
                                }
                                needs.Add(need);
                            }
                            property.SetValue(def, needs);
                        }
                        else if (property.FieldType == typeof(List<string>))
                        {
                            List<string> content = new List<string>();
                            foreach (XmlNode itemNode in childNode.ChildNodes)
                            {
                                if (itemNode.Name == "li")
                                {
                                    content.Add(itemNode.InnerText);
                                }
                            }
                            property.SetValue(def, content);
                        }
                    }
                    else
                    {
                        Debug.Log("未找到：" + fieldName);  
                    }
                }
                defs.Add(def);
            }

            defDict.Add(fileName, defs);
            defList.AddRange(defs);
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

            texture.wrapMode = TextureWrapMode.Clamp;
            texture.filterMode = FilterMode.Point;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        private Vector2Int LoadSize(string size)
        {
            switch (size)
            {
                case "Long":
                    return new Vector2Int(1, 2);
                default:
                    return new Vector2Int(1, 1);
            }
        }
    }
}