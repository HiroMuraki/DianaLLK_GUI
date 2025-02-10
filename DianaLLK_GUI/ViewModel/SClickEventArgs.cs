// using MergeDiana.GameLib;
using LianLianKan;
using System;

namespace DianaLLK_GUI
{
    public class SClickEventArgs : EventArgs
    {
        public LLKSkill SKill { get; }

        public SClickEventArgs(LLKSkill skill)
        {
            SKill = skill;
        }
    }
}