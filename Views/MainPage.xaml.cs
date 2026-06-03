using System.ComponentModel;
using Microsoft.Maui.Networking;
using RecipeRandomizer.Models;
using RecipeRandomizer.Services;
using RecipeRandomizer.Views;

namespace RecipeRandomizer.Views;

public partial class MainPage : ContentPage, INotifyPropertyChanged
{
    private RecipeService _recipeService = null!;
    private List<Recipe> _allRecipes = null!;
    private List<Recipe> _displayedRecipes = null!;
    private string _currentCategory = "All";
    private string _currentSearchKeyword = "";
    private Recipe _dailyRecommendation = null!;

    private IAccelerometer? _accelerometer;
    private ITextToSpeech? _textToSpeech;
    private bool _isHardwareInitialized = false;
    private bool _isNavigating = false;
    private bool _justReturned = false;

    private bool _isRefreshing = false;

    public bool IsRefreshing
    {
        get => _isRefreshing;
        set
        {
            _isRefreshing = value;
            OnPropertyChanged(nameof(IsRefreshing));
        }
    }

    private double _lastX = 0, _lastY = 0, _lastZ = 0;
    private DateTime _lastShakeTime = DateTime.MinValue;
    private const double ShakeThreshold = 3.0;
    private const int ShakeInterval = 1500;

    public MainPage()
    {
        InitializeComponent();
        BindingContext = this;

        _recipeService = new RecipeService();
        _allRecipes = _recipeService.GetAllRecipes();
        _displayedRecipes = new List<Recipe>(_allRecipes);

        LoadCategories();
        LoadDailyRecommendation();
        RefreshRecipeList();

        SearchBar.TextChanged += OnSearchTextChanged;
        FilterButton.Clicked += OnFilterClicked;
        RandomButton.Clicked += OnRandomClicked;
        FavoritesButton.Clicked += OnFavoritesClicked;
        SettingsButton.Clicked += OnSettingsClicked;
        MapButton.Clicked += OnMapClicked;

        RefreshView.Command = new Command(async () => await RefreshData());
    }

    private void LoadDailyRecommendation()
    {
        var allRecipes = _recipeService.GetAllRecipes();
        if (allRecipes != null && allRecipes.Count > 0)
        {
            var todaySeed = DateTime.Now.DayOfYear;
            var random = new Random(todaySeed);
            _dailyRecommendation = allRecipes[random.Next(allRecipes.Count)];
            DailyRecommendationLabel.Text = _dailyRecommendation.Name;
        }
    }

    public void RefreshFonts()
    {
        System.Diagnostics.Debug.WriteLine("RefreshFonts called");

        SearchBar.FontSize = AccessibilityService.ScaleFontSize(14);
        FilterButton.FontSize = AccessibilityService.ScaleFontSize(13);
        RandomButton.FontSize = AccessibilityService.ScaleFontSize(15);
        FavoritesButton.FontSize = AccessibilityService.ScaleFontSize(15);
        CategoryPicker.FontSize = AccessibilityService.ScaleFontSize(12);

        var currentList = _displayedRecipes.ToList();
        RecipesCollectionView.ItemsSource = null;
        RecipesCollectionView.ItemsSource = currentList;

        ApplyTheme();
    }

    public void ApplyTheme()
    {
        System.Diagnostics.Debug.WriteLine($"ApplyTheme called, IsDarkTheme: {AccessibilityService.IsDarkTheme}");

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

    private async Task RefreshData()
    {
        if (IsRefreshing) return;

        IsRefreshing = true;

        try
        {
            System.Diagnostics.Debug.WriteLine("=== RefreshData started ===");

            await _recipeService.ForceRefreshRemoteData();

            _allRecipes = _recipeService.GetAllRecipes();
            _displayedRecipes = new List<Recipe>(_allRecipes);

            System.Diagnostics.Debug.WriteLine($"=== After refresh, total recipes: {_allRecipes?.Count}");

            ApplyFilterAndSearch();
            LoadDailyRecommendation();

            System.Diagnostics.Debug.WriteLine("=== RefreshData completed ===");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Refresh error: {ex.Message}");
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    private async void OnMapClicked(object? sender, EventArgs e)
    {
        await Navigation.PushAsync(new MapPage());
    }

    private void InitializeHardware()
    {
        if (_isHardwareInitialized) return;

        try
        {
            _accelerometer = Accelerometer.Default;
            _textToSpeech = TextToSpeech.Default;

            if (_accelerometer != null && _accelerometer.IsSupported)
            {
                _accelerometer.ReadingChanged += OnAccelerometerReadingChanged;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Hardware init error: {ex.Message}");
        }

        _isHardwareInitialized = true;
    }

    private void OnAccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
    {
        if (_isNavigating || _justReturned) return;

        try
        {
            if (e?.Reading == null) return;

            var reading = e.Reading;
            double x = reading.Acceleration.X;
            double y = reading.Acceleration.Y;
            double z = reading.Acceleration.Z;

            double deltaX = Math.Abs(x - _lastX);
            double deltaY = Math.Abs(y - _lastY);
            double deltaZ = Math.Abs(z - _lastZ);
            double totalDelta = deltaX + deltaY + deltaZ;

            _lastX = x;
            _lastY = y;
            _lastZ = z;

            if (totalDelta > ShakeThreshold)
            {
                var now = DateTime.Now;
                if ((now - _lastShakeTime).TotalMilliseconds > ShakeInterval)
                {
                    _lastShakeTime = now;

                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            if (HapticFeedback.Default.IsSupported)
                            {
                                HapticFeedback.Default.Perform(HapticFeedbackType.Click);
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Vibration error: {ex.Message}");
                        }

                        await GetRandomRecipeAndNavigate();
                    });
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Accelerometer error: {ex.Message}");
        }
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
        if (_isNavigating) return;
        _isNavigating = true;

        try
        {
            if (_accelerometer != null && _accelerometer.IsSupported && _accelerometer.IsMonitoring)
            {
                _accelerometer.Stop();
            }

            var randomRecipe = _recipeService.GetRandomRecipe();

            if (randomRecipe == null)
            {
                _isNavigating = false;
                return;
            }

            if (_textToSpeech != null)
            {
                try
                {
                    await _textToSpeech.SpeakAsync($"Random recipe: {randomRecipe.Name}");
                }
                catch (Exception ttsEx)
                {
                    System.Diagnostics.Debug.WriteLine($"TTS error: {ttsEx.Message}");
                }
            }

            await Navigation.PushAsync(new RecipeDetailPage(randomRecipe, _recipeService));
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"GetRandomRecipeAndNavigate error: {ex.Message}");
        }
        finally
        {
            _isNavigating = false;
        }
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

    private async void OnAddTapped(object? sender, TappedEventArgs e)
    {
        var currentRecipes = _recipeService.GetAllRecipes();
        await Navigation.PushAsync(new AddRecipePage(_recipeService, currentRecipes));
    }

    private async void OnRecipeTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is Recipe selectedRecipe)
        {
            await Navigation.PushAsync(new RecipeDetailPage(selectedRecipe, _recipeService));
        }
    }

    private async void OnDailyRecommendationTapped(object sender, TappedEventArgs e)
    {
        if (_dailyRecommendation != null)
        {
            await Navigation.PushAsync(new RecipeDetailPage(_dailyRecommendation, _recipeService));
        }
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        InitializeHardware();

        _justReturned = true;

        Task.Delay(500).ContinueWith(_ =>
        {
            _justReturned = false;
            try
            {
                if (_accelerometer != null && _accelerometer.IsSupported && !_accelerometer.IsMonitoring)
                {
                    _accelerometer.Start(SensorSpeed.Game);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to start accelerometer: {ex.Message}");
            }
        });

        _allRecipes = _recipeService.GetAllRecipes();
        _displayedRecipes = new List<Recipe>(_allRecipes);
        ApplyFilterAndSearch();
        RefreshFonts();
        LoadDailyRecommendation();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();

        try
        {
            if (_accelerometer != null && _accelerometer.IsSupported && _accelerometer.IsMonitoring)
            {
                _accelerometer.Stop();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to stop accelerometer: {ex.Message}");
        }
    }

    protected new void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
    }
}