using DianaLLK_GUI.ViewModel;
using LianLianKan;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DianaLLK_GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGame _game;
        private readonly GameSetter _gameSetter;
        private readonly GameSoundPlayer _gameSound;
        private DateTime _startTime;
        // private Point _hitPos;
        // private Line _directionLine;

        public static readonly DependencyProperty GameThemeProperty =
            DependencyProperty.Register(nameof(GameTheme), typeof(TokenCategory), typeof(MainWindow), new PropertyMetadata(TokenCategory.None));

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
        public IGame Game
        {
            get
            {
                return _game;
            }
        }
        public GameSetter GameSetter
        {
            get
            {
                return _gameSetter;
            }
        }

        public MainWindow(IGame game)
        {
            // 初始化游戏设置器
            _gameSetter = GameSetter.GetInstance();
            // 初始化游戏
            _game = game;
            _game.GameCompleted += Game_GameCompleted;
            _game.LayoutReseted += Game_LayoutReseted;
            _game.SkillActived += Game_SkillActived;
            _gameSound = GameSoundPlayer.GetInstance();
            InitializeComponent();
            GridRoot.MaxHeight = SystemParameters.WorkArea.Height;
            GridRoot.MaxWidth = SystemParameters.WorkArea.Width;
            GameTheme = GameSetter.GetRandomGameTheme();
            ExpandGameSetterPanel();
        }

        private async void SelectToken_Click(object sender, TClickEventArgs e)
        {
            await _game.SelectTokenAsync(e.Token);
            // 播放点击效果音
            _gameSound.PlayClickFXSound();
        }
        private async void ActiveSkill_Click(object sender, SClickEventArgs e)
        {
            await _game.ActiveSkillAsync(e.SKill);
        }
        private async void StartGame_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await _game.StartGameAsync(_gameSetter.RowSize, _gameSetter.ColumnSize, _gameSetter.TokenAmount);
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
            if (e.Sucess)
            {
                TokenStack.AddToStack(e.TokenType);
                // 播放连接成功效果音
                _gameSound.PlayMatchedFXSound();
            }
        }
        private void Game_SkillActived(object sender, SkillActivedEventArgs e)
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
            var gameUsingTime = (DateTime.Now - _startTime).TotalMilliseconds / 1000.0;
            var totalScores = (int)(e.TotalScores / Math.Log(gameUsingTime));
            GameStatistics.UpdateStatistic(e, gameUsingTime, 0, totalScores);
            // 展开统计窗口
            ExpandGameStatistic(ActualWidth / 4);
            ExpandTokenStack(ActualWidth / 4 * 3);
            // 播放游戏结算音
            _gameSound.PlayGameCompletedSound();
        }
        private void Game_LayoutReseted(object sender, LayoutResetedEventArgs e)
        {
            TokensLayout.Children.Clear();
            foreach (var token in _game.LLKTokenArray)
            {
                var tokenRound = new View.LLKTokenRound(token);
                tokenRound.TClick += SelectToken_Click;
                tokenRound.Token.Matched += Token_Matched;
                if (token.TokenType == LLKTokenType.None)
                {
                    tokenRound.Opacity = 0;
                }
                TokensLayout.Children.Add(tokenRound);
            }
        }
        private void GameSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "文本文件|*.txt";
            sfd.FileName = "LLKLayout.txt";
            sfd.Title = "保存游戏布局";
            if (sfd.ShowDialog() == true)
            {
                string fileName = sfd.FileName;
                try
                {
                    string outputString = LLKHelper.ConvertLayoutFrom(_game.TokenTypeArray, _game.RowSize, _game.ColumnSize, _game.CurrentTokenTypes.Count, _game.SkillPoint);
                    if (outputString == null)
                    {
                        throw new Exception();
                    }
                    using (FileStream writer = new FileStream(fileName, FileMode.Create, FileAccess.Write))
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
        private async void GameSave_FileDraged(object sender, DragEventArgs e)
        {
            try
            {
                string[] fileList = e.Data.GetData(DataFormats.FileDrop) as string[];
                using (StreamReader file = new StreamReader(fileList[0]))
                {
                    string layoutString = file.ReadToEnd();
                    GameRestorePack result = LLKHelper.GenerateLayoutFrom(layoutString);
                    await _game.RestoreGameAsync(result);
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
            _gameSetter.OnCurrentAvatarChanged();
            DoubleAnimation heightAnimation = new DoubleAnimation()
            {
                To = (ActualHeight == 0 ? Height : ActualHeight) - 50,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            DoubleAnimation opacityAnimation = new DoubleAnimation()
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
            DoubleAnimation heightAnimation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            DoubleAnimation opacityAnimation = new DoubleAnimation()
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
            DoubleAnimation animation = new DoubleAnimation()
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
            DoubleAnimation animation = new DoubleAnimation()
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
            DoubleAnimation animation = new DoubleAnimation()
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
            DoubleAnimation animation = new DoubleAnimation()
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
            Dictionary<TokenCategory, int> numTokens = new Dictionary<TokenCategory, int>()
            {
                [TokenCategory.None] = 0,
                [TokenCategory.AS] = 0,
                [TokenCategory.Ava] = 0,
                [TokenCategory.Bella] = 0,
                [TokenCategory.Carol] = 0,
                [TokenCategory.Diana] = 0,
                [TokenCategory.Eileen] = 0
            };
            foreach (var tokenType in _game.TokenTypeArray)
            {
                numTokens[LLKHelper.GetTokenCategoryFromTokenType(tokenType)] += 1;
            }

            var targetTheme = TokenCategory.None;
            foreach (var item in numTokens)
            {
                if (numTokens[targetTheme] < item.Value)
                {
                    targetTheme = item.Key;
                }
            }

            GameTheme = targetTheme;
        }
        ///// <summary>
        ///// 计算两点距离
        ///// </summary>
        ///// <param name="startPos"></param>
        ///// <param name="endPos"></param>
        ///// <returns></returns>
        //private static double CalculateLength(Point startPos, Point endPos) {
        //    return Math.Sqrt(Math.Pow(endPos.X - startPos.X, 2) + Math.Pow(endPos.Y - startPos.Y, 2));
        //}
        ///// <summary>
        ///// 绘制方向线
        ///// </summary>
        ///// <param name="startPos"></param>
        ///// <param name="endPos"></param>
        //private void DrawDirectionLine(Point startPos, Point endPos) {
        //    _directionLine.Visibility = Visibility.Visible;
        //    // 绘制线条虚线信息
        //    double lineLength = CalculateLength(startPos, endPos);
        //    double dashLength = lineLength / 50 <= 5 ? lineLength / 50 : 5;
        //    double dashInterval = lineLength / 50 <= 3 ? lineLength / 50 : 3;
        //    _directionLine.StrokeDashArray = new DoubleCollection(new double[2] { dashLength, dashInterval });
        //    // 绘制线条颜色
        //    _directionLine.Stroke = new SolidColorBrush(Colors.White);
        //    // 绘制线条位置
        //    _directionLine.X1 = startPos.X;
        //    _directionLine.Y1 = startPos.Y;
        //    _directionLine.X2 = endPos.X;
        //    _directionLine.Y2 = endPos.Y;
        //}
        ///// <summary>
        ///// 清除方向线
        ///// </summary>
        //private void ClearDirectionLine() {
        //    _directionLine.Visibility = Visibility.Hidden;
        //    _directionLine.X1 = 0;
        //    _directionLine.Y1 = 0;
        //    _directionLine.X2 = 0;
        //    _directionLine.Y2 = 0;
        //}
        //private void GameGesture_MouseUp(object sender, MouseButtonEventArgs e) {
        //    ClearDirectionLine();
        //}
        //private void GameGesture_MouseDown(object sender, MouseButtonEventArgs e) {
        //    _hitPos = e.GetPosition(GamePlayAreaCanvas);
        //}
        //private void GameGesture_MouseMove(object sender, MouseEventArgs e) {
        //    if (e.LeftButton != MouseButtonState.Pressed && e.RightButton != MouseButtonState.Pressed) {
        //        return;
        //    }
        //    Point currentPos = e.GetPosition(GamePlayAreaCanvas);
        //    // 绘制方向线
        //    DrawDirectionLine(_hitPos, currentPos);
        //}
    }
}