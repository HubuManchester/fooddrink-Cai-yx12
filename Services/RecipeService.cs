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
            // 原有食谱 1-8
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
            
            // 新增食谱 9-25
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
                PrepTime = 5, IsFavorite = false },
            new Recipe { Id = 17, Name = "Omelette", Category = "Breakfast",
                Ingredients = new List<string> { "3 eggs", "Cheese", "Ham", "Mushrooms", "Butter", "Salt", "Pepper" },
                Steps = new List<string> { "Whisk eggs with salt and pepper", "Melt butter in pan", "Pour eggs into pan", "Add fillings on one side", "Fold omelette in half", "Cook until cheese melts", "Serve hot" },
                PrepTime = 10, IsFavorite = false },
            new Recipe { Id = 18, Name = "Pad Thai", Category = "Thai",
                Ingredients = new List<string> { "200g rice noodles", "100g shrimp", "2 eggs", "Tofu", "Bean sprouts", "Peanuts", "Tamarind paste", "Fish sauce" },
                Steps = new List<string> { "Soak noodles in warm water", "Stir-fry shrimp and tofu", "Push to side, scramble eggs", "Add noodles and sauce", "Toss with bean sprouts and peanuts", "Serve with lime wedge" },
                PrepTime = 30, IsFavorite = false },
            new Recipe { Id = 19, Name = "Grilled Cheese Sandwich", Category = "Lunch",
                Ingredients = new List<string> { "2 slices bread", "Butter", "2 slices cheese" },
                Steps = new List<string> { "Butter outside of bread", "Place cheese between bread", "Cook in pan until golden brown", "Flip and cook other side", "Serve hot" },
                PrepTime = 10, IsFavorite = false },
            new Recipe { Id = 20, Name = "Mashed Potatoes", Category = "Side Dish",
                Ingredients = new List<string> { "4 potatoes", "Butter", "Milk", "Salt", "Pepper" },
                Steps = new List<string> { "Peel and cut potatoes", "Boil until soft", "Mash potatoes", "Add butter and milk", "Season with salt and pepper", "Mix until smooth" },
                PrepTime = 25, IsFavorite = false },
            new Recipe { Id = 21, Name = "Guacamole", Category = "Appetizer",
                Ingredients = new List<string> { "2 avocados", "1 lime", "Salt", "Cilantro", "Onion", "Tomato" },
                Steps = new List<string> { "Mash avocados in a bowl", "Add lime juice", "Dice onion and tomato", "Chop cilantro", "Mix all ingredients", "Season with salt", "Serve with tortilla chips" },
                PrepTime = 10, IsFavorite = false },
            new Recipe { Id = 22, Name = "Pasta Carbonara", Category = "Italian",
                Ingredients = new List<string> { "200g pasta", "100g pancetta", "2 eggs", "Parmesan cheese", "Black pepper" },
                Steps = new List<string> { "Cook pasta", "Fry pancetta until crisp", "Whisk eggs and cheese", "Mix hot pasta with egg mixture", "Add pancetta", "Season with pepper" },
                PrepTime = 20, IsFavorite = false },
            new Recipe { Id = 23, Name = "Banana Bread", Category = "Dessert",
                Ingredients = new List<string> { "3 ripe bananas", "200g flour", "2 eggs", "100g sugar", "100g butter", "1 tsp baking soda" },
                Steps = new List<string> { "Preheat oven to 175°C", "Mash bananas", "Cream butter and sugar", "Add eggs", "Mix in flour and baking soda", "Add bananas", "Bake for 60 minutes" },
                PrepTime = 70, IsFavorite = false },
            new Recipe { Id = 24, Name = "Spring Rolls", Category = "Appetizer",
                Ingredients = new List<string> { "Rice paper wrappers", "Shrimp", "Vermicelli noodles", "Lettuce", "Mint", "Peanut sauce" },
                Steps = new List<string> { "Soak rice paper in warm water", "Place on damp towel", "Add fillings", "Roll tightly", "Serve with peanut sauce" },
                PrepTime = 20, IsFavorite = false },
            new Recipe { Id = 25, Name = "Lemonade", Category = "Drink",
                Ingredients = new List<string> { "4 lemons", "1 cup sugar", "4 cups water", "Ice cubes" },
                Steps = new List<string> { "Juice the lemons", "Make simple syrup with sugar and water", "Mix lemon juice with syrup", "Add water", "Serve over ice" },
                PrepTime = 10, IsFavorite = false }
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
        if (recipes == null || recipes.Count == 0)
        {
            return new Recipe { Name = "No recipes available", Category = "Default", PrepTime = 0 };
        }
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
        var recipes = GetAllRecipes();
        var recipe = recipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe != null)
        {
            recipe.IsFavorite = !recipe.IsFavorite;

            if (_useRemoteData && _cachedRecipes.Count > 0)
            {
                var cachedRecipe = _cachedRecipes.FirstOrDefault(r => r.Id == recipeId);
                if (cachedRecipe != null)
                {
                    cachedRecipe.IsFavorite = recipe.IsFavorite;
                }
            }
        }
    }

    public List<Recipe> GetFavorites()
    {
        var recipes = GetAllRecipes();
        return recipes.Where(r => r.IsFavorite).ToList();
    }

    public void AddLocalRecipe(Recipe newRecipe)
    {
        _localRecipes.Add(newRecipe);
        _cachedRecipes.Add(newRecipe);
    }
}