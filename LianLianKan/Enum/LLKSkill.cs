using System;

namespace LianLianKan
{
    [Flags]
    public enum LLKSkill
    {
        None = 0b0000_0000,
        AvaPower = 0b0000_0001,
        BellaPower = 0b0000_0010,
        CarolPower = 0b0000_0100,
        DianaPower = 0b0000_1000,
        EileenPower = 0b0001_0000
    }
}
