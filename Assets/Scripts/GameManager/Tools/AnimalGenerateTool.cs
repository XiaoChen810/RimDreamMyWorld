using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_AI;
using AYellowpaper.SerializedCollections;

public class AnimalGenerateTool : MonoBehaviour 
{
    [SerializeField] private List<Animal> _animalsList = new List<Animal>();
    /// <summary>
    /// 游戏内全部的Animalt列表
    /// </summary>
    public List<Animal> AnimalList
    {
        get { return _animalsList; }
    }

    [Header("动物的预制件")]
    [SerializedDictionary("名字","预制件")]
    public SerializedDictionary<string, GameObject> AnimalPrefabs;

    private int index = 0;
    public Animal GenerateAnimal(string name, Vector2 positon)
    {
        if (!AnimalPrefabs.ContainsKey(name))
        {
            Debug.LogWarning("没有这个动物的预制件");
            return null;
        }
        GameObject newAnimalObj = Instantiate(AnimalPrefabs[name]);
        newAnimalObj.name = $"{name} {index++}";
        newAnimalObj.transform.position = positon;
        if (!transform.Find("Animals"))
        {
            GameObject parent = new GameObject("Animals");
            parent.transform.parent = transform;
        }
        newAnimalObj.transform.SetParent(transform.Find("Animals"), false);

        Animal newAnimal = newAnimalObj.GetComponent<Animal>();
        AnimalList.Add(newAnimal);
        return newAnimal;
    }
}
