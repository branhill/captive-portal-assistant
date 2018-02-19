using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;
using CaptivePortalAssistant.Models;

namespace CaptivePortalAssistant.Helpers
{
    public class EnumToBoolConverter<TEnum> : IValueConverter where TEnum : struct
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return Enum.Parse(typeof(TEnum), (string)parameter).Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (bool)value
                ? Enum.Parse(typeof(TEnum), (string)parameter)
                : DependencyProperty.UnsetValue;
        }
    }

    public class DetectionMethodToBoolConverter : EnumToBoolConverter<DetectionMethod> { }

    public class AutomationOptionToBoolConverter : EnumToBoolConverter<AutomationOption> { }

}
