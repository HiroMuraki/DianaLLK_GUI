using System;
using System.Threading.Tasks;
using CurrentTokenTypes = System.Collections.Generic.List<LianLianKan.LLKTokenType>;
using LLKTokens = System.Collections.Generic.IEnumerable<LianLianKan.LLKToken>;
using LLKTokenTypes = System.Collections.Generic.IEnumerable<LianLianKan.LLKTokenType>;

namespace LianLianKan
{
    public interface IGame
    {
        /// <summary>
        /// 游戏完成
        /// </summary>
        event EventHandler<GameCompletedEventArgs> GameCompleted;

        /// <summary>
        /// 布局信息被更改
        /// </summary>
        event EventHandler<LayoutResetedEventArgs> LayoutReseted;

        /// <summary>
        /// 技能启用
        /// </summary>
        event EventHandler<SkillActivedEventArgs> SkillActived;

        /// <summary>
        /// 当前的Token类型
        /// </summary>
        CurrentTokenTypes CurrentTokenTypes { get; }

        /// <summary>
        /// 当前token序列
        /// </summary>
        LLKTokens LLKTokenArray { get; }

        /// <summary>
        /// 当前token类型序列
        /// </summary>
        LLKTokenTypes TokenTypeArray { get; }

        /// <summary>
        /// 行数
        /// </summary>
        int RowSize { get; }

        /// <summary>
        /// 列数
        /// </summary>
        int ColumnSize { get; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        GameType GameType { get; }

        /// <summary>
        /// 技能点
        /// </summary>
        int SkillPoint { get; }

        /// <summary>
        /// 开始游戏
        /// </summary>
        /// <param name="rowSize">行数</param>
        /// <param name="columnSize">列数</param>
        /// <param name="tokenAmount">token类型数</param>
        void StartGame(int rowSize, int columnSize, int tokenAmount);

        Task StartGameAsync(int rowSize, int columnSize, int tokenAmount);

        /// <summary>
        /// 恢复游戏
        /// </summary>
        /// <param name="restorePack">恢复信息</param>
        void RestoreGame(IGameRestore restorePack);

        Task RestoreGameAsync(IGameRestore restorePack);

        /// <summary>
        /// 选择token
        /// </summary>
        /// <param name="token">要选择的token</param>
        /// <returns>选择结果</returns>
        TokenSelectResult SelectToken(LLKToken token);

        Task<TokenSelectResult> SelectTokenAsync(LLKToken token);

        /// <summary>
        /// 启用技能
        /// </summary>
        /// <param name="skill">技能类型</param>
        void ActiveSkill(LLKSkill skill);

        Task ActiveSkillAsync(LLKSkill skill);
    }
}
