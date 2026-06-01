using RecipeRandomizer.Services;

namespace RecipeRandomizer.Models
{
    public class Recipe
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new List<string>();
        public List<string> Steps { get; set; } = new List<string>();
        public int PrepTime { get; set; }
        public bool IsFavorite { get; set; }

        // 动态字体大小
        public double NameFontSize => AccessibilityService.ScaleFontSize(15);
        public double CategoryFontSize => AccessibilityService.ScaleFontSize(11);
        public double TimeFontSize => AccessibilityService.ScaleFontSize(11);

        // 动态颜色（深色模式支持）
        public Color ItemBackgroundColor => AccessibilityService.IsDarkTheme ? Color.FromArgb("#2D2D2D") : Colors.White;
        public Color ItemTextColor => AccessibilityService.IsDarkTheme ? Colors.White : Colors.Black;
        public Color CategoryTextColor => AccessibilityService.IsDarkTheme ? Colors.LightGray : Colors.Gray;
    }
}