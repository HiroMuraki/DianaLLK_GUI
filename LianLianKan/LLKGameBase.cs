using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CurrentTokenTypes = System.Collections.Generic.List<LianLianKan.LLKTokenType>;
using LLKTokens = System.Collections.Generic.IEnumerable<LianLianKan.LLKToken>;
using LLKTokenTypes = System.Collections.Generic.IEnumerable<LianLianKan.LLKTokenType>;

namespace LianLianKan
{
    public class LLKGameBase : INotifyPropertyChanged
    {
        public LLKGameBase()
        {
            _rowSize = 0;
            _columnSize = 0;
            _gameLayout = new LLKToken[0, 0];
            _coordinateChecked = new Dictionary<Coordinate, bool>();
            _currentTokenTypes = new CurrentTokenTypes();
            _heldToken = null;
            _processLocker = new object();
            _gameLayoutLocker = new object();
        }

        public LLKGameBase(string testLayoutString) : this()
        {
            _gameLayout = new LLKToken[_rowSize, _columnSize];
            testLayoutString = Regex.Replace(testLayoutString, @"[\s]+", " ");
            string[] numberArray = testLayoutString.Split(' ');

            for (int y = 0; y < _rowSize; y++)
            {
                for (int x = 0; x < _columnSize; x++)
                {
                    int value = Convert.ToInt32(numberArray[(y * _columnSize) + x]);
                    _gameLayout[y, x] = new LLKToken((LLKTokenType)value, new Coordinate(x, y));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<GameCompletedEventArgs> GameCompleted;

        public event EventHandler<LayoutResetEventArgs> LayoutReseted;

        public event EventHandler<TokensLinkedEventArgs> TokensLinked;

        public LLKToken this[Coordinate coordinate] => _gameLayout[coordinate.Y, coordinate.X];

        public CurrentTokenTypes CurrentTokenTypes => _currentTokenTypes;

        public LLKTokens LLKTokenArray
        {
            get
            {
                for (int row = 0; row < _rowSize; row++)
                {
                    for (int col = 0; col < _columnSize; col++)
                    {
                        yield return _gameLayout[row, col];
                    }
                }
            }
        }

        public LLKTokenTypes TokenTypeArray
        {
            get
            {
                for (int row = 0; row < _rowSize; row++)
                {
                    for (int col = 0; col < _columnSize; col++)
                    {
                        yield return _gameLayout[row, col].TokenType;
                    }
                }
            }
        }

        public int RowSize => _rowSize;

        public int ColumnSize => _columnSize;

        public GameType GameType => _gameType;

        public virtual void StartGame(int rowSize, int columnSize, int tokenAmount)
        {
            StartGameHelper(() =>
            {
                GenerateGameLayout(rowSize, columnSize, tokenAmount);
            });
            _gameType = GameType.New;
            LayoutReseted?.Invoke(this, new LayoutResetEventArgs());
        }

        public virtual void RestoreGame(LLKTokenType[,] tokenTypes, int tokenAmount)
        {
            StartGameHelper(() =>
            {
                RestoreGameLayout(tokenTypes, tokenAmount);
            });
            _gameType = GameType.Restored;
            LayoutReseted?.Invoke(this, new LayoutResetEventArgs());
        }

        public virtual async Task RestoreGameAsync(LLKTokenType[,] tokenTypes, int tokenAmount)
        {
            await Task.Run(() =>
            {
                lock (_gameLayoutLocker)
                {
                    StartGameHelper(() =>
                    {
                        RestoreGameLayout(tokenTypes, tokenAmount);
                    });
                }
            });
            _gameType = GameType.Restored;
            LayoutReseted?.Invoke(this, new LayoutResetEventArgs());
        }

        public virtual TokenSelectResult SelectToken(LLKToken token)
        {
            LLKTokenType? matchedTokenType = _heldToken?.TokenType;
            LLKToken a = _heldToken;
            LLKToken b = token;
            TokenSelectResult tokenSelectResult = SelectTokenHelper(token, out Coordinate[] nodes);
            if (tokenSelectResult == TokenSelectResult.Matched)
            {
                a.OnMatched(new TokenMatchedEventArgs(matchedTokenType.Value));
                b.OnMatched(new TokenMatchedEventArgs(matchedTokenType.Value));
                TokensLinked?.Invoke(this, new TokensLinkedEventArgs(a, b, nodes));
                if (IsGameCompleted())
                {
                    int scores = GetTotalScores();
                    GameCompleted?.Invoke(this, new GameCompletedEventArgs(scores, _currentTokenTypes.Count, _rowSize, _columnSize, _gameType));
                }
            }
            else if (tokenSelectResult == TokenSelectResult.Reset)
            {
                a.OnReset();
                b.OnReset();
            }
            else if (tokenSelectResult == TokenSelectResult.Wait)
            {
                _heldToken.OnSelected();
            }
            return tokenSelectResult;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    sb.Append($"{_gameLayout[row, col]} ");
                }
                sb.Append('\n');
            }
            return sb.ToString();
        }

        #region NonPublic
        protected readonly CurrentTokenTypes _currentTokenTypes;
        protected readonly Dictionary<Coordinate, bool> _coordinateChecked;
        protected readonly object _processLocker;
        protected readonly object _gameLayoutLocker;
        protected LLKToken[,] _gameLayout;
        protected LLKToken _heldToken;
        protected List<LLKToken> GetTokens() => _gameLayout.OfType<LLKToken>().ToList();
        protected int _rowSize;
        protected int _columnSize;
        protected GameType _gameType;
        protected virtual TokenSelectResult SelectTokenHelper(LLKToken token, out Coordinate[] nodes)
        {
            // 如果待选token为null，且tokenType不为None，则将待选token设为当前token
            if (_heldToken == null)
            {
                if (token.TokenType == LLKTokenType.None)
                {
                    nodes = null;
                    return TokenSelectResult.None;
                }
                _heldToken = token;
                _heldToken.IsSelected = true;
                nodes = null;
                return TokenSelectResult.Wait;
            }
            // 如果点选的是空白，则重置待选token
            if (token.TokenType == LLKTokenType.None)
            {
                _heldToken.IsSelected = false;
                _heldToken = null;
                nodes = null;
                return TokenSelectResult.Reset;
            }
            // 如果点选的位置和上次位置相同，跳过
            if (_heldToken == token)
            {
                nodes = null;
                return TokenSelectResult.None;
            }
            // 常规比较，如果类型符合则连接，否则将目标token设置为heldToken
            if (IsMatchable(_heldToken.Coordinate, token.Coordinate, out nodes))
            {
                MatchTokensHelper(_heldToken, token);
                _heldToken = null;
                return TokenSelectResult.Matched;
            }
            else
            {
                _heldToken.IsSelected = false;
                _heldToken = null;
                return TokenSelectResult.Reset;
            }
        }
        protected virtual void MatchTokensHelper(LLKToken a, LLKToken b)
        {
            Coordinate aPos = a.Coordinate;
            Coordinate bPos = b.Coordinate;
            _gameLayout[aPos.Y, aPos.X].TokenType = LLKTokenType.None;
            _gameLayout[bPos.Y, bPos.X].TokenType = LLKTokenType.None;
            _gameLayout[aPos.Y, aPos.X].IsSelected = false;
            _gameLayout[bPos.Y, bPos.X].IsSelected = false;
        }
        protected virtual int GetTotalScores()
        {
            return _rowSize * _columnSize * _currentTokenTypes.Count * 100;
        }
        protected virtual void GenerateGameLayout(int rowSize, int columnSize, int tokenAmount)
        {
            // 首先随机挑选一个token，将该token生成两份
            // token添加到列表中
            // 洗牌算法打乱列表
            // 列表内容复制进游戏布局即可

            int capacity = rowSize * columnSize;

            // 如果总容量无法被2整除，则无法生成游戏
            if (capacity % 2 != 0)
            {
                throw new ArgumentException("行列设置无效，行列数乘积应为偶数");
            }

            var rnd = new Random();

            // 获取可用的token类型
            var allTokens = new CurrentTokenTypes();
            foreach (object item in Enum.GetValues(typeof(LLKTokenType)))
            {
                var tokenType = (LLKTokenType)item;
                if (tokenType == LLKTokenType.None)
                {
                    continue;
                }
                allTokens.Add(tokenType);
            }

            // 挑选token
            _currentTokenTypes.Clear();
            while (_currentTokenTypes.Count < tokenAmount)
            {
                if (allTokens.Count == 0)
                {
                    break;
                }
                LLKTokenType tokenType = allTokens[rnd.Next(0, allTokens.Count)];
                allTokens.Remove(tokenType);
                _currentTokenTypes.Add(tokenType);
            }

            // 随机添加token
            var tokenTypes = new CurrentTokenTypes(capacity);
            int cycleTimes = capacity / 2;
            for (int i = 0; i < cycleTimes; i++)
            {
                LLKTokenType selectedType;
                // 优先各种类型至少填充一次
                if (i < tokenAmount)
                {
                    selectedType = _currentTokenTypes[i];
                }
                // 之后随机选择
                else
                {
                    selectedType = _currentTokenTypes[rnd.Next(0, _currentTokenTypes.Count)];
                }
                tokenTypes.Add(selectedType);
                tokenTypes.Add(selectedType);
            }

            // 洗牌算法打乱token顺序
            for (int i = 0; i < capacity; i++)
            {
                int j = rnd.Next(i, capacity);
                (tokenTypes[j], tokenTypes[i]) = (tokenTypes[i], tokenTypes[j]);
            }

            // 添加至游戏布局
            _rowSize = rowSize;
            _columnSize = columnSize;
            _gameLayout = new LLKToken[_rowSize, _columnSize];
            for (int y = 0; y < _rowSize; y++)
            {
                for (int x = 0; x < _columnSize; x++)
                {
                    _gameLayout[y, x] = new LLKToken(tokenTypes[(y * _columnSize) + x], new Coordinate(x, y));
                }
            }
        }
        protected virtual void RestoreGameLayout(LLKTokenType[,] tokenTypes, int tokenAmount)
        {
            // 恢复布局信息
            _rowSize = tokenTypes.GetLength(0);
            _columnSize = tokenTypes.GetLength(1);
            _gameLayout = new LLKToken[_rowSize, _columnSize];
            for (int y = 0; y < _rowSize; y++)
            {
                for (int x = 0; x < _columnSize; x++)
                {
                    _gameLayout[y, x] = new LLKToken(tokenTypes[y, x], new Coordinate(x, y));
                }
            }
            // 恢复成员类数信息
            _currentTokenTypes.Clear();
            for (int i = 0; i < tokenAmount; i++)
            {
                _currentTokenTypes.Add(LLKTokenType.AS);
            }
        }
        protected virtual void StartGameHelper(Action gameLayoutGenerateCallBack)
        {
            if (gameLayoutGenerateCallBack == null)
            {
                return;
            }
            _heldToken = null;
            // 通过回调方法生成布局
            gameLayoutGenerateCallBack?.Invoke();
            // 更新坐标检测
            _coordinateChecked.Clear();
            ResetCoordinateStatus();
            OnPropertyChanged(nameof(RowSize));
            OnPropertyChanged(nameof(ColumnSize));
        }
        protected virtual bool IsMatchable(Coordinate startCoordinate, Coordinate targetCoordinate, out Coordinate[] nodes)
        {
            bool isConnectable = LinkGameHelper.TryConnect(
                GetTokens(),
                startCoordinate,
                targetCoordinate,
                out nodes);

            ResetCoordinateStatus();
            // 如果不连通，直接返回false
            if (isConnectable == false)
            {
                return false;
            }
            // 如果连通
            else
            {
                // 如果token相同，返回true
                if (LLKToken.IsSameType(_gameLayout[startCoordinate.Y, startCoordinate.X],
                    _gameLayout[targetCoordinate.Y, targetCoordinate.X]))
                {
                    return true;
                }
                // 如果连通但类型不同，返回false
                else
                {
                    return false;
                }
            }
        }
        protected virtual bool IsGameCompleted()
        {
            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    if (_gameLayout[row, col].TokenType != LLKTokenType.None)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        protected void ResetCoordinateStatus()
        {
            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    _coordinateChecked[_gameLayout[row, col].Coordinate] = false;
                }
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        protected void OnGameCompleted(GameCompletedEventArgs e)
        {
            GameCompleted?.Invoke(this, e);
        }
        protected void OnLayoutRested(LayoutResetEventArgs e)
        {
            LayoutReseted?.Invoke(this, e);
        }
        #endregion
    }
}