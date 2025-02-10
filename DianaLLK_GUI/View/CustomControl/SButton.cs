using LianLianKan;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DianaLLK_GUI.View
{
    public class SButton : Button
    {
        public static readonly DependencyProperty SkillProperty =
            DependencyProperty.Register(nameof(Skill), typeof(LLKSkill), typeof(SButton), new PropertyMetadata(LLKSkill.None));

        public event EventHandler<SClickEventArgs> SClick;

        public LLKSkill Skill
        {
            get
            {
                return (LLKSkill)GetValue(SkillProperty);
            }
            set
            {
                SetValue(SkillProperty, value);
            }
        }

        static SButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SButton), new FrameworkPropertyMetadata(typeof(SButton)));
        }

        protected override void OnClick()
        {
            base.OnClick();
            SClick?.Invoke(this, new SClickEventArgs(Skill));
        }
    }
}
