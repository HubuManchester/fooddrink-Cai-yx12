using RecipeRandomizer.Models;

namespace RecipeRandomizer.Services;

public class RecipeService
{
    private List<Recipe> _localRecipes;
    private List<Recipe> _cachedRecipes = new();
    private readonly ApiService _apiService;
    private bool _useRemoteData = true;
    private bool _isLoadingRemoteData = false;

    public RecipeService()
    {
        LoadLocalRecipes();
        _apiService = new ApiService();

        // 异步加载远程数据
        Task.Run(async () => await LoadRemoteDataAsync());
    }

    private void LoadLocalRecipes()
    {
        _localRecipes = new List<Recipe>
    {
        new Recipe { Id = 1, Name = "Tomato Scrambled Eggs", Category = "Chinese",
            Ingredients = new List<string> { "2 tomatoes", "3 eggs", "Salt", "Sugar", "Oil" },
            Steps = new List<string> { "Cut tomatoes", "Beat eggs with salt", "Scramble eggs", "Cook tomatoes", "Mix and season" },
            PrepTime = 15, IsFavorite = false },
        new Recipe { Id = 2, Name = "Spaghetti Bolognese", Category = "Italian",
            Ingredients = new List<string> { "200g spaghetti", "150g beef mince", "100g tomato sauce", "Half onion", "2 garlic cloves" },
            Steps = new List<string> { "Cook pasta", "Fry onion and garlic", "Brown the meat", "Add tomato sauce", "Simmer for 10 min", "Mix with pasta" },
            PrepTime = 30, IsFavorite = false },
        new Recipe { Id = 3, Name = "Fruit Salad", Category = "Dessert",
            Ingredients = new List<string> { "1 apple", "1 banana", "1 orange", "100g yogurt", "1 spoon honey" },
            Steps = new List<string> { "Cut fruits", "Mix with yogurt", "Add honey", "Stir well" },
            PrepTime = 10, IsFavorite = false },
        new Recipe { Id = 4, Name = "Stir-fried Greens", Category = "Chinese",
            Ingredients = new List<string> { "500g greens", "3 garlic cloves", "Salt", "Oil" },
            Steps = new List<string> { "Wash greens", "Heat oil and garlic", "Add greens", "Add salt and serve" },
            PrepTime = 10, IsFavorite = false },
        new Recipe { Id = 5, Name = "Chocolate Cake", Category = "Dessert",
            Ingredients = new List<string> { "200g flour", "3 eggs", "100g chocolate", "100g butter", "100g sugar" },
            Steps = new List<string> { "Melt butter and chocolate", "Beat eggs with sugar", "Mix in flour", "Bake at 180C for 30 min" },
            PrepTime = 60, IsFavorite = false },
        new Recipe { Id = 6, Name = "Sushi Roll", Category = "Japanese",
            Ingredients = new List<string> { "Nori seaweed", "Sushi rice", "Fresh fish", "Cucumber", "Avocado" },
            Steps = new List<string> { "Prepare rice", "Lay nori on mat", "Spread rice", "Add fillings", "Roll tightly", "Slice and serve" },
            PrepTime = 45, IsFavorite = false },
        new Recipe { Id = 7, Name = "Caesar Salad", Category = "Western",
            Ingredients = new List<string> { "Lettuce", "Croutons", "Parmesan cheese", "Caesar dressing", "Grilled chicken" },
            Steps = new List<string> { "Wash lettuce", "Add croutons", "Add cheese", "Add dressing", "Top with chicken" },
            PrepTime = 15, IsFavorite = false },
        new Recipe { Id = 8, Name = "Pancakes", Category = "Breakfast",
            Ingredients = new List<string> { "200g flour", "2 eggs", "300ml milk", "1 tbsp sugar", "Butter", "Maple syrup" },
            Steps = new List<string> { "Mix dry ingredients", "Add eggs and milk", "Whisk until smooth", "Cook on pan until bubbles form", "Flip and cook other side", "Serve with syrup" },
            PrepTime = 20, IsFavorite = false },     
        new Recipe { Id = 9, Name = "Beef Burger", Category = "Western",
            Ingredients = new List<string> { "Burger bun", "Beef patty", "Lettuce", "Tomato", "Cheese", "Ketchup", "Mayonnaise" },
            Steps = new List<string> { "Toast the burger bun", "Cook beef patty on grill", "Place cheese on patty to melt", "Assemble bun with lettuce, tomato, patty", "Add ketchup and mayonnaise", "Serve with fries" },
            PrepTime = 25, IsFavorite = false },

        new Recipe { Id = 10, Name = "Fried Rice", Category = "Chinese",
            Ingredients = new List<string> { "2 cups cooked rice", "2 eggs", "100g shrimp or chicken", "Soy sauce", "Green onions", "Oil" },
            Steps = new List<string> { "Scramble eggs in wok", "Add meat and stir-fry", "Add rice and break up clumps", "Add soy sauce for color", "Stir-fry for 2-3 minutes", "Garnish with green onions" },
            PrepTime = 15, IsFavorite = false },

        new Recipe { Id = 11, Name = "Greek Yogurt Bowl", Category = "Breakfast",
            Ingredients = new List<string> { "200g Greek yogurt", "Granola", "Mixed berries", "Honey", "Chia seeds" },
            Steps = new List<string> { "Scoop yogurt into a bowl", "Top with granola and berries", "Drizzle with honey", "Sprinkle chia seeds", "Serve immediately" },
            PrepTime = 5, IsFavorite = false },

        new Recipe { Id = 12, Name = "Chicken Noodle Soup", Category = "Lunch",
            Ingredients = new List<string> { "200g chicken breast", "100g egg noodles", "Carrots", "Celery", "Onion", "Chicken broth", "Herbs" },
            Steps = new List<string> { "Sauté onion, carrots, celery", "Add broth and bring to boil", "Add chicken and simmer", "Shred chicken when cooked", "Add noodles and cook until tender", "Season with herbs" },
            PrepTime = 40, IsFavorite = false },

        new Recipe { Id = 13, Name = "Tacos", Category = "Mexican",
            Ingredients = new List<string> { "Taco shells", "Ground beef", "Taco seasoning", "Lettuce", "Cheese", "Salsa", "Sour cream" },
            Steps = new List<string> { "Brown ground beef with taco seasoning", "Warm taco shells", "Fill shells with beef", "Top with lettuce, cheese, salsa", "Add sour cream", "Serve with lime wedges" },
            PrepTime = 20, IsFavorite = false },

        new Recipe { Id = 14, Name = "Green Smoothie", Category = "Drink",
            Ingredients = new List<string> { "1 cup spinach", "1 banana", "1/2 cup mango", "1/2 cup orange juice", "1/2 cup Greek yogurt" },
            Steps = new List<string> { "Add spinach and orange juice to blender", "Add banana and mango", "Add Greek yogurt", "Blend until smooth", "Pour and serve cold" },
            PrepTime = 5, IsFavorite = false },

        new Recipe { Id = 15, Name = "Roasted Vegetables", Category = "Lunch",
            Ingredients = new List<string> { "Broccoli", "Carrots", "Bell peppers", "Red onion", "Olive oil", "Salt", "Pepper", "Rosemary" },
            Steps = new List<string> { "Preheat oven to 200°C", "Chop all vegetables", "Toss with oil and seasoning", "Spread on baking sheet", "Roast for 20-25 minutes", "Serve warm" },
            PrepTime = 30, IsFavorite = false },

        new Recipe { Id = 16, Name = "Ice Cream Sundae", Category = "Dessert",
            Ingredients = new List<string> { "2 scoops vanilla ice cream", "Chocolate sauce", "Whipped cream", "Sprinkles", "Cherry" },
            Steps = new List<string> { "Place ice cream in a bowl", "Drizzle with chocolate sauce", "Add whipped cream", "Top with sprinkles", "Place cherry on top", "Serve immediately" },
            PrepTime = 5, IsFavorite = false }
    };
    }

    private async Task LoadRemoteDataAsync()
    {
        if (_isLoadingRemoteData) return;
        _isLoadingRemoteData = true;

        try
        {
            var remoteRecipes = await _apiService.GetRecipesAsync();
            if (remoteRecipes != null && remoteRecipes.Count > 0)
            {
                _cachedRecipes = remoteRecipes;
                _useRemoteData = true;
            }
            else
            {
                _useRemoteData = false;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load remote data: {ex.Message}");
            _useRemoteData = false;
        }
        finally
        {
            _isLoadingRemoteData = false;
        }
    }

    public List<Recipe> GetAllRecipes()
    {
        if (_useRemoteData && _cachedRecipes.Count > 0)
        {
            return _cachedRecipes;
        }
        return _localRecipes;
    }

    public List<Recipe> GetRecipesByCategory(string category)
    {
        var recipes = GetAllRecipes();
        if (string.IsNullOrEmpty(category) || category == "All")
            return recipes;
        return recipes.Where(r => r.Category == category).ToList();
    }

    public List<Recipe> SearchRecipes(string keyword)
    {
        var recipes = GetAllRecipes();
        if (string.IsNullOrEmpty(keyword))
            return recipes;
        return recipes.Where(r => r.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase)).ToList();
    }

    public Recipe GetRandomRecipe()
    {
        var recipes = GetAllRecipes();
        var random = new Random();
        return recipes[random.Next(recipes.Count)];
    }

    public List<string> GetCategories()
    {
        var recipes = GetAllRecipes();
        var categories = recipes.Select(r => r.Category).Distinct().ToList();
        categories.Insert(0, "All");
        return categories;
    }

    public void ToggleFavorite(int recipeId)
    {
        System.Diagnostics.Debug.WriteLine($"=== ToggleFavorite called with ID: {recipeId}");

        // 先尝试在缓存中找
        if (_useRemoteData && _cachedRecipes.Count > 0)
        {
            var cachedRecipe = _cachedRecipes.FirstOrDefault(r => r.Id == recipeId);
            if (cachedRecipe != null)
            {
                System.Diagnostics.Debug.WriteLine($"=== Found in cache: {cachedRecipe.Name}, current favorite: {cachedRecipe.IsFavorite}");
                cachedRecipe.IsFavorite = !cachedRecipe.IsFavorite;
                System.Diagnostics.Debug.WriteLine($"=== New favorite status: {cachedRecipe.IsFavorite}");
                return;
            }
        }

        // 如果在缓存中没找到，尝试在本地数据中找
        var localRecipe = _localRecipes.FirstOrDefault(r => r.Id == recipeId);
        if (localRecipe != null)
        {
            System.Diagnostics.Debug.WriteLine($"=== Found in local: {localRecipe.Name}, current favorite: {localRecipe.IsFavorite}");
            localRecipe.IsFavorite = !localRecipe.IsFavorite;
            System.Diagnostics.Debug.WriteLine($"=== New favorite status: {localRecipe.IsFavorite}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"=== ERROR: Recipe with ID {recipeId} not found anywhere!");

            // 打印所有可用的 ID
            System.Diagnostics.Debug.WriteLine("=== Available cache IDs:");
            foreach (var r in _cachedRecipes)
            {
                System.Diagnostics.Debug.WriteLine($"Cache ID: {r.Id}, Name: {r.Name}");
            }
            System.Diagnostics.Debug.WriteLine("=== Available local IDs:");
            foreach (var r in _localRecipes)
            {
                System.Diagnostics.Debug.WriteLine($"Local ID: {r.Id}, Name: {r.Name}");
            }
        }
    }

    public List<Recipe> GetFavorites()
    {
        var recipes = GetAllRecipes();
        return recipes.Where(r => r.IsFavorite).ToList();
    }
}