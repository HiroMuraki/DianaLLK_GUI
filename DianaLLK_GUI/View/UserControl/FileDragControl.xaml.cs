using System.Windows;
using System.Windows.Controls;

namespace DianaLLK_GUI.View
{
    public partial class FileDragControl : UserControl
    {
        public event DragEventHandler FileDraged;

        public FileDragControl()
        {
            InitializeComponent();
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            FileDraged?.Invoke(this, e);
        }
    }
}
