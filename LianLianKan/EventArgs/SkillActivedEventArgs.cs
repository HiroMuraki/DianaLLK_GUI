using System;

namespace LianLianKan
{
    public class SkillActivedEventArgs : EventArgs
    {
        public SkillActivedEventArgs(LLKSkill skill, bool activeResult)
        {
            Skill = skill;
            ActiveResult = activeResult;
        }

        public LLKSkill Skill { get; }

        public bool ActiveResult { get; }
    }
}
