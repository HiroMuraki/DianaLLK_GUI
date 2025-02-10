using System;

namespace LianLianKan
{
    public class TokenMatchedEventArgs : EventArgs
    {
        public TokenMatchedEventArgs(LLKTokenType matchedTokenType, bool matched)
        {
            TokenType = matchedTokenType;
            Sucess = matched;
        }

        public LLKTokenType TokenType { get; }

        public bool Sucess { get; }
    }
}