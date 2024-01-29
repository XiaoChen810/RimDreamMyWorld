using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ��ͼ����;
using ����;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public SceneSystem SceneSystem {  get; private set; }

    public List<GameObject> CharactersList;

    public GameObject CharacterTest;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            Init();
        }
    }

    private void Init()
    {
        SceneSystem = new();
        CharactersList = new List<GameObject>();

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        SceneSystem.SetScene(new StartScene());
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            MapManager.Instance.CloseSceneMap("MainMap");
            SceneSystem.SetScene(new StartScene());
        }

        if(Input.GetKeyUp(KeyCode.L))
        {
            MapManager.Instance.LoadSceneMap("Test");
        }
    }

    public void ����һ������С��()
    {
        GameObject newCharacter = Instantiate(CharacterTest);
        CharactersList.Add(newCharacter);   
    }
}
