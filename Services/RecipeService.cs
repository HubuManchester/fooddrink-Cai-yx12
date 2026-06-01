using RecipeRandomizer.Models;

namespace RecipeRandomizer.Services
{
    public class RecipeService
    {
        private List<Recipe> _allRecipes;

        public RecipeService()
        {
            LoadSampleRecipes();
        }

        private void LoadSampleRecipes()
        {
            _allRecipes = new List<Recipe>
            {
                new Recipe
                {
                    Id = 1,
                    Name = "Tomato Scrambled Eggs",
                    Category = "Chinese",
                    ImageUrl = "tomato_egg.jpg",
                    Ingredients = new List<string> { "2 tomatoes", "3 eggs", "Salt", "Sugar", "Oil" },
                    Steps = new List<string> { "Cut tomatoes", "Beat eggs with salt", "Scramble eggs", "Cook tomatoes", "Mix and season" },
                    PrepTime = 15,
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 2,
                    Name = "Spaghetti Bolognese",
                    Category = "Italian",
                    ImageUrl = "pasta.jpg",
                    Ingredients = new List<string> { "200g spaghetti", "150g beef mince", "100g tomato sauce", "Half onion", "2 garlic cloves", "Salt", "Pepper" },
                    Steps = new List<string> { "Cook pasta", "Fry onion and garlic", "Brown the meat", "Add tomato sauce", "Simmer for 10 min", "Mix with pasta" },
                    PrepTime = 30,
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 3,
                    Name = "Fruit Salad",
                    Category = "Dessert",
                    ImageUrl = "salad.jpg",
                    Ingredients = new List<string> { "1 apple", "1 banana", "1 orange", "100g yogurt", "1 spoon honey" },
                    Steps = new List<string> { "Cut fruits", "Mix with yogurt", "Add honey", "Stir well" },
                    PrepTime = 10,
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 4,
                    Name = "Stir-fried Greens",
                    Category = "Chinese",
                    ImageUrl = "greens.jpg",
                    Ingredients = new List<string> { "500g greens", "3 garlic cloves", "Salt", "Oil" },
                    Steps = new List<string> { "Wash greens", "Heat oil and garlic", "Add greens", "Add salt and serve" },
                    PrepTime = 10,
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 5,
                    Name = "Chocolate Cake",
                    Category = "Dessert",
                    ImageUrl = "cake.jpg",
                    Ingredients = new List<string> { "200g flour", "3 eggs", "100g chocolate", "100g butter", "100g sugar" },
                    Steps = new List<string> { "Melt butter and chocolate", "Beat eggs with sugar", "Mix in flour", "Bake at 180C for 30 min" },
                    PrepTime = 60,
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 6,
                    Name = "Sushi Roll",
                    Category = "Japanese",
                    ImageUrl = "sushi.jpg",
                    Ingredients = new List<string> { "Nori seaweed", "Sushi rice", "Fresh fish", "Cucumber", "Avocado", "Soy sauce" },
                    Steps = new List<string> { "Prepare rice", "Lay nori on mat", "Spread rice", "Add fillings", "Roll tightly", "Slice and serve" },
                    PrepTime = 45,
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 7,
                    Name = "Caesar Salad",
                    Category = "Western",
                    ImageUrl = "caesar.jpg",
                    Ingredients = new List<string> { "Lettuce", "Croutons", "Parmesan cheese", "Caesar dressing", "Grilled chicken" },
                    Steps = new List<string> { "Wash lettuce", "Add croutons", "Add cheese", "Add dressing", "Top with chicken" },
                    PrepTime = 15,
                    IsFavorite = false
                },
                new Recipe
                {
                    Id = 8,
                    Name = "Pancakes",
                    Category = "Breakfast",
                    ImageUrl = "pancakes.jpg",
                    Ingredients = new List<string> { "200g flour", "2 eggs", "300ml milk", "1 tbsp sugar", "Butter", "Maple syrup" },
                    Steps = new List<string> { "Mix dry ingredients", "Add eggs and milk", "Whisk until smooth", "Cook on pan until bubbles form", "Flip and cook other side", "Serve with syrup" },
                    PrepTime = 20,
                    IsFavorite = false
                }
            };
        }

        public List<Recipe> GetAllRecipes()
        {
            return _allRecipes;
        }

        public List<Recipe> GetRecipesByCategory(string category)
        {
            if (string.IsNullOrEmpty(category) || category == "All")
                return _allRecipes;
            return _allRecipes.Where(r => r.Category == category).ToList();
        }

        public List<Recipe> SearchRecipes(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return _allRecipes;
            return _allRecipes.Where(r =>
                r.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                r.Category.Contains(keyword, StringComparison.OrdinalIgnoreCase)
            ).ToList();
        }

        public Recipe GetRandomRecipe()
        {
            var random = new Random();
            return _allRecipes[random.Next(_allRecipes.Count)];
        }

        public List<string> GetCategories()
        {
            var categories = _allRecipes.Select(r => r.Category).Distinct().ToList();
            categories.Insert(0, "All");
            return categories;
        }

        public void ToggleFavorite(int recipeId)
        {
            var recipe = _allRecipes.FirstOrDefault(r => r.Id == recipeId);
            if (recipe != null)
            {
                recipe.IsFavorite = !recipe.IsFavorite;
            }
        }

        public List<Recipe> GetFavorites()
        {
            return _allRecipes.Where(r => r.IsFavorite).ToList();
        }
    }
}