﻿using DianaLLK_GUI.ViewModel;
using LianLianKan;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DianaLLK_GUI {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private readonly LLKGame _game;
        private readonly GameSetter _gameSetter;
        private DateTime _startTime;
        // private Point _hitPos;
        // private Line _directionLine;

        public static readonly DependencyProperty GameThemeProperty =
            DependencyProperty.Register(nameof(GameTheme), typeof(GameTheme), typeof(MainWindow), new PropertyMetadata(GameTheme.None));
        public GameTheme GameTheme {
            get {
                return (GameTheme)GetValue(GameThemeProperty);
            }
            set {
                SetValue(GameThemeProperty, value);
            }
        }
        public LLKGame Game {
            get {
                return _game;
            }
        }
        public GameSetter GameSetter {
            get {
                return _gameSetter;
            }
        }

        public MainWindow() {
            // 初始化游戏设置器
            _gameSetter = GameSetter.GetInstance();
            // 初始化游戏
            _game = new LLKGame();
            _game.GameCompleted += Game_GameCompleted;
            _game.LayoutReseted += Game_LayoutReseted;
            _game.SkillActived += Game_SkillActived;
            _game.TokenMatched += Game_TokenMatched;
            InitializeComponent();
            GridRoot.MaxHeight = SystemParameters.WorkArea.Height;
            GridRoot.MaxWidth = SystemParameters.WorkArea.Width;
            GameTheme = GameSetter.GetRandomGameTheme();
            ExpandGameSetterPanel();
        }

        private async void SelectToken_Click(object sender, TClickEventArgs e) {
            await _game.SelectTokenAsync(e.Token);
        }
        private async void ActiveSkill_Click(object sender, SClickEventArgs e) {
            await _game.ActiveSkillAsync(e.SKill);
        }
        private void StartGame_Click(object sender, RoutedEventArgs e) {
            try {
                StartGame();
                GetGameTheme();
                FoldGameSetterPanel();
                FoldTokenStack();
                GameStatistics.Hide();
                _startTime = DateTime.Now;
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void ExpandGameSetter_Click(object sender, RoutedEventArgs e) {
            if (GameSetterPanel.Height != 0) {
                FoldGameSetterPanel();
            }
            else {
                ExpandGameSetterPanel();
            }
        }
        private void ExpandTokenStack_Click(object sender, RoutedEventArgs e) {
            if (TokenStack.Width == 0) {
                ExpandTokenStack(ActualWidth / 3);
            }
            else {
                FoldTokenStack();
            }
        }
        private void GameStatistics_Confirmed(object sender, RoutedEventArgs e) {
            ExpandGameSetterPanel();
        }
        private void Game_SkillActived(object sender, SkillActivedEventArgs e) {
            if (e.ActiveResult == false) {
                return;
            }
            SkillDisplayer.DisplaySkill(e.Skill, 750, ActualWidth);
        }
        private void Game_TokenMatched(object sender, TokenMatchedEventArgs e) {
            if (e.Sucess) {
                TokenStack.AddToStack(e.TokenType);
            }
        }
        private void Game_GameCompleted(object sender, GameCompletedEventArgs e) {
            // 弹出统计窗口
            var gameUsingTime = (DateTime.Now - _startTime).TotalMilliseconds / 1000.0;
            var totalScores = (int)(e.TotalScores / Math.Log(gameUsingTime));
            GameStatistics.Display(e, gameUsingTime, 0, totalScores, ActualWidth / 4);
            ExpandTokenStack(ActualWidth / 4 * 3);
            // 展开设置窗口
            //ExpandGameSetterPanel();
        }
        private void Game_LayoutReseted(object sender, LayoutResetedEventArgs e) {
            TokensLayout.Children.Clear();
            foreach (var token in _game.LLKTokenArray) {
                var tokenRound = new View.LLKTokenRound(token);
                tokenRound.TClick += SelectToken_Click;
                if (token.TokenType == LLKTokenType.None) {
                    tokenRound.Opacity = 0;
                }
                TokensLayout.Children.Add(tokenRound);
            }
        }

        private void Window_Minimum(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }
        private void Window_Maximum(object sender, RoutedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                WindowState = WindowState.Normal;
            }
            else {
                WindowState = WindowState.Maximized;
            }
        }
        private void Window_Close(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
        private void Window_Move(object sender, MouseButtonEventArgs e) {
            if (e.ClickCount >= 2) {
                Window_Maximum(null, null);
            }
            else {
                DragMove();
            }
        }


        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame() {
            _game.StartGame(_gameSetter.RowSize, _gameSetter.ColumnSize, _gameSetter.TokenAmount);
            TokensLayout.Children.Clear();
            foreach (var token in _game.LLKTokenArray) {
                var tokenRound = new View.LLKTokenRound(token);
                tokenRound.TClick += SelectToken_Click;
                TokensLayout.Children.Add(tokenRound);
            }
            TokenStack.ResetStack();
        }
        /// <summary>
        /// 展开游戏设置面板
        /// </summary>
        private void ExpandGameSetterPanel() {
            _gameSetter.OnCurrentAvatarChanged();
            DoubleAnimation heightAnimation = new DoubleAnimation() {
                To = (ActualHeight == 0 ? Height : ActualHeight) - 50,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            DoubleAnimation opacityAnimation = new DoubleAnimation() {
                To = 1,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            GameSetterPanel.BeginAnimation(Grid.HeightProperty, heightAnimation);
            GameSetterPanel.BeginAnimation(Grid.OpacityProperty, opacityAnimation);
        }
        /// <summary>
        /// 收起游戏设置面板
        /// </summary>
        private void FoldGameSetterPanel() {
            DoubleAnimation heightAnimation = new DoubleAnimation() {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            DoubleAnimation opacityAnimation = new DoubleAnimation() {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(200)
            };
            GameSetterPanel.BeginAnimation(HeightProperty, heightAnimation);
            GameSetterPanel.BeginAnimation(OpacityProperty, opacityAnimation);
        }
        /// <summary>
        /// 展开收藏区
        /// </summary>
        private void ExpandTokenStack(double width) {
            DoubleAnimation animation = new DoubleAnimation() {
                To = width,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            TokenStack.BeginAnimation(WidthProperty, animation);
        }
        /// <summary>
        /// 收起收藏去
        /// </summary>
        private void FoldTokenStack() {
            DoubleAnimation animation = new DoubleAnimation() {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(150)
            };
            TokenStack.BeginAnimation(WidthProperty, animation);
        }
        /// <summary>
        /// 设置游戏主题色
        /// </summary>
        private void GetGameTheme() {
            Dictionary<GameTheme, int> numTokens = new Dictionary<GameTheme, int>() {
                [GameTheme.None] = 0,
                [GameTheme.Ava] = 0,
                [GameTheme.Bella] = 0,
                [GameTheme.Carol] = 0,
                [GameTheme.Diana] = 0,
                [GameTheme.Eileen] = 0
            };
            foreach (var item in _game.LLKTokenArray) {
                switch (item.TokenType) {
                    case LLKTokenType.None:
                        break;
                    case LLKTokenType.AS:
                        break;
                    case LLKTokenType.A1:
                    case LLKTokenType.A2:
                    case LLKTokenType.A3:
                    case LLKTokenType.A4:
                    case LLKTokenType.A5:
                        numTokens[GameTheme.Ava]++;
                        break;
                    case LLKTokenType.B1:
                    case LLKTokenType.B2:
                    case LLKTokenType.B3:
                    case LLKTokenType.B4:
                    case LLKTokenType.B5:
                        numTokens[GameTheme.Bella]++;
                        break;
                    case LLKTokenType.C1:
                    case LLKTokenType.C2:
                    case LLKTokenType.C3:
                    case LLKTokenType.C4:
                    case LLKTokenType.C5:
                        numTokens[GameTheme.Carol]++;
                        break;
                    case LLKTokenType.D1:
                    case LLKTokenType.D2:
                    case LLKTokenType.D3:
                    case LLKTokenType.D4:
                    case LLKTokenType.D5:
                        numTokens[GameTheme.Diana]++;
                        break;
                    case LLKTokenType.E1:
                    case LLKTokenType.E2:
                    case LLKTokenType.E3:
                    case LLKTokenType.E4:
                    case LLKTokenType.E5:
                        numTokens[GameTheme.Eileen]++;
                        break;
                }
            }

            var targetTheme = GameTheme.None;
            foreach (var item in numTokens) {
                if (numTokens[targetTheme] < item.Value) {
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