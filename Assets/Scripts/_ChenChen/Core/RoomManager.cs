using ChenChen_Map;
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
        public int size = 0;
        public string type = "空房间";

        public bool InRoom(Vector2 pos)
        {
            return area.Any(x => x == new Vector2((int)pos.x, (int)pos.y)); 
        }
    }

    public class RoomManager : SingletonMono<RoomManager>
    {
        [SerializeField] private List<List<bool>> walls;
        [SerializeField] private List<Room> rooms = new List<Room>();

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
                room.size++;

                stack.Push(new Vector2(curX + 1, curY));
                stack.Push(new Vector2(curX - 1, curY));
                stack.Push(new Vector2(curX, curY + 1));
                stack.Push(new Vector2(curX, curY - 1));
            }

            return isClosed;
        }
    }
}
