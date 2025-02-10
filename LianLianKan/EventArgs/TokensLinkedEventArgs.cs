using System;

namespace LianLianKan
{
    public class TokensLinkedEventArgs : EventArgs
    {
        public TokensLinkedEventArgs(LLKToken first, LLKToken second)
        {
            First = first;
            Second = second;
        }

        public LLKToken First { get; }

        public LLKToken Second { get; }
    }
}