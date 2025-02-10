using LianLianKan;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DianaLLK_GUI.ViewModel.ValueConverter
{
    [ValueConversion(typeof(TokenCategory), typeof(Brush))]
    public class GameThemeToColor : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tokenCategory = (TokenCategory)value;
                return App.GetColor(LLKHelper.TokenCategoryThemes[tokenCategory]);

            }
            catch (Exception)
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
