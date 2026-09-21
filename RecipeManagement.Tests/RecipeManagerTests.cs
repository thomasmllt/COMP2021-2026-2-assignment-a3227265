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
    public void InstructionsAreCompletedInFileOrder()
    {
        var manager = CreateManager();
        Assert.True(manager.StartCooking(10));
        Assert.Equal("First step", manager.PeekNextInstruction());
        Assert.Equal("First step", manager.CompleteNextInstruction());
        Assert.Equal("Second step", manager.PeekNextInstruction());
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
