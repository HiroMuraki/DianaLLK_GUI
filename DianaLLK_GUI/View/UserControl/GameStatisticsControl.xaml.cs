using LianLianKan;
using System.Windows;
using System.Windows.Controls;

namespace DianaLLK_GUI.View
{
    public partial class GameStatisticsControl : UserControl
    {
        public static readonly RoutedEvent ConfirmedEvent =
            EventManager.RegisterRoutedEvent(nameof(Confirmed), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(GameStatisticsControl));
        public static readonly DependencyProperty GameUsingTimeProperty =
            DependencyProperty.Register(nameof(GameUsingTime), typeof(double), typeof(GameStatisticsControl), new PropertyMetadata(0.0));
        public static readonly DependencyProperty TokenAmountProperty =
            DependencyProperty.Register(nameof(TokenAmount), typeof(int), typeof(GameStatisticsControl), new PropertyMetadata(0));
        public static readonly DependencyProperty GameSizeProperty =
            DependencyProperty.Register(nameof(GameSize), typeof(string), typeof(GameStatisticsControl), new PropertyMetadata(""));
        public static readonly DependencyProperty SkillActivedTimesProperty =
            DependencyProperty.Register(nameof(SkillActivedTimes), typeof(int), typeof(GameStatisticsControl), new PropertyMetadata(0));
        public static readonly DependencyProperty TotalScoresProperty =
            DependencyProperty.Register(nameof(TotalScores), typeof(int), typeof(GameStatisticsControl), new PropertyMetadata(0));
        public static readonly DependencyProperty TokenTypeProperty =
            DependencyProperty.Register(nameof(TokenType), typeof(LLKTokenType), typeof(GameStatisticsControl), new PropertyMetadata(LLKTokenType.None));
        public static readonly DependencyProperty GameTypeProperty =
            DependencyProperty.Register(nameof(GameType), typeof(GameType), typeof(GameStatisticsControl), new PropertyMetadata(GameType.New));

        public event RoutedEventHandler Confirmed
        {
            add => AddHandler(ConfirmedEvent, value);
            remove => RemoveHandler(ConfirmedEvent, value);
        }

        public LLKTokenType TokenType
        {
            get => (LLKTokenType)GetValue(TokenTypeProperty);
            set => SetValue(TokenTypeProperty, value);
        }

        public double GameUsingTime
        {
            get => (double)GetValue(GameUsingTimeProperty);
            set => SetValue(GameUsingTimeProperty, value);
        }

        public int TokenAmount
        {
            get => (int)GetValue(TokenAmountProperty);
            set => SetValue(TokenAmountProperty, value);
        }

        public GameType GameType
        {
            get => (GameType)GetValue(GameTypeProperty);
            set => SetValue(GameTypeProperty, value);
        }

        public string GameSize
        {
            get => (string)GetValue(GameSizeProperty);
            set => SetValue(GameSizeProperty, value);
        }

        public int SkillActivedTimes
        {
            get => (int)GetValue(SkillActivedTimesProperty);
            set => SetValue(SkillActivedTimesProperty, value);
        }

        public int TotalScores
        {
            get => (int)GetValue(TotalScoresProperty);
            set => SetValue(TotalScoresProperty, value);
        }

        public GameStatisticsControl()
        {
            InitializeComponent();
        }

        public void UpdateStatistic(GameCompletedEventArgs e, double gameUsingTime, int skillActivedTimes, int totalScore)
        {
            TokenAmount = e.TokenAmount;
            GameUsingTime = gameUsingTime;
            SkillActivedTimes = skillActivedTimes;
            TotalScores = totalScore;
            GameSize = $"{e.RowSize} x {e.ColumnSize}";
            TokenType = ViewModel.GameSetter.GetRandomTokenType();
            GameType = e.GameType;
        }


        #region NonPublic
        private void ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            var arg = new RoutedEventArgs(ConfirmedEvent, this);
            RaiseEvent(arg);
        }
        #endregion
    }
}
