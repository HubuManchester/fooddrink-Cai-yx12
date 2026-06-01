using System.Net.Http.Json;
using RecipeRandomizer.Models;

namespace RecipeRandomizer.Services;

public class ApiService
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public ApiService()
    {
        // 使用新的、标准的基础地址
        _baseUrl = "https://6a1d4d34bcc4f20d5ca45888.mockapi.io/api/v1/";
        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new Uri(_baseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);
    }

    public async Task<List<Recipe>?> GetRecipesAsync()
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Recipe>>("recipes");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"API Error: {ex.Message}");
            return null;
        }
    }

    public async Task<Recipe?> GetRecipeByIdAsync(string id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Recipe>($"recipes/{id}");
        }
        catch (Exception)
        {
            return null;
        }
    }
}