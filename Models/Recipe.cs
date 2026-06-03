using RecipeRandomizer.Services;
using System.Text.Json.Serialization;

namespace RecipeRandomizer.Models
{
    public class Recipe
    {
        [JsonPropertyName("id")]
        public string? ApiId { get; set; }

        [JsonIgnore]
        public int Id
        {
            get => int.TryParse(ApiId, out int result) ? result : 0;
            set => ApiId = value.ToString();
        }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;

        [JsonIgnore]
        public string ImageUrl { get; set; } = string.Empty;

        [JsonPropertyName("ingredients")]
        public List<string> Ingredients { get; set; } = new List<string>();

        [JsonPropertyName("steps")]
        public List<string> Steps { get; set; } = new List<string>();

        [JsonPropertyName("prepTime")]
        public int PrepTime { get; set; }

        [JsonPropertyName("isFavorite")]
        public bool IsFavorite { get; set; }

        [JsonIgnore]
        public double NameFontSize => AccessibilityService.ScaleFontSize(15);

        [JsonIgnore]
        public double CategoryFontSize => AccessibilityService.ScaleFontSize(11);

        [JsonIgnore]
        public double TimeFontSize => AccessibilityService.ScaleFontSize(11);

        [JsonIgnore]
        public Color ItemBackgroundColor => AccessibilityService.IsDarkTheme ? Color.FromArgb("#2D2D2D") : Colors.White;

        [JsonIgnore]
        public Color ItemTextColor => AccessibilityService.IsDarkTheme ? Colors.White : Colors.Black;

        [JsonIgnore]
        public Color CategoryTextColor => AccessibilityService.IsDarkTheme ? Colors.LightGray : Colors.Gray;
    }
}