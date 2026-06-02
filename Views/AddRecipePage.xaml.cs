using RecipeRandomizer.Models;
using RecipeRandomizer.Services;

namespace RecipeRandomizer.Views;

public partial class AddRecipePage : ContentPage
{
    private RecipeService _recipeService;
    private List<Recipe> _existingRecipes;

    public AddRecipePage(RecipeService recipeService, List<Recipe> existingRecipes)
    {
        InitializeComponent();
        _recipeService = recipeService;
        _existingRecipes = existingRecipes;

        LoadCategories();
        SaveButton.Clicked += OnSaveClicked;
        CancelButton.Clicked += OnCancelClicked;
    }

    private void LoadCategories()
    {
        var categories = _recipeService.GetCategories();
        var filtered = categories.Where(c => c != "All").ToList();
        CategoryPicker.ItemsSource = filtered;
        if (filtered.Any()) CategoryPicker.SelectedIndex = 0;
    }

    private bool ValidateInputs()
    {
        bool isValid = true;

        if (string.IsNullOrWhiteSpace(NameEntry.Text))
        {
            NameErrorLabel.Text = "Recipe name is required";
            NameErrorLabel.IsVisible = true;
            isValid = false;
        }
        else NameErrorLabel.IsVisible = false;

        if (CategoryPicker.SelectedItem == null)
        {
            CategoryErrorLabel.Text = "Please select a category";
            CategoryErrorLabel.IsVisible = true;
            isValid = false;
        }
        else CategoryErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(PrepTimeEntry.Text) || !int.TryParse(PrepTimeEntry.Text, out int time) || time <= 0)
        {
            PrepTimeErrorLabel.Text = "Valid prep time is required";
            PrepTimeErrorLabel.IsVisible = true;
            isValid = false;
        }
        else PrepTimeErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(IngredientsEditor.Text))
        {
            IngredientsErrorLabel.Text = "At least one ingredient is required";
            IngredientsErrorLabel.IsVisible = true;
            isValid = false;
        }
        else IngredientsErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(StepsEditor.Text))
        {
            StepsErrorLabel.Text = "At least one step is required";
            StepsErrorLabel.IsVisible = true;
            isValid = false;
        }
        else StepsErrorLabel.IsVisible = false;

        return isValid;
    }

    private async void OnSaveClicked(object? sender, EventArgs e)
    {
        if (!ValidateInputs()) return;

        try
        {
            var ingredients = IngredientsEditor.Text.Split('\n', '\r')
                .Select(i => i.Trim()).Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            var steps = StepsEditor.Text.Split('\n', '\r')
                .Select(s => s.Trim()).Where(s => !string.IsNullOrWhiteSpace(s)).ToList();

            int newId = _existingRecipes.Any() ? _existingRecipes.Max(r => r.Id) + 1 : 1;

            var newRecipe = new Recipe
            {
                Id = newId,
                Name = NameEntry.Text.Trim(),
                Category = CategoryPicker.SelectedItem?.ToString() ?? "Other",
                PrepTime = int.Parse(PrepTimeEntry.Text),
                Ingredients = ingredients,
                Steps = steps,
                IsFavorite = false
            };

            _recipeService.AddLocalRecipe(newRecipe);
            await DisplayAlert("Success", $"Recipe '{newRecipe.Name}' added!", "OK");
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed: {ex.Message}", "OK");
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}