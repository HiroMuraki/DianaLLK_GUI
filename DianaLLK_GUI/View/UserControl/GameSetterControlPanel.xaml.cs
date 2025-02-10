using LianLianKan;
using System.Windows;
using System.Windows.Controls;

namespace DianaLLK_GUI.View
{
    public partial class GameSetterControlPanel : UserControl
    {
        public static readonly RoutedEvent StartEvent =
            EventManager.RegisterRoutedEvent(nameof(Start), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameSetterControlPanel));
        public static readonly DependencyProperty GameThemeProperty =
            DependencyProperty.Register(nameof(GameTheme), typeof(TokenCategory), typeof(GameSetterControlPanel), new PropertyMetadata(TokenCategory.None));

        public event RoutedEventHandler Start
        {
            add => AddHandler(StartEvent, value);
            remove => RemoveHandler(StartEvent, value);
        }

        public TokenCategory GameTheme
        {
            get => (TokenCategory)GetValue(GameThemeProperty);
            set => SetValue(GameThemeProperty, value);
        }

        public GameSetterControlPanel()
        {
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            var arg = new RoutedEventArgs(StartEvent, this);
            RaiseEvent(arg);
        }
    }
}
