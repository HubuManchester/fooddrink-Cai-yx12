using System.Globalization;

namespace RecipeRandomizer.Converters
{
    public class FavoriteConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool isFavorite && isFavorite)
                return "⭐";
            return "☆";
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}