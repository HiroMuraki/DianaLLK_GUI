using LianLianKan;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DianaLLK_GUI.ViewModel.ValueConverter
{
    [ValueConversion(typeof(TokenCategory), typeof(Uri))]
    public class GameThemeToBackground : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string path = (TokenCategory)value switch
                {
                    TokenCategory.Ava => "BG-1",
                    TokenCategory.Bella => "BG-2",
                    TokenCategory.Carol => "BG-3",
                    TokenCategory.Diana => "BG-4",
                    TokenCategory.Eileen => "BG-5",
                    _ => "BG.jpg",
                };
                ImageSource imageSource = App.GetImage(path, ImageType.Background)?.ImageSource;
                if (imageSource is not null)
                {
                    return imageSource;
                }
            }
            catch
            {
            }

            try
            {
                string path = (TokenCategory)value switch
                {
                    TokenCategory.Ava => "Resources/Images/Backgrounds/Background_AvaTheme.jpg",
                    TokenCategory.Bella => "Resources/Images/Backgrounds/Background_BellaTheme.jpg",
                    TokenCategory.Carol => "Resources/Images/Backgrounds/Background_CarolTheme.jpg",
                    TokenCategory.Diana => "Resources/Images/Backgrounds/Background_DianaTheme.jpg",
                    TokenCategory.Eileen => "Resources/Images/Backgrounds/Background_EileenTheme.jpg",
                    _ => "Resources/Images/Backgrounds/Background_ASTheme.jpg",
                };
                return new Uri(path, UriKind.Relative);
            }
            catch
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
