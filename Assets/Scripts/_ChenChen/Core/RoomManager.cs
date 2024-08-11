using ChenChen_Map;
using ChenChen_UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChenChen_Core
{
    [System.Serializable]
    public class Room
    {
        public List<Vector2> area = new List<Vector2>();
        public int size => area.Count;
        public string type = "杂间";
        public Color color = new Color(1, 1, 1, 0.3f);

        public bool InRoom(Vector2 pos)
        {
            return area.Any(x => x == new Vector2((int)pos.x, (int)pos.y)); 
        }

        public GameObject ShowRoom()
        {
            GameObject parent = new GameObject(type);
            var dv = parent.AddComponent<DetailView_Room>();
            dv.Init(this);

            // 创建子物体方块
            foreach (Vector2 position in area)
            {
                GameObject block = new GameObject("Room");
                block.transform.position = position + new Vector2(0.5f, 0.5f);

                SpriteRenderer renderer = block.AddComponent<SpriteRenderer>();
                renderer.sprite = Resources.Load<Sprite>("Room");
                renderer.color = color;

                block.transform.parent = parent.transform;
            }

            // 计算边界并设置Collider2D
            Vector2 min = area[0];
            Vector2 max = area[0];

            foreach (Vector2 position in area)
            {
                min = Vector2.Min(min, position);
                max = Vector2.Max(max, position);
            }

            Vector2 size = max - min + Vector2.one; // +1保证包含所有方块
            Vector2 center = (min + max) / 2;

            // 在父对象上添加BoxCollider2D并设为触发器
            BoxCollider2D collider = parent.AddComponent<BoxCollider2D>();
            collider.size = size;
            collider.offset = center - (Vector2)parent.transform.position;
            collider.isTrigger = true;

            return parent;
        }
    }

    public class RoomManager : SingletonMono<RoomManager>
    {
        [SerializeField] private List<List<bool>> walls;
        [SerializeField] private List<Room> rooms = new List<Room>();
        [SerializeField] private List<GameObject> graphics = new List<GameObject>();
        [SerializeField] private bool open = false;
        protected override void Awake()
        {
            base.Awake();

            MapManager mapManager = MapManager.Instance;
            walls = new List<List<bool>>();
            for (int i = 0; i < mapManager.CurMapHeight; i++)
            {
                walls.Add(new List<bool>(new bool[mapManager.CurMapWidth]));
            }
        }

        #region - Public -
        public void AddWall(int x,int y)
        {
            walls[y][x] = true;
            JuggleRoom();
        }

        public void AddWall(Vector2 pos)
        {
            AddWall((int)pos.x, (int)pos.y);
        }

        public void RemoveWall(int x,int y)
        {
            walls[y][x] = false;
            JuggleRoom();
        }

        public void RemoveWall(Vector2 pos)
        {
            RemoveWall((int)pos.x, (int)pos.y);
        }

        public void ChangeRoomType(Vector2 pos, string type)
        {
            foreach (Room room in rooms)
            {
                if (room.InRoom(pos))
                {
                    room.type = type;
                    Debug.Log($"房间类型改变：{type}");
                }
            }
        }
        #endregion

        private void JuggleRoom()
        {
            rooms.Clear();

            int width = walls[0].Count;
            int height = walls.Count;
            bool[,] visited = new bool[height, width]; 

            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    if (!walls[y][x] && !visited[y, x])
                    {
                        Room room = new Room();
                        bool isClosed = ExploreRoom(x, y, room, visited);
                        if (isClosed)
                        {
                            rooms.Add(room);
                            Debug.Log($"形成了一个房间，大小为{room.size}");
                        }
                    }
                }
            }
        }

        private bool ExploreRoom(int x, int y, Room room, bool[,] visited)
        {
            int width = walls[0].Count;
            int height = walls.Count;
            Stack<Vector2> stack = new Stack<Vector2>();
            stack.Push(new Vector2(x, y));

            bool isClosed = true;

            while (stack.Count > 0)
            {
                Vector2 current = stack.Pop();
                int curX = (int)current.x;
                int curY = (int)current.y;

                if (curX < 0 || curX >= width || curY < 0 || curY >= height)
                {
                    isClosed = false; // 如果到达边界，表示这个区域不是封闭的
                    continue;
                }

                if (walls[curY][curX] || visited[curY, curX])
                    continue;

                visited[curY, curX] = true;
                room.area.Add(current);

                stack.Push(new Vector2(curX + 1, curY));
                stack.Push(new Vector2(curX - 1, curY));
                stack.Push(new Vector2(curX, curY + 1));
                stack.Push(new Vector2(curX, curY - 1));
            }

            return isClosed;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F3))
            {
                if(!open)
                {
                    open = true;
                    ShowRooms();
                }
                else
                {
                    open = false;
                    CloseRooms();
                }
                
            }
        }

        private void ShowRooms()
        {
            graphics.Clear();

            foreach (Room room in rooms)
            {
                graphics.Add(room.ShowRoom());
            }
        }

        private void CloseRooms()
        {
            foreach (var obj in graphics)
            {
                Destroy(obj);
            }

            graphics.Clear();
        }
    }
}
