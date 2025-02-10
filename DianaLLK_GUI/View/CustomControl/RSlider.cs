using System.Windows;
using System.Windows.Controls;

namespace DianaLLK_GUI.View
{
    public class RSlider : Slider
    {
        static RSlider()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RSlider), new FrameworkPropertyMetadata(typeof(RSlider)));
        }
    }
}
