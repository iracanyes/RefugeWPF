using RefugeWPF.CoucheMetiers.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace RefugeWPF.CouchePresentation.Converter
{
    class ContactRolesToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<ContactRole> contactRoles)
            {
                return string.Join(", ", contactRoles.Select(cr => cr.Role.Name));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
