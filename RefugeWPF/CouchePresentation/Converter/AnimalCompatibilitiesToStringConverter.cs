using RefugeWPF.CoucheMetiers.Model.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace RefugeWPF.CouchePresentation.Converter
{
    class AnimalCompatibilitiesToStringConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<AnimalCompatibility> animalCompatibilities)
            {
                //Debug.WriteLine(string.Join(" || ", animalCompatibilities.Select(ac => $"{ac.Compatibility.Type} ({ac.Value}) : {ac.Description}")));
                return string.Join("\n", animalCompatibilities.Select(ac => $"{ac.Compatibility.Type} ({ac.Value}) : {ac.Description}"));
            }

            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        
    }
}
