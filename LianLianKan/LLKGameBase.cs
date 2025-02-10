using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        protected readonly CurrentTokenTypes _currentTokenTypes;
        protected readonly Dictionary<Coordinate, bool> _coordinateChecked;
        protected readonly object _processLocker;
        protected readonly object _gameLayoutLocker;
        protected LLKToken[,] _gameLayout;
        protected LLKToken _heldToken;
        protected int _rowSize;
        protected int _columnSize;
        protected GameType _gameType;

        #region 公开事件
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<GameCompletedEventArgs> GameCompleted;
        public event EventHandler<LayoutResetedEventArgs> LayoutReseted;
        #endregion

        #region 公开属性
        public CurrentTokenTypes CurrentTokenTypes
        {
            get
            {
                return _currentTokenTypes;
            }
        }
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
        public int RowSize
        {
            get
            {
                return _rowSize;
            }
        }
        public int ColumnSize
        {
            get
            {
                return _columnSize;
            }
        }
        public GameType GameType
        {
            get
            {
                return _gameType;
            }
        }
        #endregion

        #region 构造方法
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
            var numberArray = testLayoutString.Split(' ');

            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    int value = Convert.ToInt32(numberArray[(row * _columnSize) + col]);
                    _gameLayout[row, col] = new LLKToken((LLKTokenType)value, new Coordinate(row, col));
                }
            }
        }
        #endregion

        #region 公开方法
        public virtual void StartGame(int rowSize, int columnSize, int tokenAmount)
        {
            StartGameHelper(() =>
            {
                GenerateGameLayout(rowSize, columnSize, tokenAmount);
            });
            _gameType = GameType.New;
            LayoutReseted?.Invoke(this, new LayoutResetedEventArgs());
        }
        public virtual async Task StartGameAsync(int rowSize, int columnSize, int tokenAmount)
        {
            await Task.Run(() =>
            {
                lock (_gameLayoutLocker)
                {
                    StartGameHelper(() =>
                    {
                        GenerateGameLayout(rowSize, columnSize, tokenAmount);
                    });
                    _gameType = GameType.New;
                }
            });
            LayoutReseted?.Invoke(this, new LayoutResetedEventArgs());
        }
        public virtual void RestoreGame(LLKTokenType[,] tokenTypes, int tokenAmount)
        {
            StartGameHelper(() =>
            {
                RestoreGameLayout(tokenTypes, tokenAmount);
            });
            _gameType = GameType.Restored;
            LayoutReseted?.Invoke(this, new LayoutResetedEventArgs());
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
            LayoutReseted?.Invoke(this, new LayoutResetedEventArgs());
        }
        public virtual TokenSelectResult SelectToken(LLKToken token)
        {
            var matchedTokenType = _heldToken?.TokenType;
            var a = _heldToken;
            var b = token;
            TokenSelectResult tokenSelectResult = SelectTokenHelper(token);
            if (tokenSelectResult == TokenSelectResult.Matched)
            {
                a.OnMatched();
                b.OnMatched();
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
                a.OnSelected();
            }
            return tokenSelectResult;
        }
        public virtual async Task<TokenSelectResult> SelectTokenAsync(LLKToken token)
        {
            var matchedTokenType = _heldToken?.TokenType;
            var a = _heldToken;
            var b = token;
            TokenSelectResult tokenSelectResult = await Task.Run(() =>
            {
                lock (_processLocker)
                {
                    return SelectTokenHelper(token);
                }
            });
            if (tokenSelectResult == TokenSelectResult.Matched)
            {
                a.OnMatched(new TokenMatchedEventArgs(matchedTokenType.Value, true));
                b.OnMatched(new TokenMatchedEventArgs(matchedTokenType.Value, true));
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
            StringBuilder sb = new StringBuilder();
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
        #endregion

        protected virtual TokenSelectResult SelectTokenHelper(LLKToken token)
        {
            // 如果待选token为null，且tokenType不为None，则将待选token设为当前token
            if (_heldToken == null)
            {
                if (token.TokenType == LLKTokenType.None)
                {
                    return TokenSelectResult.None;
                }
                _heldToken = token;
                _heldToken.IsSelected = true;
                return TokenSelectResult.Wait;
            }
            // 如果点选的是空白，则重置待选token
            if (token.TokenType == LLKTokenType.None)
            {
                _heldToken.IsSelected = false;
                _heldToken = null;
                return TokenSelectResult.Reset;
            }
            // 如果点选的位置和上次位置相同，跳过
            if (_heldToken == token)
            {
                return TokenSelectResult.None;
            }
            // 常规比较，如果类型符合则连接，否则将目标token设置为heldToken
            if (IsMatchable(_heldToken.Coordinate, token.Coordinate))
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
            _gameLayout[aPos.Row, aPos.Column].TokenType = LLKTokenType.None;
            _gameLayout[bPos.Row, bPos.Column].TokenType = LLKTokenType.None;
            _gameLayout[aPos.Row, aPos.Column].IsSelected = false;
            _gameLayout[bPos.Row, bPos.Column].IsSelected = false;
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

            Random rnd = new Random();
            // 获取可用的token类型
            CurrentTokenTypes allTokens = new CurrentTokenTypes();
            foreach (var item in Enum.GetValues(typeof(LLKTokenType)))
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
                var tokenType = allTokens[rnd.Next(0, allTokens.Count)];
                allTokens.Remove(tokenType);
                _currentTokenTypes.Add(tokenType);
            }
            // 随机添加token
            CurrentTokenTypes tokenTypes = new CurrentTokenTypes(capacity);
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
                var t = tokenTypes[i];
                tokenTypes[i] = tokenTypes[j];
                tokenTypes[j] = t;
            }
            // 添加至游戏布局
            _rowSize = rowSize;
            _columnSize = columnSize;
            _gameLayout = new LLKToken[_rowSize, _columnSize];
            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    _gameLayout[row, col] = new LLKToken(tokenTypes[(row * _columnSize) + col], new Coordinate(row, col));
                }
            }
        }
        protected virtual void RestoreGameLayout(LLKTokenType[,] tokenTypes, int tokenAmount)
        {
            // 恢复布局信息
            _rowSize = tokenTypes.GetLength(0);
            _columnSize = tokenTypes.GetLength(1);
            _gameLayout = new LLKToken[_rowSize, _columnSize];
            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    _gameLayout[row, col] = new LLKToken(tokenTypes[row, col], new Coordinate(row, col));
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
        protected virtual bool IsMatchable(Coordinate startCoordinate, Coordinate targetCoordinate)
        {
            var result = IsConnectable(startCoordinate, targetCoordinate);
            ResetCoordinateStatus();
            // 如果不连通，直接返回false
            if (result == false)
            {
                return false;
            }
            // 如果连通
            else
            {
                // 如果token相同，返回true
                if (LLKToken.IsSameType(_gameLayout[startCoordinate.Row, startCoordinate.Column],
                    _gameLayout[targetCoordinate.Row, targetCoordinate.Column]))
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
        protected virtual bool IsConnectable(Coordinate startCoordinate, Coordinate targetCoordinate)
        {
            // 先将自己标记为已检查
            _coordinateChecked[startCoordinate] = true;
            // 设置待检查列表
            List<Coordinate> checkList = new List<Coordinate>();
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    // 获取新位置
                    int nRow = startCoordinate.Row + i;
                    int nCol = startCoordinate.Column + j;
                    Coordinate nCoordinate = new Coordinate(nRow, nCol);
                    // 跳过四角
                    if ((i == -1 || i == 1) && (j == -1 || j == 1))
                    {
                        continue;
                    }
                    // 跳过中心
                    if (i == 0 && j == 0)
                    {
                        continue;
                    }
                    // 跳过负位置
                    if (nRow < 0 || nCol < 0)
                    {
                        continue;
                    }
                    // 跳过越界位置
                    if (nRow >= _rowSize || nCol >= _columnSize)
                    {
                        continue;
                    }
                    // 跳过已检查
                    if (_coordinateChecked[nCoordinate] == true)
                    {
                        continue;
                    }
                    checkList.Add(nCoordinate);
                }
            }
            // 开始检查
            foreach (var coordinate in checkList)
            {
                // 检查该位置是否为目标位置，是立即返回True
                if (coordinate == targetCoordinate)
                {
                    _coordinateChecked[coordinate] = true;
                    return true;
                }
                // 否则如果该位置是空的话，则递归检查
                else if (_gameLayout[coordinate.Row, coordinate.Column].TokenType == LLKTokenType.None)
                {
                    bool result = IsConnectable(coordinate, targetCoordinate);
                    // 递归检查结果为true，直接返回
                    if (result == true)
                    {
                        return result;
                    }
                    // 否则如果待查列表中还存在未检查坐标，则跳过
                    bool allChecked = true;
                    foreach (var pos in checkList)
                    {
                        if (_coordinateChecked[pos] == false)
                        {
                            allChecked = false;
                            break;
                        }
                    }
                    // 否则若所有坐标都检查完毕，返回false
                    if (allChecked)
                    {
                        return result;
                    }
                }
                // 否则跳过此方块
                else
                {
                    continue;
                }
            }
            // 若检查完毕后到了这里，说明没有直接连同
            return false;
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
        protected void OnLayoutRested(LayoutResetedEventArgs e)
        {
            LayoutReseted?.Invoke(this, e);
        }
    }
}