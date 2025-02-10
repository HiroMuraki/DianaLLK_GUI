using DianaLLK_GUI.ViewModel;
using LianLianKan;
using System.Windows;
using System.Windows.Controls;

namespace DianaLLK_GUI.View
{
    /// <summary>
    /// GameSetterControlPanel.xaml 的交互逻辑
    /// </summary>
    public partial class GameSetterControlPanel : UserControl
    {
        private GameSetter _gameSetter;

        public static readonly RoutedEvent StartEvent =
            EventManager.RegisterRoutedEvent(nameof(Start), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameSetterControlPanel));
        public static readonly DependencyProperty GameThemeProperty =
            DependencyProperty.Register(nameof(GameTheme), typeof(TokenCategory), typeof(GameSetterControlPanel), new PropertyMetadata(TokenCategory.None));

        public event RoutedEventHandler Start
        {
            add
            {
                AddHandler(StartEvent, value);
            }
            remove
            {
                RemoveHandler(StartEvent, value);
            }
        }

        public GameSetter GameSetter
        {
            get
            {
                return _gameSetter;
            }
        }
        public TokenCategory GameTheme
        {
            get
            {
                return (TokenCategory)GetValue(GameThemeProperty);
            }
            set
            {
                SetValue(GameThemeProperty, value);
            }
        }

        public GameSetterControlPanel()
        {
            _gameSetter = GameSetter.GetInstance();
            InitializeComponent();
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            RoutedEventArgs arg = new RoutedEventArgs(StartEvent, this);
            RaiseEvent(arg);
        }
    }
}
