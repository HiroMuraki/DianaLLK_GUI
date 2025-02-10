namespace LianLianKan
{
    public interface IGameRestore
    {
        /// <summary>
        /// 表示token布局的二维矩阵
        /// </summary>
        LLKTokenType[,] TokenTypes { get; }

        /// <summary>
        /// 技能点数
        /// </summary>
        int SkillPoint { get; }

        /// <summary>
        /// 列数
        /// </summary>
        int ColumnSize { get; }

        /// <summary>
        /// 行数
        /// </summary>
        int RowSize { get; }

        /// <summary>
        /// token种类数
        /// </summary>
        int TokenAmount { get; }
    }
}
