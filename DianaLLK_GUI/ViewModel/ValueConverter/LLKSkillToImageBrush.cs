//using MergeDiana.GameLib;
using LianLianKan;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DianaLLK_GUI.ViewModel.ValueConverter
{
    [ValueConversion(typeof(LLKSkill), typeof(ImageBrush))]
    public class LLKSkillToImageBrush : IValueConverter
    {
        public static LLKSkillToImageBrush Default { get; } = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (LLKSkill)value switch
            {
                LLKSkill.AvaPower => App.GetImage("AVA_SKILL", ImageType.SKill),
                LLKSkill.BellaPower => App.GetImage("BELLA_SKILL", ImageType.SKill),
                LLKSkill.CarolPower => App.GetImage("CAROL_SKILL", ImageType.SKill),
                LLKSkill.DianaPower => App.GetImage("DIANA_SKILL", ImageType.SKill),
                LLKSkill.EileenPower => App.GetImage("EILEEN_SKILL", ImageType.SKill),
                _ => App.GetImage("AVA_SKILL", ImageType.SKill),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
