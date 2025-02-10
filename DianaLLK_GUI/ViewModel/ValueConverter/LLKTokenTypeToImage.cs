﻿//using MergeDiana.GameLib;
using LianLianKan;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace DianaLLK_GUI.ViewModel.ValueConverter
{
    [ValueConversion(typeof(LLKTokenType), typeof(Brush))]
    public class LLKTokenTypeToImage : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var tokenType = (LLKTokenType)value;
                return App.GetImage(LLKHelper.TokenResources[tokenType]);
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
