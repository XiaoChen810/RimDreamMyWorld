using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Furniture : ThingBase
    {
        // ������ɫ
        private Color drawColor = Color.white;
        public Color DrawColor => drawColor;

        public override void OnCompleteBuild()
        {
            base.OnCompleteBuild();
            SR.color = drawColor;
        }
    }
}