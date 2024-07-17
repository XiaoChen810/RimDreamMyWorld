using ChenChen_AI;
using System;
using System.Collections.Generic;

namespace ChenChen_UI
{
    public class DetailViewPanel_Animal : DetailViewPanel
    {
        private Animal animal;
        public DetailViewPanel_Animal(Animal animal, Callback onEnter, Callback onExit) : base(onEnter, onExit)
        {
            this.animal = animal;
        }
    }
}
