using System;
using System.Windows.Controls;

namespace DianaLLK_GUI.View
{
    /// <summary>
    /// SkillGroupControl.xaml 的交互逻辑
    /// </summary>
    public partial class SkillGroupControl : UserControl
    {
        public event EventHandler<SClickEventArgs> SkillActived;

        public SkillGroupControl()
        {
            InitializeComponent();
        }

        private void ActiveSkill_Click(object sender, SClickEventArgs e)
        {
            SkillActived?.Invoke(this, new SClickEventArgs(e.SKill));
        }
    }
}
