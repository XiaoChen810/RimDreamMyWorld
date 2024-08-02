using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;

namespace ChenChen_Core
{
    public class XmlLoader : SingletonMono<XmlLoader>
    {
        private readonly string DEF_PATH = "Xml/Defs";
        private readonly string TEXTURE_PATH = "Xml/Texture";

        private void Start()
        {
            Load();
        }

        private void Load()
        {
            string xmlDirectory = Path.Combine(Application.dataPath, DEF_PATH);
            string[] xmlFiles = Directory.GetFiles(xmlDirectory, "*.xml");

            foreach (string xmlFile in xmlFiles)
            {

            }
        }

        private Sprite LoadSprite(string relativePath)
        {
            string fullPath = Path.Combine(Application.dataPath, TEXTURE_PATH, relativePath + ".png");

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

