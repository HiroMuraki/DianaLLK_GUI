using System;

namespace LianLianKan
{
    public class TokenMatchedEventArgs : EventArgs
    {
        public TokenMatchedEventArgs(LLKTokenType matchedTokenType)
        {
            TokenType = matchedTokenType;
        }

        public LLKTokenType TokenType { get; }
    }
}