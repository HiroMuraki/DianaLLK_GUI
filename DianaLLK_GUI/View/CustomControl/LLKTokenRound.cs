using LianLianKan;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace DianaLLK_GUI.View
{
    public class LLKTokenRound : Button
    {
        static LLKTokenRound()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LLKTokenRound), new FrameworkPropertyMetadata(typeof(LLKTokenRound)));
        }

        public LLKTokenRound(LLKToken token) : this()
        {
            Token = token;
            Token.Selected += Token_Selected;
            Token.Matched += Token_Matched;
            Token.Reseted += Token_Reset;
        }

        public static readonly DependencyProperty SelectedHighlighterOpacityProperty =
            DependencyProperty.Register(nameof(SelectedHighlighterOpacity), typeof(double), typeof(LLKTokenRound), new PropertyMetadata(0.0));

        public static readonly DependencyProperty HoveredHighlighterOpacityProperty =
            DependencyProperty.Register(nameof(HoveredHighlighterOpacity), typeof(double), typeof(LLKTokenRound), new PropertyMetadata(0.0));

        public static readonly DependencyProperty TokenProperty =
            DependencyProperty.Register(nameof(Token), typeof(LLKToken), typeof(LLKTokenRound), new PropertyMetadata(null));

        public event EventHandler<TClickEventArgs> TClick;

        public double SelectedHighlighterOpacity
        {
            get => (double)GetValue(SelectedHighlighterOpacityProperty);
            set => SetValue(SelectedHighlighterOpacityProperty, value);
        }

        public double HoveredHighlighterOpacity
        {
            get => (double)GetValue(HoveredHighlighterOpacityProperty);
            set => SetValue(HoveredHighlighterOpacityProperty, value);
        }

        public LLKToken Token
        {
            get => (LLKToken)GetValue(TokenProperty);
            set => SetValue(TokenProperty, value);
        }

        #region NonPublic
        protected override void OnClick()
        {
            base.OnClick();
            TClick?.Invoke(this, new TClickEventArgs(Token));
        }
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            BeginAnimation(HoveredHighlighterOpacityProperty, _hoveredAnimation);
        }
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            BeginAnimation(HoveredHighlighterOpacityProperty, _resetAnimation);
        }
        private LLKTokenRound()
        {
            _flickAnimation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                EasingFunction = new BounceEase(),
                Duration = TimeSpan.FromMilliseconds(200)
            };
            _hoveredAnimation = new DoubleAnimation()
            {
                To = 0.65,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(15)
            };
            _selectedAnimation = new DoubleAnimation()
            {
                To = 0.85,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(15)
            };
            _resetAnimation = new DoubleAnimation()
            {
                To = 0,
                AccelerationRatio = 0.2,
                DecelerationRatio = 0.8,
                Duration = TimeSpan.FromMilliseconds(15)
            };
        }
        private readonly DoubleAnimation _flickAnimation;
        private readonly DoubleAnimation _selectedAnimation;
        private readonly DoubleAnimation _hoveredAnimation;
        private readonly DoubleAnimation _resetAnimation;
        private void Token_Selected(object sender, TokenSelectedEventArgs e)
        {
            base.OnClick();
            BeginAnimation(SelectedHighlighterOpacityProperty, _selectedAnimation);
            TClick?.Invoke(this, new TClickEventArgs(Token));
        }
        private void Token_Matched(object sender, TokenMatchedEventArgs e)
        {
            BeginAnimation(OpacityProperty, _flickAnimation);
        }
        private void Token_Reset(object sender, TokenResetedEventArgs e)
        {
            BeginAnimation(SelectedHighlighterOpacityProperty, _resetAnimation);
            BeginAnimation(HoveredHighlighterOpacityProperty, _resetAnimation);
        }
        #endregion
    }
}
