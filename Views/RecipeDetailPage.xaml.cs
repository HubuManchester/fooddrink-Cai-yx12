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

        // Display recipe details immediately
        RecipeNameLabel.Text = _recipe.Name;
        CategoryLabel.Text = _recipe.Category;
        PrepTimeLabel.Text = $"{_recipe.PrepTime} min";

        // Load collections
        IngredientsCollectionView.ItemsSource = _recipe.Ingredients;
        StepsCollectionView.ItemsSource = _recipe.Steps;

        UpdateFavoriteButton();

        // Bind events
        FavoriteButton.Clicked += OnFavoriteClicked;
        SpeakButton.Clicked += OnSpeakClicked;
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

    private void OnFavoriteClicked(object? sender, EventArgs e)
    {
        _recipeService.ToggleFavorite(_recipe.Id);
        UpdateFavoriteButton();
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
}