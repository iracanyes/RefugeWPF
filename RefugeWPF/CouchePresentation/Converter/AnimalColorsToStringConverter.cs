using RefugeWPF.ClassesMetiers.Model.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace RefugeWPF.CouchePresentation.Converter
{
    class AnimalColorsToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<AnimalColor> animalColors)
            {
                return string.Join(", ", animalColors.Select(ac => ac.Color.Name));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
