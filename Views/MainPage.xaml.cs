using RecipeRandomizer.Models;
using RecipeRandomizer.Services;
using RecipeRandomizer.Views;

namespace RecipeRandomizer.Views;

public partial class MainPage : ContentPage
{
    private RecipeService _recipeService = null!;
    private List<Recipe> _allRecipes = null!;
    private List<Recipe> _displayedRecipes = null!;
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
        SettingsButton.Clicked += OnSettingsClicked;
    }

    public void RefreshFonts()
    {
        SearchBar.FontSize = AccessibilityService.ScaleFontSize(14);
        FilterButton.FontSize = AccessibilityService.ScaleFontSize(13);
        RandomButton.FontSize = AccessibilityService.ScaleFontSize(15);
        FavoritesButton.FontSize = AccessibilityService.ScaleFontSize(15);
        CategoryPicker.FontSize = AccessibilityService.ScaleFontSize(12);

        ApplyTheme();
        RefreshRecipeList();
    }

    public void ApplyTheme()
    {
        if (AccessibilityService.IsDarkTheme)
        {
            this.BackgroundColor = Color.FromArgb("#1E1E1E");
            SearchBar.BackgroundColor = Color.FromArgb("#333333");
            SearchBar.TextColor = Colors.White;
            CategoryPicker.BackgroundColor = Color.FromArgb("#333333");
            CategoryPicker.TextColor = Colors.White;
        }
        else
        {
            this.BackgroundColor = Color.FromArgb("#F5F5F5");
            SearchBar.BackgroundColor = Colors.White;
            SearchBar.TextColor = Colors.Black;
            CategoryPicker.BackgroundColor = Colors.White;
            CategoryPicker.TextColor = Colors.Black;
        }
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

    private async void OnSettingsClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new SettingsPage());
    }

    private async void OnRecipeTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Recipe selectedRecipe)
        {
            var recipeId = selectedRecipe.Id;
            var fullRecipe = _recipeService.GetAllRecipes().FirstOrDefault(r => r.Id == recipeId);
            if (fullRecipe != null)
            {
                await Navigation.PushAsync(new RecipeDetailPage(fullRecipe, _recipeService));
            }
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
        RefreshFonts();
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