using System;

namespace LianLianKan
{
    public class TokensLinkedEventArgs : EventArgs
    {
        public TokensLinkedEventArgs(LLKToken first, LLKToken second, Coordinate[] nodes)
        {
            First = first;
            Second = second;
            Nodes = nodes;
        }

        public LLKToken First { get; }

        public LLKToken Second { get; }

        public Coordinate[] Nodes { get; }
    }
}