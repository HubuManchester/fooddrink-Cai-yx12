using RecipeRandomizer.Models;
using RecipeRandomizer.Services;

namespace RecipeRandomizer.Views;

public partial class FavoritesPage : ContentPage
{
    private List<Recipe> _favorites;
    private RecipeService _recipeService;

    public FavoritesPage(List<Recipe> favorites, RecipeService recipeService)
    {
        InitializeComponent();

        _favorites = favorites;
        _recipeService = recipeService;

        LoadFavorites();
        FavoritesCollectionView.SelectionChanged += OnFavoriteSelected;
    }

    private void LoadFavorites()
    {
        if (_favorites.Any())
        {
            FavoritesCollectionView.ItemsSource = _favorites;
            EmptyLabel.IsVisible = false;
        }
        else
        {
            EmptyLabel.IsVisible = true;
        }
    }

    private async void OnFavoriteSelected(object? sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is Recipe selectedRecipe)
        {
            FavoritesCollectionView.SelectedItem = null;
            await Navigation.PushAsync(new RecipeDetailPage(selectedRecipe, _recipeService));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _favorites = _recipeService.GetFavorites();
        LoadFavorites();
    }
}