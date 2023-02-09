using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Markup;
using Utility.Extensions;

namespace WorkTools.Infrastructure.Converters
{
    [ValueConversion(typeof(Enum), typeof(IEnumerable<ValueDescription>))]
    public class EnumToCollectionConverter : MarkupExtension, IValueConverter
    {
        //public bool IsTranslateRequired { get; set; } = false;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Enum.GetValues(value.GetType())
              .Cast<Enum>()
              .Select(e => new ValueDescription() { Value = e, Description = GetDescription(e) })
              .ToList();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }

        private string GetDescription(Enum @enum)
        {
            string description = @enum.GetDescription();
            //if (description.IsNullOrEmpty())
            //    return string.Empty;
            //if (IsTranslateRequired)
            //    return @enum.GetDescription();
            return description;
        }
    }


    public class ValueDescription
    {
        public Enum Value { get; set; }
        public string Description { get; set; }
    }
}
