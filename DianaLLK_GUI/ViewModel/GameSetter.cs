using LianLianKan;
using System.ComponentModel;

namespace DianaLLK_GUI.ViewModel
{
    public class GameSetter : INotifyPropertyChanged
    {
        public static GameSetter Instance { get; } = new();

        public event PropertyChangedEventHandler PropertyChanged;

        public int RowSize
        {
            get => _rowSize;
            set
            {
                if (value < MinSize || value > MaxSize)
                {
                    return;
                }
                _rowSize = value;
                OnPropertyChanged(nameof(RowSize));
            }
        }

        public int ColumnSize
        {
            get => _columnSize;
            set
            {
                _columnSize = value;
                if (value < MinSize || value > MaxSize)
                {
                    return;
                }
                OnPropertyChanged(nameof(ColumnSize));
            }
        }

        public int TokenAmount
        {
            get => _tokenAmount;
            set
            {
                if (value < MinTokenAmount)
                {
                    return;
                }
                _tokenAmount = value;
                OnPropertyChanged(nameof(TokenAmount));
            }
        }

        public int MinSize => 6;

        public int MaxSize => 25;

        public int MinTokenAmount => 6;

        public int MaxTokenAmount => LLKHelper.NumTokenTypes;

        public LLKTokenType CurrentAvatar => GetRandomTokenType();

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

        #region NonPublic
        private GameSetter()
        {

        }
        private int _rowSize;
        private int _columnSize;
        private int _tokenAmount;
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
