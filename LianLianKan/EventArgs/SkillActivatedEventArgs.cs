using System;

namespace LianLianKan
{
    public class SkillActivatedEventArgs : EventArgs
    {
        public SkillActivatedEventArgs(LLKSkill skill, bool activeResult)
        {
            Skill = skill;
            ActiveResult = activeResult;
        }

        public LLKSkill Skill { get; }

        public bool ActiveResult { get; }
    }
}
