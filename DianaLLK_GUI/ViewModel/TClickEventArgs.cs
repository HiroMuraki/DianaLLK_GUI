using LianLianKan;
using System;

namespace DianaLLK_GUI
{
    public class TClickEventArgs : EventArgs
    {
        public LLKToken Token { get; }

        public TClickEventArgs(LLKToken token)
        {
            Token = token;
        }
    }
}
