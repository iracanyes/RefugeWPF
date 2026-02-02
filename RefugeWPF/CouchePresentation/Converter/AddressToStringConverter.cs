using RefugeWPF.CoucheMetiers.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace RefugeWPF.CouchePresentation.Converter
{
    class AddressToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is Address address)
            {
                return $"{address.Street}, {address.City}, {address.ZipCode} {address.State}, {address.Country}";
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
