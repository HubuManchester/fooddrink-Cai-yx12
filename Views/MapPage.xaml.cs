using RecipeRandomizer.Services;

namespace RecipeRandomizer.Views;

public partial class MapPage : ContentPage
{
    private LocationData? _currentLocation;

    public MapPage()
    {
        InitializeComponent();
        OpenMapButton.Clicked += OnOpenMapClicked;
        LoadLocation();
        LoadNearbyRestaurants();
    }

    private async void LoadLocation()
    {
        try
        {
            _currentLocation = await LocationService.GetCurrentLocationAsync();
            if (_currentLocation != null)
            {
                LocationLabel.Text = $"📍 {_currentLocation.City}, {_currentLocation.Country}\n🌐 {_currentLocation.Coordinates}";
            }
            else
            {
                LocationLabel.Text = "⚠️ Unable to get location. Please enable GPS.";
            }
        }
        catch (Exception ex)
        {
            LocationLabel.Text = $"❌ Location error: {ex.Message}";
        }
    }

    private void LoadNearbyRestaurants()
    {
        var restaurants = new List<RestaurantItem>
        {
            new RestaurantItem { Name = "🍕 Pizza House", Address = "123 Main Street", Distance = "0.2 km away" },
            new RestaurantItem { Name = "🍔 Burger King", Address = "456 Oak Avenue", Distance = "0.5 km away" },
            new RestaurantItem { Name = "🍜 Noodle Bar", Address = "789 Pine Road", Distance = "0.8 km away" },
            new RestaurantItem { Name = "☕ Coffee Shop", Address = "321 Elm Street", Distance = "1.0 km away" },
            new RestaurantItem { Name = "🍣 Sushi Place", Address = "654 Maple Drive", Distance = "1.3 km away" },
            new RestaurantItem { Name = "🥗 Salad Stop", Address = "987 Birch Lane", Distance = "1.6 km away" },
            new RestaurantItem { Name = "🌮 Taco Bell", Address = "147 Cedar Court", Distance = "2.0 km away" }
        };
        RestaurantsCollectionView.ItemsSource = restaurants;
    }

    private async void OnOpenMapClicked(object? sender, EventArgs e)
    {
        if (_currentLocation != null)
        {
            var location = new Location(_currentLocation.Latitude, _currentLocation.Longitude);
            var options = new MapLaunchOptions { Name = "Your Location" };
            await Map.Default.OpenAsync(location, options);
        }
        else
        {
            await DisplayAlert("Location Unavailable", "Cannot open map without your location. Please enable GPS.", "OK");
        }
    }
}

public class RestaurantItem
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Distance { get; set; } = string.Empty;
}