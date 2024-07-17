using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChenChen_AI;
using AYellowpaper.SerializedCollections;
using ChenChen_Map;
using System.Linq;

public class AnimalGenerateTool : MonoBehaviour 
{
    [SerializeField] private List<Animal> _animalsList = new List<Animal>();
    /// <summary>
    /// ��Ϸ��ȫ����Animalt�б�
    /// </summary>
    public List<Animal> AnimalList
    {
        get { return _animalsList; }
    }

    [Header("����Ķ���")]
    [SerializedDictionary("����","Ԥ�Ƽ�")]
    public SerializedDictionary<string, AnimalDef> AnimalDefs;



    private int index = 0;
    public Animal GenerateAnimal(string name, Vector2 position)
    {
        if (!AnimalDefs.ContainsKey(name))
        {
            Debug.LogWarning("û����������Ԥ�Ƽ�");
            return null;
        }
        GameObject newAnimalObj = Instantiate(AnimalDefs[name].Prefab, position, Quaternion.identity);
        newAnimalObj.name = $"{name} {index++}";

        Animal newAnimal = newAnimalObj.GetComponent<Animal>();
        newAnimal.Init(AnimalDefs.First(x => x.Key == name).Value, new AnimalInfo());
        AnimalList.Add(newAnimal);
        return newAnimal;
    }

    public void CreateAllAnimalThree()
    {
        foreach (var def in AnimalDefs)
        {
            //ÿ�ֶ�������3ֻ
            int createNum = 0;
            //��ֹ����ѭ��
            int flag = 0;
            while (createNum < 3 && flag < 1000)
            {
                Vector2 random = new Vector2(UnityEngine.Random.Range(0, MapManager.Instance.CurMapWidth), UnityEngine.Random.Range(0, MapManager.Instance.CurMapHeight));
                if (def.Value.IsAquaticAnimals)
                {
                    if (MapManager.Instance.GetMapNodeHere(random).type == NodeType.water)
                    {
                        GenerateAnimal(def.Value.Name, random);
                        createNum++;
                    }
                    else
                    {
                        flag++;
                    }
                }
                else
                {
                    if (MapManager.Instance.GetMapNodeHere(random).type != NodeType.water)
                    {
                        GenerateAnimal(def.Value.Name, random);
                        createNum++;
                    }
                    else
                    {
                        flag++;
                    }
                }
            }
        }
    }
}
