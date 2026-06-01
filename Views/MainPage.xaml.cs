using RecipeRandomizer.Models;
using RecipeRandomizer.Services;
using RecipeRandomizer.Views;

namespace RecipeRandomizer.Views;

public partial class MainPage : ContentPage
{
    private RecipeService _recipeService;
    private List<Recipe> _allRecipes = new();
    private List<Recipe> _displayedRecipes = new();
    private string _currentCategory = "All";
    private string _currentSearchKeyword = "";

    private IAccelerometer? _accelerometer;
    private ITextToSpeech? _textToSpeech;
    private bool _isHardwareInitialized = false;

    public MainPage()
    {
        InitializeComponent();

        _recipeService = new RecipeService();
        _allRecipes = _recipeService.GetAllRecipes();
        _displayedRecipes = new List<Recipe>(_allRecipes);

        LoadCategories();
        RefreshRecipeList();

        SearchBar.TextChanged += OnSearchTextChanged;
        FilterButton.Clicked += OnFilterClicked;
        RandomButton.Clicked += OnRandomClicked;
        FavoritesButton.Clicked += OnFavoritesClicked;
    }

    private void InitializeHardware()
    {
        if (_isHardwareInitialized) return;

        _accelerometer = Accelerometer.Default;
        _textToSpeech = TextToSpeech.Default;

        if (_accelerometer.IsSupported)
        {
            _accelerometer.ShakeDetected += OnShakeDetected;
        }

        _isHardwareInitialized = true;
    }

    private async void OnShakeDetected(object? sender, EventArgs e)
    {
        if (HapticFeedback.Default.IsSupported)
        {
            HapticFeedback.Default.Perform(HapticFeedbackType.Click);
        }
        await GetRandomRecipeAndNavigate();
    }

    private void LoadCategories()
    {
        var categories = _recipeService.GetCategories();
        CategoryPicker.ItemsSource = categories;
        CategoryPicker.SelectedIndex = 0;
    }

    private void RefreshRecipeList()
    {
        RecipesCollectionView.ItemsSource = null;
        RecipesCollectionView.ItemsSource = _displayedRecipes;
    }

    private void ApplyFilterAndSearch()
    {
        var filtered = _recipeService.GetRecipesByCategory(_currentCategory);

        if (!string.IsNullOrEmpty(_currentSearchKeyword))
        {
            filtered = filtered.Where(r =>
                r.Name.Contains(_currentSearchKeyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        _displayedRecipes = filtered;
        RefreshRecipeList();
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        _currentSearchKeyword = e.NewTextValue ?? "";
        ApplyFilterAndSearch();
    }

    private void OnFilterClicked(object? sender, EventArgs e)
    {
        if (CategoryPicker.SelectedItem != null)
        {
            _currentCategory = CategoryPicker.SelectedItem.ToString() ?? "All";
            ApplyFilterAndSearch();
        }
    }

    private async void OnRandomClicked(object? sender, EventArgs e)
    {
        await GetRandomRecipeAndNavigate();
    }

    private async Task GetRandomRecipeAndNavigate()
    {
        var randomRecipe = _recipeService.GetRandomRecipe();

        if (_textToSpeech != null)
        {
            try
            {
                await _textToSpeech.SpeakAsync($"Random recipe: {randomRecipe.Name}");
            }
            catch (Exception) { }
        }

        await Navigation.PushAsync(new RecipeDetailPage(randomRecipe, _recipeService));
    }

    private async void OnFavoritesClicked(object? sender, EventArgs e)
    {
        var favorites = _recipeService.GetFavorites();
        await Navigation.PushAsync(new FavoritesPage(favorites, _recipeService));
    }

    /// <summary>
    /// Handle recipe tap - THIS IS THE NEW METHOD
    /// </summary>
    private async void OnRecipeTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Recipe selectedRecipe)
        {
            await Navigation.PushAsync(new RecipeDetailPage(selectedRecipe, _recipeService));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        InitializeHardware();

        if (_accelerometer != null && _accelerometer.IsSupported && !_accelerometer.IsMonitoring)
        {
            _accelerometer.Start(SensorSpeed.UI);
        }

        ApplyFilterAndSearch();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        if (_accelerometer != null && _accelerometer.IsSupported && _accelerometer.IsMonitoring)
        {
            _accelerometer.Stop();
        }
    }
}