using System.Collections.Generic;
using RecipeManagement.Core;

namespace RecipeManagement.Tests;

/// <summary>
/// Example tests from the assignment specification. Add your own tests as you work.
/// </summary>
public sealed class RecipeManagerTests
{
    [Fact]
    public void Constructor_BuildsRecipeDictionary()
    {
        var manager = CreateManager();
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal("Recipe A", manager.FindRecipe(10)?.Title);
    }

    [Fact]
    public void Constructor_ThrowsForNullRecipes()
    {
        Assert.Throws<ArgumentNullException>(() => new RecipeManager(null!));
    }

    [Fact]
    public void Constructor_ThrowsForInvalidRecipe()
    {
        var invalidRecipe = new Recipe { Id = 0, Title = "Invalid Recipe" };

        Assert.Throws<ArgumentException>(() => new RecipeManager(new[] { invalidRecipe }));
    }

    [Fact]
    public void Constructor_ThrowsForDuplicateRecipeId()
    {
        var recipes = new[]
        {
            new Recipe { Id = 10, Title = "Recipe A" },
            new Recipe { Id = 10, Title = "Recipe B" }
        };

        Assert.Throws<ArgumentException>(() => new RecipeManager(recipes));
    }

    [Fact]
    public void FindRecipe_ReturnsNullWhenRecipeDoesNotExist()
    {
        var manager = CreateManager();

        Assert.Null(manager.FindRecipe(99));
    }

    [Fact]
    public void AddRecipe_AddsRecipeToCatalogue()
    {
        var manager = CreateManager();
        var recipe = new Recipe { Id = 30, Title = "Recipe C" };

        Assert.True(manager.AddRecipe(recipe));
        Assert.Equal(3, manager.RecipeCount);
        Assert.Equal(recipe, manager.FindRecipe(30));
    }

    [Fact]
    public void AddRecipe_ReturnsFalseForDuplicateId()
    {
        var manager = CreateManager();
        var duplicate = new Recipe { Id = 10, Title = "Another Recipe" };

        Assert.False(manager.AddRecipe(duplicate));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_ReturnsFalseForInvalidRecipe()
    {
        var manager = CreateManager();
        var invalidRecipe = new Recipe { Id = 0, Title = "Invalid Recipe" };

        Assert.False(manager.AddRecipe(invalidRecipe));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void AddRecipe_ThrowsForNullRecipe()
    {
        var manager = CreateManager();

        Assert.Throws<ArgumentNullException>(() => manager.AddRecipe(null!));
    }

    [Fact]
    public void RemoveRecipe_RemovesRecipeFromCatalogue()
    {
        var manager = CreateManager();

        Assert.True(manager.RemoveRecipe(10));
        Assert.Equal(1, manager.RecipeCount);
        Assert.Null(manager.FindRecipe(10));
    }

    [Fact]
    public void RemoveRecipe_ReturnsFalseWhenRecipeDoesNotExist()
    {
        var manager = CreateManager();

        Assert.False(manager.RemoveRecipe(99));
        Assert.Equal(2, manager.RecipeCount);
    }

    [Fact]
    public void RemoveRecipe_ReturnsFalseWhenRecipeIsInCookingPlan()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        Assert.False(manager.RemoveRecipe(10));
        Assert.Equal(2, manager.RecipeCount);
        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void AddIngredientsToShoppingList_AddsIngredientsInOrder()
    {
        var manager = CreateManager();
        manager.AddRecipe(new Recipe
        {
            Id = 30,
            Title = "Recipe C",
            Ingredients = new() { "1 cup flour", "2 eggs" }
        });

        Assert.Equal(2, manager.AddIngredientsToShoppingList(30));
        Assert.Equal(new[] { "1 cup flour", "2 eggs" }, manager.GetShoppingList());
        Assert.Equal(2, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddIngredientsToShoppingList_ReturnsZeroForMissingRecipe()
    {
        var manager = CreateManager();

        Assert.Equal(0, manager.AddIngredientsToShoppingList(99));
        Assert.Empty(manager.GetShoppingList());
    }

    [Fact]
    public void ClearShoppingList_RemovesAllIngredients()
    {
        var manager = CreateManager();
        manager.AddIngredientsToShoppingList(10);

        manager.ClearShoppingList();

        Assert.Empty(manager.GetShoppingList());
        Assert.Equal(0, manager.ShoppingItemCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_AddsRecipesInOrder()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.True(manager.AddRecipeToCookingPlan(20));

        Assert.Equal(new[] { 10, 20 }, manager.GetCookingPlan());
        Assert.Equal(2, manager.CookingPlanCount);
    }

    [Fact]
    public void AddRecipeToCookingPlan_ReturnsFalseForDuplicateRecipe()
    {
        var manager = CreateManager();

        Assert.True(manager.AddRecipeToCookingPlan(10));
        Assert.False(manager.AddRecipeToCookingPlan(10));
        Assert.Equal(new[] { 10 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_PushesRecipeToHistory()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);

        Assert.True(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Empty(manager.GetCookingPlan());
        Assert.Equal(10, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RemoveRecipeFromCookingPlan_ReturnsFalseForMissingRecipe()
    {
        var manager = CreateManager();

        Assert.False(manager.RemoveRecipeFromCookingPlan(10));
        Assert.Empty(manager.GetCookingPlan());
        Assert.Equal(0, manager.RemovedRecipeCount);
    }

    [Fact]
    public void PeekLastRemovedRecipe_ReturnsNullWhenHistoryIsEmpty()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_ReturnsFalseWhenHistoryIsEmpty()
    {
        var manager = CreateManager();

        Assert.False(manager.RestoreLastRemovedRecipe());
    }

    [Fact]
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
    }

    [Fact]
    public void InstructionsReturnNullWhenQueueIsEmpty()
    {
        var manager = CreateManager();

        Assert.Null(manager.PeekNextInstruction());
        Assert.Null(manager.CompleteNextInstruction());
    }

    [Fact]
    public void StartCookingReturnsFalseForRecipeWithoutInstructions()
    {
        var manager = CreateManager();

        Assert.False(manager.StartCooking(20));
        Assert.False(manager.StartCooking(99));
    }

    [Fact]
    public void StartCookingReplacesPreviousInstructions()
    {
        var manager = CreateManager();

        manager.StartCooking(10);
        manager.CompleteNextInstruction();
        manager.StartCooking(10);

        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal(2, manager.PendingInstructionCount);
    }

    [Fact]
    public void RemovedRecipesAreRestoredLastInFirstOut()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.AddRecipeToCookingPlan(20);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(20);
        Assert.Equal(20, manager.PeekLastRemovedRecipe());
        Assert.True(manager.RestoreLastRemovedRecipe());
        Assert.Equal(new[] { 20 }, manager.GetCookingPlan());
    }

    [Fact]
    public void RestoreLastRemovedRecipe_PreservesHistoryWhenRestoreFails()
    {
        var manager = CreateManager();
        manager.AddRecipeToCookingPlan(10);
        manager.RemoveRecipeFromCookingPlan(10);
        manager.RemoveRecipe(10);

        Assert.False(manager.RestoreLastRemovedRecipe());
        Assert.Equal(1, manager.RemovedRecipeCount);
        Assert.Equal(10, manager.PeekLastRemovedRecipe());
    }

    [Fact]
    public void SearchByTitle_ReturnsMatchingRecipesIgnoringCase()
    {
        var manager = CreateManager();

        var results = manager.SearchByTitle("recipe a");

        Assert.Single(results);
        Assert.Equal(10, results[0].Id);
    }

    [Fact]
    public void SearchByIngredient_ReturnsRecipesWithMatchingIngredientIgnoringCase()
    {
        var manager = CreateManager();

        var results = manager.SearchByIngredient("APPLE");

        Assert.Single(results);
        Assert.Equal(10, results[0].Id);
    }

    [Fact]
    public void SearchByTitle_ReturnsAllRecipesForBlankText()
    {
        var manager = CreateManager();

        Assert.Equal(2, manager.SearchByTitle(" ").Count);
        Assert.Empty(manager.SearchByIngredient(string.Empty));
    }

    [Fact]
    public void GetHighestProteinRecipes_ReturnsDescendingResultsUpToCount()
    {
        var manager = CreateManager();
        manager.AddRecipe(new Recipe
        {
            Id = 30,
            Title = "High Protein Recipe",
            Nutrition = new NutritionInfo { ProteinG = 30 }
        });
        manager.AddRecipe(new Recipe
        {
            Id = 40,
            Title = "Medium Protein Recipe",
            Nutrition = new NutritionInfo { ProteinG = 20 }
        });

        var results = manager.GetHighestProteinRecipes(2);

        Assert.Equal(30, results[0].Id);
        Assert.Equal(40, results[1].Id);
    }

    [Fact]
    public void GetHighestProteinRecipes_ExcludesRecipesWithoutProtein()
    {
        var manager = CreateManager();
        manager.AddRecipe(new Recipe
        {
            Id = 30,
            Title = "Protein Recipe",
            Nutrition = new NutritionInfo { ProteinG = 15 }
        });

        var results = manager.GetHighestProteinRecipes(5);

        Assert.Single(results);
        Assert.Equal(30, results[0].Id);
    }

    [Fact]
    public void PartBWorkflow()
    {
        var manager = CreateManager();
        var searchResults = manager.SearchByTitle("recipe a");
        var recipeId = searchResults[0].Id;

        Assert.True(manager.AddSavedRecipe(recipeId));
        Assert.True(manager.IsRecipeSaved(recipeId));
        Assert.True(manager.RemoveSavedRecipe(recipeId));
        Assert.False(manager.IsRecipeSaved(recipeId));
    }

    [Fact]
    public void AddSavedRecipe_SavesExistingRecipeOnlyOnce()
    {
        var manager = CreateManager();

        Assert.True(manager.AddSavedRecipe(10));
        Assert.False(manager.AddSavedRecipe(10));
        Assert.True(manager.IsRecipeSaved(10));
        Assert.Equal(new[] { 10 }, manager.GetSavedRecipes());
    }

    [Fact]
    public void AddSavedRecipe_ReturnsFalseForMissingRecipe()
    {
        var manager = CreateManager();

        Assert.False(manager.AddSavedRecipe(99));
        Assert.False(manager.IsRecipeSaved(99));
        Assert.Empty(manager.GetSavedRecipes());
    }

    [Fact]
    public void RemoveSavedRecipe_ReturnsFalseWhenRecipeIsNotSaved()
    {
        var manager = CreateManager();

        Assert.False(manager.RemoveSavedRecipe(10));
        Assert.Empty(manager.GetSavedRecipes());
    }

    [Fact]
    public void RemoveSavedRecipe_RemovesSavedRecipe()
    {
        var manager = CreateManager();
        manager.AddSavedRecipe(10);

        Assert.True(manager.RemoveSavedRecipe(10));
        Assert.False(manager.IsRecipeSaved(10));
        Assert.Empty(manager.GetSavedRecipes());
    }

    private static RecipeManager CreateManager()
    {
        return new RecipeManager(new[]
        {
            new Recipe
            {
                Id = 10,
                Title = "Recipe A",
                Ingredients = new() { "1 apple" },
                Instructions = new() { "First step", "Second step" }
            },
            new Recipe
            {
                Id = 20,
                Title = "Recipe B"
            }
        });
    }
}
