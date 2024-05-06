using System;

namespace ChenChen_Scene
{
    public abstract class SceneBase
    {
        public SceneType Type;
        public SceneBase(SceneType type)
        {
            Type = type;
        }

        public abstract void OnEnter();

        public abstract void OnExit();
    }
}
