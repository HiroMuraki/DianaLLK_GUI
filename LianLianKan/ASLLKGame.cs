using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace LianLianKan
{
    public sealed class ASLLKGame : LLKGameBase, INotifyPropertyChanged
    {
        public ASLLKGame() : base()
        {
            _skillLocker = new object();
        }

        public event EventHandler<SkillActivatedEventArgs> SkillActived;

        public int SkillPoint { get; private set; }

        public override void StartGame(int rowSize, int columnSize, int tokenAmount)
        {
            base.StartGame(rowSize, columnSize, tokenAmount);
            SkillPoint = GetSkillPoint();
            OnPropertyChanged(nameof(SkillPoint));
        }

        public override async Task StartGameAsync(int rowSize, int columnSize, int tokenAmount)
        {
            await base.StartGameAsync(rowSize, columnSize, tokenAmount);
            SkillPoint = GetSkillPoint();
            OnPropertyChanged(nameof(SkillPoint));
        }

        public void RestoreGame(IGameRestore restorePack)
        {
            base.RestoreGame(restorePack.TokenTypes, restorePack.TokenAmount);
            SkillPoint = restorePack.SkillPoint;
            OnPropertyChanged(nameof(SkillPoint));
        }

        public async Task RestoreGameAsync(IGameRestore restorePack)
        {
            await base.RestoreGameAsync(restorePack.TokenTypes, restorePack.TokenAmount);
            SkillPoint = restorePack.SkillPoint;
            OnPropertyChanged(nameof(SkillPoint));
        }

        public void ActiveSkill(LLKSkill skill)
        {
            bool isActivated = ActiveSkillHelper(skill);
            if (isActivated)
            {
                if (skill == LLKSkill.AvaPower)
                {
                    OnLayoutRested(new LayoutResetEventArgs());
                }
            }
            SkillActived?.Invoke(this, new SkillActivatedEventArgs(skill, isActivated));
        }

        public async Task ActiveSkillAsync(LLKSkill skill)
        {
            bool isActivated = await Task.Run(() =>
            {
                lock (_skillLocker)
                {
                    return ActiveSkillHelper(skill);
                }
            });
            if (isActivated)
            {
                if (skill == LLKSkill.AvaPower)
                {
                    OnLayoutRested(new LayoutResetEventArgs());
                }
            }
            SkillActived?.Invoke(this, new SkillActivatedEventArgs(skill, isActivated));
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
        protected override TokenSelectResult SelectTokenHelper(LLKToken token)
        {
            LLKToken heldToken = _heldToken;
            LLKToken currentToken = token;
            LLKTokenType? heldTokenType = _heldToken?.TokenType;
            LLKTokenType? currentTokenType = token?.TokenType;
            TokenSelectResult tokenSelectResult = base.SelectTokenHelper(token);
            if (tokenSelectResult == TokenSelectResult.Reset)
            {
                // 如果启用了贝拉Power
                if (_isBellaPowerOn)
                {
                    if (heldToken.TokenType == currentToken.TokenType)
                    {
                        MatchTokensHelper(heldToken, currentToken);
                        _isBellaPowerOn = false;
                        tokenSelectResult = TokenSelectResult.Matched;
                    }
                    else
                    {
                        tokenSelectResult = TokenSelectResult.Reset;
                    }
                }
                // 如果启用了乃琳Power
                if (_isEileenPowerOn)
                {
                    if (LinkGameHelper.TryConnect(
                        GetTokens(),
                        heldToken.Coordinate,
                        currentToken.Coordinate,
                        out _))
                    {
                        LLKTokenType fixTypeA = heldToken.TokenType;
                        LLKTokenType fixTypeB = currentToken.TokenType;
                        var typeAList = new List<LLKToken>();
                        var typeBList = new List<LLKToken>();
                        var rnd = new Random();
                        MatchTokensHelper(heldToken, currentToken);
                        for (int row = 0; row < _rowSize; row++)
                        {
                            for (int col = 0; col < _columnSize; col++)
                            {
                                if (_gameLayout[row, col].TokenType == fixTypeA)
                                {
                                    typeAList.Add(_gameLayout[row, col]);
                                }
                                else if (_gameLayout[row, col].TokenType == fixTypeB)
                                {
                                    typeBList.Add(_gameLayout[row, col]);
                                }
                            }
                        }
                        LLKTokenType tType = LLKHelper.GetRandomTokenType();
                        typeAList[rnd.Next(0, typeAList.Count)].TokenType = tType;
                        typeBList[rnd.Next(0, typeBList.Count)].TokenType = tType;
                        _isEileenPowerOn = false;
                        tokenSelectResult = TokenSelectResult.Matched;
                    }
                    else
                    {
                        tokenSelectResult = TokenSelectResult.Reset;
                    }
                }
            }
            // 连接阿草后技能点+1
            if (tokenSelectResult == TokenSelectResult.Matched)
            {
                if (currentTokenType == LLKTokenType.AS && currentTokenType == LLKTokenType.AS)
                {
                    SkillPoint++;
                    OnPropertyChanged(nameof(SkillPoint));
                }
            }
            return tokenSelectResult;
        }
        protected override int GetTotalScores()
        {
            int result = base.GetTotalScores();
            double skillPointBooster = Math.Ceiling(Math.Log2(SkillPoint));
            if (skillPointBooster > 1)
            {
                result = (int)(result * skillPointBooster);
            }
            return result;
        }
        private readonly object _skillLocker;
        private bool _isBellaPowerOn;
        private bool _isEileenPowerOn;
        private bool ActiveSkillHelper(LLKSkill skill)
        {
            if (SkillPoint <= 0)
            {
                return false;
            }
            bool activeResult = false;
            switch (skill)
            {
                case LLKSkill.None:
                    activeResult = true;
                    break;
                case LLKSkill.AvaPower:
                    activeResult = AvaPower();
                    break;
                case LLKSkill.BellaPower:
                    activeResult = BellaPower();
                    break;
                case LLKSkill.CarolPower:
                    activeResult = CarolPower();
                    break;
                case LLKSkill.DianaPower:
                    activeResult = DianaPower();
                    break;
                case LLKSkill.EileenPower:
                    activeResult = EileenPower();
                    break;
                default:
                    activeResult = false;
                    break;
            }
            OnPropertyChanged(nameof(SkillPoint));
            return activeResult;
        }
        private int GetSkillPoint()
        {
            int point = CurrentTokenTypes.Count * 10 / (_rowSize * _rowSize);
            if (CurrentTokenTypes.Count * 10 % (_rowSize * _rowSize) != 0)
            {
                point += 1;
            }
            return point;
        }
        private bool AvaPower()
        {
            if (SkillPoint < 3)
            {
                return false;
            }
            var rnd = new Random();
            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    int indexB = rnd.Next((row * _columnSize) + row, _rowSize * _columnSize);
                    int tRow = indexB / _columnSize;
                    int tCol = indexB % _columnSize;
                    LLKTokenType t = _gameLayout[row, col].TokenType;
                    _gameLayout[row, col].TokenType = _gameLayout[tRow, tCol].TokenType;
                    _gameLayout[tRow, tCol].TokenType = t;
                }
            }
            SkillPoint -= 3;
            return true;
        }
        private bool BellaPower()
        {
            if (SkillPoint < 2)
            {
                return false;
            }
            _isBellaPowerOn = true;
            SkillPoint -= 2;
            return true;
        }
        private bool CarolPower()
        {
            if (SkillPoint < 1)
            {
                return false;
            }
            // 根据当前剩余技能点随机获取技能
            // 最低10%，最高50%
            // 将skillPoint乘以1.5获得矫正点
            // skillPoint到这里不会为0，故余数为1-10，设余数为n
            // 从[0,n)中抽取一个数字的概率即为1/n
            int correctedPoint = (int)(SkillPoint * 1.5) % 11;
            bool canGetExtraPoint = new Random().Next(0, correctedPoint) == 0;
            if (canGetExtraPoint)
            {
                SkillPoint += 1;
            }
            else
            {
                SkillPoint -= 1;
            }
            return true;
        }
        private bool DianaPower()
        {
            if (SkillPoint < 1)
            {
                return false;
            }
            var rnd = new Random();
            var strarwberries = new List<LLKTokenType>() {
                        LLKTokenType.D1,
                        LLKTokenType.D2,
                        LLKTokenType.D3,
                        LLKTokenType.D4,
                        LLKTokenType.D5
                    };
            var strawberriesPos = new List<Coordinate>();
            for (int row = 0; row < _rowSize; row++)
            {
                for (int col = 0; col < _columnSize; col++)
                {
                    if (strarwberries.Contains(_gameLayout[row, col].TokenType))
                    {
                        strawberriesPos.Add(new Coordinate(row, col));
                    }
                }
            }
            while (strawberriesPos.Count > 0)
            {
                LLKTokenType current = strarwberries[rnd.Next(0, strarwberries.Count)];
                Coordinate a = strawberriesPos[rnd.Next(0, strawberriesPos.Count)];
                strawberriesPos.Remove(a);
                Coordinate b = strawberriesPos[rnd.Next(0, strawberriesPos.Count)];
                strawberriesPos.Remove(b);
                _gameLayout[a.Row, a.Column].TokenType = current;
                _gameLayout[b.Row, b.Column].TokenType = current;
            }
            SkillPoint -= 1;
            return true;
        }
        private bool EileenPower()
        {
            if (SkillPoint < 2)
            {
                return false;
            }
            SkillPoint -= 2;
            _isEileenPowerOn = true;
            return true;
        }
        #endregion
    }
}