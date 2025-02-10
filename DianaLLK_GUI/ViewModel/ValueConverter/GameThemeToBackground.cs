using LianLianKan;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DianaLLK_GUI.ViewModel.ValueConverter
{
    [ValueConversion(typeof(TokenCategory), typeof(Uri))]
    public class GameThemeToBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string path;
            switch ((TokenCategory)value)
            {
                case TokenCategory.None:
                    path = "Resources/Images/Backgrounds/Background_ASTheme.jpg";
                    break;
                case TokenCategory.AS:
                    path = "Resources/Images/Backgrounds/Background_ASTheme.jpg";
                    break;
                case TokenCategory.Ava:
                    path = "Resources/Images/Backgrounds/Background_AvaTheme.jpg";
                    break;
                case TokenCategory.Bella:
                    path = "Resources/Images/Backgrounds/Background_BellaTheme.jpg";
                    break;
                case TokenCategory.Carol:
                    path = "Resources/Images/Backgrounds/Background_CarolTheme.jpg";
                    break;
                case TokenCategory.Diana:
                    path = "Resources/Images/Backgrounds/Background_DianaTheme.jpg";
                    break;
                case TokenCategory.Eileen:
                    path = "Resources/Images/Backgrounds/Background_EileenTheme.jpg";
                    break;
                default:
                    path = "Resources/Images/Backgrounds/Background_ASTheme.jpg";
                    break;
            }
            return new Uri(path, UriKind.Relative);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
