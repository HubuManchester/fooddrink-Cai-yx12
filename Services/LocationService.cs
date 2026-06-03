using Microsoft.Maui.Devices.Sensors;

namespace RecipeRandomizer.Services;

public static class LocationService
{
    public static async Task<LocationData?> GetCurrentLocationAsync()
    {
        try
        {
            var permissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (permissionStatus != PermissionStatus.Granted)
            {
                permissionStatus = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (permissionStatus != PermissionStatus.Granted)
                {
                    return null;
                }
            }

            var location = await Geolocation.Default.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.Medium,
                Timeout = TimeSpan.FromSeconds(30)
            });

            if (location == null) return null;

            var placemarks = await Geocoding.Default.GetPlacemarksAsync(location.Latitude, location.Longitude);
            var placemark = placemarks?.FirstOrDefault();

            return new LocationData
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Country = placemark?.CountryName ?? "Unknown",
                City = placemark?.Locality ?? placemark?.SubAdminArea ?? "Unknown",
                Street = placemark?.Thoroughfare ?? "",
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Location error: {ex.Message}");
            return null;
        }
    }
}

public class LocationData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }

    public string DisplayText => $"{City}, {Country}\n📍 {Latitude:F4}, {Longitude:F4}";
    public string Coordinates => $"{Latitude:F4}, {Longitude:F4}";
}