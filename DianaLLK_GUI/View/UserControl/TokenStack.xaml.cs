﻿using LianLianKan;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DianaLLK_GUI.View {
    /// <summary>
    /// TokenStack.xaml 的交互逻辑
    /// </summary>
    public partial class TokenStack : UserControl {
        private const int _identifierWidth = 10;
        private double _aLsX;
        private double _bLsX;
        private double _cLsX;
        private double _dLsX;
        private double _eLsX;

        public TokenStack() {
            _aLsX = _identifierWidth;
            _bLsX = _identifierWidth;
            _cLsX = _identifierWidth;
            _dLsX = _identifierWidth;
            _eLsX = _identifierWidth;
            InitializeComponent();
        }

        public void AddToStack(LLKTokenType tokenType) {
            Border img = new Border() {
                Height = 90,
                Width = 90,
                Margin = new Thickness(5),
                CornerRadius = new CornerRadius(20),
                Background = App.ImageDict[tokenType.ToString()] as ImageBrush,
                BorderThickness = new Thickness(2),
            };
            switch (tokenType) {
                case LLKTokenType.None:
                case LLKTokenType.AS:
                    break;
                case LLKTokenType.A1:
                case LLKTokenType.A2:
                case LLKTokenType.A3:
                case LLKTokenType.A4:
                case LLKTokenType.A5:
                    img.BorderBrush = App.ColorDict["AvaTheme"] as SolidColorBrush;
                    img.SetValue(Canvas.LeftProperty, _aLsX);
                    _aLsX += 5;
                    ATokenStack.Children.Add(img);
                    break;
                case LLKTokenType.B1:
                case LLKTokenType.B2:
                case LLKTokenType.B3:
                case LLKTokenType.B4:
                case LLKTokenType.B5:
                    img.BorderBrush = App.ColorDict["BellaTheme"] as SolidColorBrush;
                    img.SetValue(Canvas.LeftProperty, _bLsX);
                    _bLsX += 5;
                    BTokenStack.Children.Add(img);
                    break;
                case LLKTokenType.C1:
                case LLKTokenType.C2:
                case LLKTokenType.C3:
                case LLKTokenType.C4:
                case LLKTokenType.C5:
                    img.BorderBrush = App.ColorDict["CarolTheme"] as SolidColorBrush;
                    img.SetValue(Canvas.LeftProperty, _cLsX);
                    _cLsX += 5;
                    CTokenStack.Children.Add(img);
                    break;
                case LLKTokenType.D1:
                case LLKTokenType.D2:
                case LLKTokenType.D3:
                case LLKTokenType.D4:
                case LLKTokenType.D5:
                    img.BorderBrush = App.ColorDict["DianaTheme"] as SolidColorBrush;
                    img.SetValue(Canvas.LeftProperty, _dLsX);
                    _dLsX += 5;
                    DTokenStack.Children.Add(img);
                    break;
                case LLKTokenType.E1:
                case LLKTokenType.E2:
                case LLKTokenType.E3:
                case LLKTokenType.E4:
                case LLKTokenType.E5:
                    img.BorderBrush = App.ColorDict["EileenTheme"] as SolidColorBrush;
                    img.SetValue(Canvas.LeftProperty, _eLsX);
                    _eLsX += 5;
                    ETokenStack.Children.Add(img);
                    break;
                default:
                    break;
            }
        }
        public void ResetStack() {
            _aLsX = _identifierWidth;
            _bLsX = _identifierWidth;
            _cLsX = _identifierWidth;
            _dLsX = _identifierWidth;
            _eLsX = _identifierWidth;
            ATokenStack.Children.Clear();
            BTokenStack.Children.Clear();
            CTokenStack.Children.Clear();
            DTokenStack.Children.Clear();
            ETokenStack.Children.Clear();
        }
    }
}
