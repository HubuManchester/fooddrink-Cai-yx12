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
            _recipeService.ToggleFavorite(recipeId);

            var updatedRecipe = _recipeService.GetAllRecipes().FirstOrDefault(r => r.Id == recipeId);
            if (updatedRecipe != null)
            {
                _recipe.IsFavorite = updatedRecipe.IsFavorite;
            }

            UpdateFavoriteButton();
            await DisplayAlert("Success", _recipe.IsFavorite ? "Added to favorites!" : "Removed from favorites", "OK");
        }
        catch (Exception ex)
        {
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

    private async void OnShareClicked(object? sender, EventArgs e)
    {
        try
        {
            var shareText = $"🍽️ {_recipe.Name}\n\n📝 Ingredients:\n";
            foreach (var ingredient in _recipe.Ingredients)
            {
                shareText += $"• {ingredient}\n";
            }
            shareText += $"\n👩‍🍳 Steps:\n";
            for (int i = 0; i < _recipe.Steps.Count; i++)
            {
                shareText += $"{i + 1}. {_recipe.Steps[i]}\n";
            }
            shareText += $"\n⏱️ Prep Time: {_recipe.PrepTime} minutes";

            await Share.Default.RequestAsync(new ShareTextRequest
            {
                Text = shareText,
                Title = $"Share {_recipe.Name}"
            });
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Share error: {ex.Message}");
            await DisplayAlert("Error", "Cannot share recipe", "OK");
        }
    }
}