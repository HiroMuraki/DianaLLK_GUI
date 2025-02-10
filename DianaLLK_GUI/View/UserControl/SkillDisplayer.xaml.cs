using LianLianKan;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace DianaLLK_GUI.View
{
    /// <summary>
    /// SkillDisplayer.xaml 的交互逻辑
    /// </summary>
    public partial class SkillDisplayer : UserControl
    {
        public SkillDisplayer()
        {
            InitializeComponent();
        }

        public async void DisplaySkill(LLKSkill skill, double displayTime, double displayWidth)
        {
            SkillIcon.Content = LLKHelper.GetSkillDescription(skill);
            switch (skill)
            {
                case LLKSkill.None:
                    SkillBar.Background = new SolidColorBrush(Colors.Black);
                    break;
                case LLKSkill.AvaPower:
                    SkillBar.Background = App.GetColor(LLKHelper.TokenCategoryThemes[TokenCategory.Ava]);
                    break;
                case LLKSkill.BellaPower:
                    SkillBar.Background = App.GetColor(LLKHelper.TokenCategoryThemes[TokenCategory.Bella]);
                    break;
                case LLKSkill.CarolPower:
                    SkillBar.Background = App.GetColor(LLKHelper.TokenCategoryThemes[TokenCategory.Carol]);
                    break;
                case LLKSkill.DianaPower:
                    SkillBar.Background = App.GetColor(LLKHelper.TokenCategoryThemes[TokenCategory.Diana]);
                    break;
                case LLKSkill.EileenPower:
                    SkillBar.Background = App.GetColor(LLKHelper.TokenCategoryThemes[TokenCategory.Eileen]);
                    break;
            }

            SkillIcon.Opacity = 1;
            HorizontalAlignment = HorizontalAlignment.Left;
            var animation = new DoubleAnimation()
            {
                To = displayWidth,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            BeginAnimation(WidthProperty, animation);

            await Task.Delay(TimeSpan.FromMilliseconds(displayTime));

            HorizontalAlignment = HorizontalAlignment.Right;
            var animation2 = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            BeginAnimation(WidthProperty, animation2);
            SkillIcon.Opacity = 0;
        }
    }
}
