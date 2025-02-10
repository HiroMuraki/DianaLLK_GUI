using System;

namespace LianLianKan
{
    public class GameCompletedEventArgs : EventArgs
    {
        public GameCompletedEventArgs(int totalScores, int tokenAmount, int rowSize, int columnSize, GameType gameType)
        {
            TotalScores = totalScores;
            TokenAmount = tokenAmount;
            RowSize = rowSize;
            ColumnSize = columnSize;
            GameType = gameType;
        }
        
        public int TotalScores { get; }

        public int TokenAmount { get; }

        public int ColumnSize { get; }

        public int RowSize { get; }

        public GameType GameType { get; }
    }
}
