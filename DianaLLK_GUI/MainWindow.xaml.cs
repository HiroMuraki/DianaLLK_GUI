using DianaLLK_GUI.ViewModel;
using LianLianKan;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Linq;
using DianaLLK_GUI.View;
using System.Threading.Tasks;

namespace DianaLLK_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(ASLLKGame game)
        {
            // 初始化游戏设置器
            GameSetter = GameSetter.Instance;
            // 初始化游戏
            Game = game;
            Game.GameCompleted += Game_GameCompleted;
            Game.LayoutReseted += Game_LayoutReset;
            Game.SkillActived += Game_SkillActivated;
            Game.TokensLinked += Game_TokensLinked;
            _gameSound = GameSoundPlayer.GetInstance();
            InitializeComponent();
            GridRoot.MaxHeight = SystemParameters.WorkArea.Height;
            GridRoot.MaxWidth = SystemParameters.WorkArea.Width;
            GameTheme = GameSetter.GetRandomGameTheme();
            ExpandGameSetterPanel();
        }

        public static readonly DependencyProperty GameThemeProperty =
            DependencyProperty.Register(nameof(GameTheme), typeof(TokenCategory), typeof(MainWindow), new PropertyMetadata(TokenCategory.None));

        public TokenCategory GameTheme
        {
            get => (TokenCategory)GetValue(GameThemeProperty);
            set => SetValue(GameThemeProperty, value);
        }

        public ASLLKGame Game { get; }

        public GameSetter GameSetter { get; }

        private void SelectToken_Click(object sender, TClickEventArgs e)
        {
            Game.SelectToken(e.Token);
            // 播放点击效果音
            _gameSound.PlayClickFXSound();
        }

        private async void ActiveSkill_Click(object sender, SClickEventArgs e)
        {
            await Game.ActiveSkillAsync(e.SKill);
        }

        private void StartGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Game.StartGame(GameSetter.RowSize, GameSetter.ColumnSize, GameSetter.TokenAmount);
                GetGameTheme();
                FoldGameSetterPanel();
                FoldTokenStack();
                FoldGameStatistic();
                TokenStack.ResetStack();
                _startTime = DateTime.Now;
            }
            catch (Exception exp)
            {
                TipBar.DisplayTip(exp.Message, TimeSpan.FromMilliseconds(2000));
            }
        }

        private void ExpandGameSetter_Click(object sender, RoutedEventArgs e)
        {
            if (GameSetterPanel.Height != 0)
            {
                FoldGameSetterPanel();
            }
            else
            {
                ExpandGameSetterPanel();
            }
        }

        private void ExpandTokenStack_Click(object sender, RoutedEventArgs e)
        {
            if (TokenStack.Width == 0)
            {
                ExpandTokenStack(ActualWidth / 3);
            }
            else
            {
                FoldTokenStack();
            }
        }

        private void GameStatistics_Confirmed(object sender, RoutedEventArgs e)
        {
            ExpandGameSetterPanel();
        }

        private void Token_Matched(object sender, TokenMatchedEventArgs e)
        {
            TokenStack.AddToStack(e.TokenType);
            // 播放连接成功效果音
            _gameSound.PlayMatchedFXSound();
        }

        private void Game_SkillActivated(object sender, SkillActivatedEventArgs e)
        {
            if (e.ActiveResult == false)
            {
                return;
            }
            SkillDisplayer.DisplaySkill(e.Skill, 750, ActualWidth);
            // 播放技能效果音
            _gameSound.PlaySkillActivedSound(e.Skill);
        }

        private void Game_GameCompleted(object sender, GameCompletedEventArgs e)
        {
            // 弹出统计窗口
            double gameUsingTime = (DateTime.Now - _startTime).TotalMilliseconds / 1000.0;
            int totalScores = (int)(e.TotalScores / Math.Log(gameUsingTime));
            GameStatistics.UpdateStatistic(e, gameUsingTime, 0, totalScores);
            // 展开统计窗口
            ExpandGameStatistic(ActualWidth / 4);
            ExpandTokenStack(ActualWidth / 4 * 3);
            // 播放游戏结算音
            _gameSound.PlayGameCompletedSound();
        }

        private void Game_LayoutReset(object sender, LayoutResetEventArgs e)
        {
            TokensLayout.Children.Clear();
            TokensLayout.Width = Game.ColumnSize * 120;
            TokensLayout.Height = Game.RowSize * 120;
            foreach (LLKToken token in Game.LLKTokenArray)
            {
                var tokenRound = new LLKTokenRound(token);
                tokenRound.TClick += SelectToken_Click;
                tokenRound.Token.Matched += Token_Matched;
                if (token.TokenType == LLKTokenType.None)
                {
                    tokenRound.Opacity = 0;
                }
                Canvas.SetLeft(tokenRound, token.Coordinate.X * 120);
                Canvas.SetTop(tokenRound, token.Coordinate.Y * 120);
                TokensLayout.Children.Add(tokenRound);
            }
        }

        private async void Game_TokensLinked(object sender, TokensLinkedEventArgs e)
        {
            var tokenPositions = new Point[e.Nodes.Length];
            for (int i = 0; i < e.Nodes.Length; i++)
            {
                Coordinate node = e.Nodes[i];
                LLKToken token = Game[node];
                LLKTokenRound tokenRound = TokensLayout.Children.OfType<LLKTokenRound>()
                    .First(x => x.Token.Coordinate == node);
                GeneralTransform transform = tokenRound.TransformToAncestor(TokensLayout);
                Point pos = transform.Transform(new Point(0, 0));
                pos.X += 60;
                pos.Y += 60;
                tokenPositions[i] = pos;
            }

            if (tokenPositions.Length >= 2)
            {
                var lineDrawer = new LineDrawer(TokensLayout);
                await lineDrawer.DrawAsync(tokenPositions);
            }
        }

        private void GameSave_Click(object sender, RoutedEventArgs e)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt";
            sfd.FileName = "LLKLayout.txt";
            sfd.Title = "保存游戏布局";
            if (sfd.ShowDialog() == true)
            {
                string fileName = sfd.FileName;
                try
                {
                    string outputString = LLKHelper.ConvertLayoutFrom(Game.TokenTypeArray, Game.RowSize, Game.ColumnSize, Game.CurrentTokenTypes.Count, Game.SkillPoint)
                        ?? throw new InvalidOperationException();

                    using (var writer = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                    {
                        writer.Write(Encoding.UTF8.GetBytes(outputString));
                    }

                    TipBar.DisplayTip($"存档已保存至{fileName}", TimeSpan.FromSeconds(3));
                }
                catch (Exception exp)
                {
                    TipBar.DisplayTip(exp.Message, TimeSpan.FromSeconds(3));
                }
            }
        }

        private async void GameSave_FileDragged(object sender, DragEventArgs e)
        {
            try
            {
                string[] fileList = e.Data.GetData(DataFormats.FileDrop) as string[];
                using (var file = new StreamReader(fileList[0]))
                {
                    string layoutString = file.ReadToEnd();
                    GameRestorePack result = LLKHelper.GenerateLayoutFrom(layoutString);
                    await Game.RestoreGameAsync(result);
                }
                TokenStack.ResetStack();
                FoldGameSetterPanel();
                FoldTokenStack();
                FoldGameStatistic();
                TipBar.DisplayTip("已加载存档", TimeSpan.FromSeconds(1));
                _startTime = DateTime.Now;
            }
            catch (Exception)
            {
                TipBar.DisplayTip("! 存档文件读取错误", TimeSpan.FromSeconds(2));
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 播放背景音乐
            _gameSound.PlayMusic();
        }

        private void Window_Minimum(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void Window_Maximum(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        private void Window_Close(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Move(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
            {
                Window_Maximum(null, null);
            }
            else
            {
                if (Mouse.LeftButton == MouseButtonState.Pressed)
                {
                    DragMove();
                }
            }
        }

        private void Window_DragEnter(object sender, DragEventArgs e)
        {
            FileDropArea.IsHitTestVisible = true;
        }

        private void Window_DragLeave(object sender, DragEventArgs e)
        {
            FileDropArea.IsHitTestVisible = false;
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            FileDropArea.IsHitTestVisible = false;
        }

        /// <summary>
        /// 展开游戏设置面板
        /// </summary>
        private void ExpandGameSetterPanel()
        {
            GameSetter.OnCurrentAvatarChanged();
            var heightAnimation = new DoubleAnimation()
            {
                To = (ActualHeight == 0 ? Height : ActualHeight) - 50,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            var opacityAnimation = new DoubleAnimation()
            {
                To = 1,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            GameSetterPanel.BeginAnimation(Grid.HeightProperty, heightAnimation);
            GameSetterPanel.BeginAnimation(Grid.OpacityProperty, opacityAnimation);
        }

        /// <summary>
        /// 收起游戏设置面板
        /// </summary>
        private void FoldGameSetterPanel()
        {
            var heightAnimation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            var opacityAnimation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            GameSetterPanel.BeginAnimation(HeightProperty, heightAnimation);
            GameSetterPanel.BeginAnimation(OpacityProperty, opacityAnimation);
        }

        /// <summary>
        /// 展开收藏
        /// </summary>
        private void ExpandTokenStack(double width)
        {
            var animation = new DoubleAnimation()
            {
                To = width,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            TokenStack.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// 收起收藏
        /// </summary>
        private void FoldTokenStack()
        {
            var animation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            TokenStack.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// 展开游戏统计信息
        /// </summary>
        /// <param name="width">展开宽度</param>
        private void ExpandGameStatistic(double width)
        {
            var animation = new DoubleAnimation()
            {
                To = width,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            GameStatistics.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// 收起游戏统计信息
        /// </summary>
        private void FoldGameStatistic()
        {
            var animation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            GameStatistics.BeginAnimation(WidthProperty, animation);
        }

        /// <summary>
        /// 设置游戏主题色
        /// </summary>
        private void GetGameTheme()
        {
            var numTokens = new Dictionary<TokenCategory, int>()
            {
                [TokenCategory.None] = 0,
                [TokenCategory.AS] = 0,
                [TokenCategory.Ava] = 0,
                [TokenCategory.Bella] = 0,
                [TokenCategory.Carol] = 0,
                [TokenCategory.Diana] = 0,
                [TokenCategory.Eileen] = 0
            };
            foreach (LLKTokenType tokenType in Game.TokenTypeArray)
            {
                numTokens[LLKHelper.GetTokenCategoryFromTokenType(tokenType)] += 1;
            }

            TokenCategory targetTheme = TokenCategory.None;
            foreach (KeyValuePair<TokenCategory, int> item in numTokens)
            {
                if (numTokens[targetTheme] < item.Value)
                {
                    targetTheme = item.Key;
                }
            }

            GameTheme = targetTheme;
        }

        #region NonPublic
        private readonly GameSoundPlayer _gameSound;
        private DateTime _startTime;
        #endregion
    }

    sealed class LineDrawer
    {
        public LineDrawer(Canvas canvas)
        {
            _canvas = canvas;
        }

        public async Task DrawAsync(Point[] points)
        {
            var addedLines = new List<Line>();

            // 检查传入的参数是否有效
            if (_canvas == null || points == null || points.Length < 2)
            {
                return;
            }
            // 遍历点数组，依次连接相邻的点
            for (int i = 0; i < points.Length - 1; i++)
            {
                Line line = DrawLine(points[i], points[i + 1]);
                // 将线段添加到 Canvas 中
                _canvas.Children.Add(line);
                addedLines.Add(line);
            }

            await Task.Delay(200);

            foreach (Line line in addedLines)
            {
                var animation = new DoubleAnimation()
                {
                    To = 0,
                    EasingFunction = new SineEase(),
                    Duration = TimeSpan.FromMilliseconds(500),
                };

                line.BeginAnimation(UIElement.OpacityProperty, animation);
            }

            await Task.Delay(1000);

            foreach (Line line in addedLines)
            {
                _canvas.Children.Remove(line);
            }
        }

        #region NonPublic
        private readonly Canvas _canvas;
        #endregion
        private static double CalculateLength(Point startPos, Point endPos)
        {
            return Math.Sqrt(Math.Pow(endPos.X - startPos.X, 2) + Math.Pow(endPos.Y - startPos.Y, 2));
        }
        private static Line DrawLine(Point startPos, Point endPos)
        {
            var line = new Line();
            // 绘制线条虚线信息
            double lineLength = CalculateLength(startPos, endPos);
            double dashLength = lineLength / 50 <= 5 ? lineLength / 50 : 5;
            double dashInterval = lineLength / 50 <= 3 ? lineLength / 50 : 3;
            line.StrokeDashArray = new DoubleCollection(new double[2] { dashLength, dashInterval });
            // 绘制线条颜色
            line.Stroke = new SolidColorBrush(Colors.White);
            // 绘制线条位置
            line.X1 = startPos.X;
            line.Y1 = startPos.Y;
            line.X2 = endPos.X;
            line.Y2 = endPos.Y;
            line.StrokeThickness = 5;
            return line;
        }
    }
}