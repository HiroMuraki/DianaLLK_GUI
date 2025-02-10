using LianLianKan;
using System.ComponentModel;

namespace DianaLLK_GUI.ViewModel
{
    public class GameSetter : INotifyPropertyChanged
    {
        private static GameSetter _singletonObject;
        private static readonly int _minSize;
        private static readonly int _maxSize;
        private static readonly int _minTokenAmount;
        private int _rowSize;
        private int _columnSize;
        private int _tokenAmount;

        public event PropertyChangedEventHandler PropertyChanged;

        public int RowSize
        {
            get
            {
                return _rowSize;
            }
            set
            {
                if (value < _minSize || value > _maxSize)
                {
                    return;
                }
                _rowSize = value;
                OnPropertyChanged(nameof(RowSize));
            }
        }
        public int ColumnSize
        {
            get
            {
                return _columnSize;
            }
            set
            {
                _columnSize = value;
                if (value < _minSize || value > _maxSize)
                {
                    return;
                }
                OnPropertyChanged(nameof(ColumnSize));
            }
        }
        public int TokenAmount
        {
            get
            {
                return _tokenAmount;
            }
            set
            {
                if (value < _minTokenAmount)
                {
                    return;
                }
                _tokenAmount = value;
                OnPropertyChanged(nameof(TokenAmount));
            }
        }
        public int MinSize
        {
            get
            {
                return _minSize;
            }
        }
        public int MaxSize
        {
            get
            {
                return _maxSize;
            }
        }
        public int MinTokenAmount
        {
            get
            {
                return _minTokenAmount;
            }
        }
        public int MaxTokenAmount
        {
            get
            {
                return LLKHelper.NumTokenTypes;
            }
        }
        public LLKTokenType CurrentAvatar
        {
            get
            {
                return GetRandomTokenType();
            }
        }

        static GameSetter()
        {
            _singletonObject = new GameSetter();
            _minSize = 6;
            _maxSize = 25;
            _minTokenAmount = 6;
        }
        private GameSetter()
        {

        }
        public static GameSetter GetInstance()
        {
            _singletonObject ??= new GameSetter();
            return _singletonObject;
        }

        public static LLKTokenType GetRandomTokenType()
        {
            return LLKHelper.GetRandomTokenType();
        }
        public static TokenCategory GetRandomGameTheme()
        {
            return LLKHelper.GetRandomTokenCategory();
        }
        public void OnCurrentAvatarChanged()
        {
            OnPropertyChanged(nameof(CurrentAvatar));
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
