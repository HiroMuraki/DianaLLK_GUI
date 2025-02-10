using System;
using System.ComponentModel;

namespace LianLianKan
{
    public class LLKToken : INotifyPropertyChanged
    {
        public LLKToken(LLKTokenType content, Coordinate coordinate)
        {
            _tokenType = content;
            _isSelected = false;
            _coordinate = coordinate;
        }

        #region 公开事件
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<TokenMatchedEventArgs> Matched;
        public event EventHandler<TokenSelectedEventArgs> Selected;
        public event EventHandler<TokenResetedEventArgs> Reseted;
        #endregion

        public Coordinate Coordinate
        {
            get
            {
                return _coordinate;
            }
        }

        public LLKTokenType TokenType
        {
            get
            {
                return _tokenType;
            }
            set
            {
                _tokenType = value;
                OnPropertyChanged(nameof(TokenType));
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged(nameof(IsSelected));
            }
        }

        public void OnMatched()
        {
            Matched?.Invoke(this, new TokenMatchedEventArgs(_tokenType, true));
        }

        public void OnMatched(TokenMatchedEventArgs e)
        {
            Matched?.Invoke(this, e);
        }

        public void OnReset()
        {
            Reseted?.Invoke(this, new TokenResetedEventArgs());
        }

        public void OnSelected()
        {
            Selected?.Invoke(this, new TokenSelectedEventArgs(_tokenType));
        }

        public static bool IsSameType(LLKToken left, LLKToken right)
        {
            return left._tokenType == right._tokenType;
        }

        public override string ToString()
        {
            if (_tokenType == LLKTokenType.None)
            {
                return "?";
            }
            return _tokenType.ToString();
        }

        #region NonPublic
        private readonly Coordinate _coordinate;
        private LLKTokenType _tokenType;
        private bool _isSelected;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}