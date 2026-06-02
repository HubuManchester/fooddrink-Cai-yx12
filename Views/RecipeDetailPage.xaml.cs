using RecipeRandomizer.Models;
using RecipeRandomizer.Services;

namespace RecipeRandomizer.Views;

public partial class RecipeDetailPage : ContentPage
{
    private Recipe _recipe;
    private RecipeService _recipeService;
    private ITextToSpeech? _textToSpeech;

    public RecipeDetailPage(Recipe recipe, RecipeService recipeService)
    {
        InitializeComponent();

        _recipe = recipe;
        _recipeService = recipeService;
        _textToSpeech = TextToSpeech.Default;

        RecipeNameLabel.Text = _recipe.Name;
        CategoryLabel.Text = _recipe.Category;
        PrepTimeLabel.Text = $"{_recipe.PrepTime} min";

        IngredientsCollectionView.ItemsSource = _recipe.Ingredients;
        StepsCollectionView.ItemsSource = _recipe.Steps;

        UpdateFavoriteButton();
    }

    private void UpdateFavoriteButton()
    {
        if (_recipe.IsFavorite)
        {
            FavoriteButton.Text = "⭐ Remove from Favorites";
            FavoriteButton.BackgroundColor = Colors.Orange;
        }
        else
        {
            FavoriteButton.Text = "☆ Add to Favorites";
            FavoriteButton.BackgroundColor = Colors.Gray;
        }
    }

    private async void OnFavoriteClicked(object? sender, EventArgs e)
    {
        try
        {
            var recipeId = _recipe.Id;
            System.Diagnostics.Debug.WriteLine($"=== Favorite clicked for recipe ID: {recipeId}, Name: {_recipe.Name}");

            _recipeService.ToggleFavorite(recipeId);

            // 重新获取当前菜谱的最新状态
            var updatedRecipe = _recipeService.GetAllRecipes().FirstOrDefault(r => r.Id == recipeId);
            if (updatedRecipe != null)
            {
                _recipe.IsFavorite = updatedRecipe.IsFavorite;
            }

            System.Diagnostics.Debug.WriteLine($"=== After toggle, IsFavorite: {_recipe.IsFavorite}");

            UpdateFavoriteButton();

            // 显示提示确认操作
            await DisplayAlert("Success", _recipe.IsFavorite ? "Added to favorites!" : "Removed from favorites", "OK");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"=== ERROR in OnFavoriteClicked: {ex.Message}");
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnSpeakClicked(object? sender, EventArgs e)
    {
        if (_textToSpeech == null)
        {
            await DisplayAlert("Not Available", "Text-to-speech is not supported on this device.", "OK");
            return;
        }

        try
        {
            var stepsText = string.Join(". ", _recipe.Steps);
            await _textToSpeech.SpeakAsync($"Recipe for {_recipe.Name}. {stepsText}");
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Not Supported", "Text-to-speech is not supported on this device.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Cannot read aloud: {ex.Message}", "OK");
        }
    }

    private async void OnCameraClicked(object? sender, EventArgs e)
    {
        var photoPath = await CameraService.TakePhotoAsync();

        if (photoPath != null)
        {
            await DisplayAlert("Success", "Photo taken and saved!", "OK");
        }
        else
        {
            await DisplayAlert("Error", "Failed to take photo. Please grant camera permission.", "OK");
        }
    }
}