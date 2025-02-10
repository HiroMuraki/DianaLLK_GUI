using System;
using System.Collections.Generic;

namespace LianLianKan
{
    public static class LLKHelper
    {
        static LLKHelper()
        {
            _rnd = new Random();
            _allTokenTypes = Enum.GetValues(typeof(LLKTokenType));
            _allTokenCategory = Enum.GetValues(typeof(TokenCategory));
            TokenResources = new Dictionary<LLKTokenType, string>()
            {
                [LLKTokenType.None] = null,

                [LLKTokenType.AS] = "AS",

                [LLKTokenType.A1] = "A1",
                [LLKTokenType.A2] = "A2",
                [LLKTokenType.A3] = "A3",
                [LLKTokenType.A4] = "A4",
                [LLKTokenType.A5] = "A5",

                [LLKTokenType.B1] = "B1",
                [LLKTokenType.B2] = "B2",
                [LLKTokenType.B3] = "B3",
                [LLKTokenType.B4] = "B4",
                [LLKTokenType.B5] = "B5",

                [LLKTokenType.C1] = "C1",
                [LLKTokenType.C2] = "C2",
                [LLKTokenType.C3] = "C3",
                [LLKTokenType.C4] = "C4",
                [LLKTokenType.C5] = "C5",

                [LLKTokenType.D1] = "D1",
                [LLKTokenType.D2] = "D2",
                [LLKTokenType.D3] = "D3",
                [LLKTokenType.D4] = "D4",
                [LLKTokenType.D5] = "D5",

                [LLKTokenType.E1] = "E1",
                [LLKTokenType.E2] = "E2",
                [LLKTokenType.E3] = "E3",
                [LLKTokenType.E4] = "E4",
                [LLKTokenType.E5] = "E5"
            };
            TokenCategoryThemes = new Dictionary<TokenCategory, string>()
            {
                [TokenCategory.None] = "",
                [TokenCategory.AS] = "ASTheme",
                [TokenCategory.Ava] = "AvaTheme",
                [TokenCategory.Bella] = "BellaTheme",
                [TokenCategory.Carol] = "CarolTheme",
                [TokenCategory.Diana] = "DianaTheme",
                [TokenCategory.Eileen] = "EileenTheme"
            };
            NumTokenTypes = _allTokenTypes.Length - 1;
            NumTokenCategory = _allTokenCategory.Length - 1;
        }

        public static readonly Dictionary<LLKTokenType, string> TokenResources;

        public static readonly Dictionary<TokenCategory, string> TokenCategoryThemes;

        public static readonly int NumTokenTypes;

        public static readonly int NumTokenCategory;

        public static GameRestorePack GenerateLayoutFrom(object obj)
        {
            return GameRestorePack.GenerateGameInfoFrom(obj);
        }

        public static string ConvertLayoutFrom(LLKTokenType[,] tokenTypes, int numTokenTypes, int skillPoint)
        {
            return GameRestorePack.GetGameInfoFrom(tokenTypes, numTokenTypes, skillPoint);
        }

        public static string ConvertLayoutFrom(IEnumerable<LLKTokenType> tokenTypes, int rowSize, int columnSize, int numTokenTypes, int skillPoint)
        {
            return GameRestorePack.GetGameInfoFrom(tokenTypes, rowSize, columnSize, numTokenTypes, skillPoint);
        }

        public static LLKTokenType GetRandomTokenType()
        {
            return (LLKTokenType)_allTokenTypes.GetValue(_rnd.Next(1, _allTokenTypes.Length));
        }

        public static TokenCategory GetRandomTokenCategory()
        {
            return (TokenCategory)_allTokenCategory.GetValue(_rnd.Next(1, _allTokenCategory.Length));
        }

        public static TokenCategory GetTokenCategoryFromTokenType(LLKTokenType tokenType)
        {
            switch (tokenType)
            {
                case LLKTokenType.None:
                    return TokenCategory.None;
                case LLKTokenType.AS:
                    return TokenCategory.AS;
                case LLKTokenType.A1:
                case LLKTokenType.A2:
                case LLKTokenType.A3:
                case LLKTokenType.A4:
                case LLKTokenType.A5:
                    return TokenCategory.Ava;
                case LLKTokenType.B1:
                case LLKTokenType.B2:
                case LLKTokenType.B3:
                case LLKTokenType.B4:
                case LLKTokenType.B5:
                    return TokenCategory.Bella;
                case LLKTokenType.C1:
                case LLKTokenType.C2:
                case LLKTokenType.C3:
                case LLKTokenType.C4:
                case LLKTokenType.C5:
                    return TokenCategory.Carol;
                case LLKTokenType.D1:
                case LLKTokenType.D2:
                case LLKTokenType.D3:
                case LLKTokenType.D4:
                case LLKTokenType.D5:
                    return TokenCategory.Diana;
                case LLKTokenType.E1:
                case LLKTokenType.E2:
                case LLKTokenType.E3:
                case LLKTokenType.E4:
                case LLKTokenType.E5:
                    return TokenCategory.Eileen;
                default:
                    return TokenCategory.None;
            }
        }

        public static string GetSkillDescription(LLKSkill skill)
        {
            return skill switch
            {
                LLKSkill.None => "",
                LLKSkill.AvaPower => "AVAVA!",
                LLKSkill.BellaPower => "击穿月球!",
                LLKSkill.CarolPower => "R I S E",
                LLKSkill.DianaPower => "多态小草莓",
                LLKSkill.EileenPower => "团队粘合",
                _ => "",
            };
        }

        #region NonPublic
        private static readonly Array _allTokenTypes;
        private static readonly Array _allTokenCategory;
        private static readonly Random _rnd;
        #endregion
    }
}
