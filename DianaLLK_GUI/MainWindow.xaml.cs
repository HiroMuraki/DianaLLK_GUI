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
            // 初始化计时器
            //_gameTimer = new DispatcherTimer() {
            //    Interval = TimeSpan.FromMilliseconds(50)
            //};
            //_gameTimer.Tick += GameTimer_Tick;
            //// 初始化方向线
            //_directionLine = new Line {
            //    StrokeThickness = 5,
            //    StrokeEndLineCap = PenLineCap.Triangle,
            //    StrokeDashCap = PenLineCap.Round,
            //    StrokeStartLineCap = PenLineCap.Round,
            //    Visibility = Visibility.Hidden
            //};
            // 初始化游戏设置器
            _gameSetter = GameSetter.GetInstance();
            // 初始化游戏
            _game = new LLKGame();
            _game.GameCompleted += Game_GameCompleted;
            InitializeComponent();
            GridRoot.MaxHeight = SystemParameters.WorkArea.Height;
            GridRoot.MaxWidth = SystemParameters.WorkArea.Width;
            GameTheme = GameSetter.GetRandomGameTheme();
            ExpandGameSetterPanel();
            // GamePlayAreaCanvas.Children.Add(_directionLine);
        }

        private async void LLKToken_Click(object sender, RoutedEventArgs e) {
            LLKToken token = (LLKToken)(sender as FrameworkElement).Tag;
            await _game.SelectTokenAsync(token);
        }
        private async void ActiveSkill_Click(object sender, SClickEventArgs e) {
            await _game.ActiveSkillAsync(e.SKill);
        }
        private void StartGame_Click(object sender, RoutedEventArgs e) {
            try {
                StartGame();
                GetGameTheme();
                FoldGameSetterPanel();
                _startTime = DateTime.Now;
            }
            catch (Exception exp) {
                MessageBox.Show(exp.Message, "", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        private void Game_GameCompleted(object sender, GameCompletedEventArgs e) {
            // 模糊背景
            //BlurEffect effect = new BlurEffect();
            //DoubleAnimation effectAnimation = new DoubleAnimation() {
            //    To = 15,
            //    AccelerationRatio = 0.2,
            //    DecelerationRatio = 0.8,
            //    Duration = TimeSpan.FromMilliseconds(150)
            //};
            //GameArea.Effect = effect;
            //effect.BeginAnimation(BlurEffect.RadiusProperty, effectAnimation);
            // 弹出统计窗口
            var gameUsingTime = (DateTime.Now - _startTime).TotalMilliseconds / 1000.0;
            var totalScores = (int)(e.TotalScores / Math.Log(gameUsingTime));
            View.GameCompletedWindow gcw = new View.GameCompletedWindow(e, gameUsingTime, 0, totalScores);
            gcw.Owner = this;
            gcw.ShowDialog();
            // 取消模糊背景
            //GameArea.Effect = null;
            ExpandGameSetterPanel();
        }
        private void ExpandGameSetter_Click(object sender, RoutedEventArgs e) {
            if (GameSetterPanel.Height != 0) {
                FoldGameSetterPanel();
            }
            else {
                ExpandGameSetterPanel();
            }
        }

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
    }
}