using UnityEngine;

namespace ChenChen_Thing
{
    public class Thing_Furniture : ThingBase
    {
        // ÎïÌåÑÕÉ«
        private Color drawColor = Color.white;
        public Color DrawColor => drawColor;

        public override void OnCompleteBuild()
        {
            base.OnCompleteBuild();
            SR.color = drawColor;
        }
    }
}