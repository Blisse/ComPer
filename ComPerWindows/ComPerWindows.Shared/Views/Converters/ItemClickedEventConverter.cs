﻿using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HushWindowsPhoneLibrary.Views.Converters
{
    public class ItemClickedEventConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var args = value as ItemClickEventArgs;

            if (args != null)
            {
                return args.ClickedItem;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
