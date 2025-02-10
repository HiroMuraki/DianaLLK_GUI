using System;

namespace LianLianKan
{
    public class TokenSelectedEventArgs : EventArgs
    {
        public TokenSelectedEventArgs(LLKTokenType tokenType)
        {
            MyProperty = tokenType;
        }

        public LLKTokenType MyProperty { get; }
    }
}
