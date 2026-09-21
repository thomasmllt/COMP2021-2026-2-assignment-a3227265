using System;
using System.Collections.Generic;

namespace RecipeManagement.Core;

/// <summary>
/// Implement this class using the five Part A collections as private fields:
/// Dictionary&lt;int, Recipe&gt;, List&lt;string&gt;, LinkedList&lt;int&gt;,
/// Stack&lt;int&gt; and Queue&lt;string&gt;.
/// </summary>
public sealed class RecipeManager : IRecipeManager
{
    // TODO Part A: add your private collection fields here.
    private Dictionary<int, Recipe> recipes = new Dictionary<int, Recipe>();
    private List<string> shoppingList = new List<string>();
    private LinkedList<int> cookingPlan = new LinkedList<int>();
    private Stack<int> removedRecipes = new Stack<int>();
    private Queue<string> instructions = new Queue<string>();

    public RecipeManager(IEnumerable<Recipe> recipes)
    {
        // TODO Part A: validate recipes and build Dictionary<int, Recipe>.
        if (recipes is null)
        {
            throw new ArgumentNullException();
        }
        else
        {
            foreach (var recipe in recipes)
            {
                if ((recipe.Id <= 0) || string.IsNullOrWhiteSpace(recipe.Title) || this.recipes.ContainsKey(recipe.Id))
                {
                    throw new ArgumentException();
                }
                else
                {
                    this.recipes.Add(recipe.Id, recipe);
                }
            }
        }
    }

    public int RecipeCount => recipes.Count;
    public int ShoppingItemCount => shoppingList.Count;
    public int CookingPlanCount => cookingPlan.Count;
    public int PendingInstructionCount => instructions.Count;
    public int RemovedRecipeCount => removedRecipes.Count;

    public bool AddRecipe(Recipe? recipe)
    {
        if (recipe is null)
        {
            throw new ArgumentNullException();
        }
        else if ((recipe.Id <= 0) || string.IsNullOrWhiteSpace(recipe.Title) || this.recipes.ContainsKey(recipe.Id))
        {
            return false;
        }
        else
        {
            this.recipes.Add(recipe.Id, recipe);
            return true;
        }
    }

    public Recipe? FindRecipe(int recipeId)
    {
        if (recipes.ContainsKey(recipeId))
        {
            return recipes[recipeId];
        }
        else
        {
            return null;
        }
    }


    public bool RemoveRecipe(int recipeId)
    {
        if (!recipes.ContainsKey(recipeId) || cookingPlan.Contains(recipeId))
        {
            return false;
        }
        else
        {
            recipes.Remove(recipeId);
            return true;
        }
    }

    public int AddIngredientsToShoppingList(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe is null)
        {
            return 0;
        }
        else
        {
            foreach (var ingredient in recipe.Ingredients)
            {
                shoppingList.Add(ingredient);
            }
        }
        return recipe.Ingredients.Count;
    }

    public IReadOnlyList<string> GetShoppingList()
    {
        return new List<string>(shoppingList);
    }

    public void ClearShoppingList() =>
        shoppingList.Clear();

    public bool AddRecipeToCookingPlan(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe is null || cookingPlan.Contains(recipeId))
        {
            return false;
        }
        else
        {
            cookingPlan.AddLast(recipeId);
            return true;
        }
    }

    public bool RemoveRecipeFromCookingPlan(int recipeId)
    {
        if (!cookingPlan.Remove(recipeId))
        {
            return false;
        }

        removedRecipes.Push(recipeId);
        return true;
    }

    public bool RestoreLastRemovedRecipe()
    {
        if (removedRecipes.Count > 0)
        {
            return AddRecipeToCookingPlan(removedRecipes.Pop());
        } else
        {
            return false;
        }
    }

    public int? PeekLastRemovedRecipe()
    {
        if (removedRecipes.Count > 0)
        {
            return removedRecipes.Peek();
        }
        else 
        {
            return null;
        }
    }

    public IReadOnlyList<int> GetCookingPlan()
    {
        return new List<int>(cookingPlan);
    }

    public bool StartCooking(int recipeId) =>
        throw new NotImplementedException("Part A: implement StartCooking.");

    public string? PeekNextInstruction() =>
        throw new NotImplementedException("Part A: implement PeekNextInstruction.");

    public string? CompleteNextInstruction() =>
        throw new NotImplementedException("Part A: implement CompleteNextInstruction.");

    public IReadOnlyList<Recipe> SearchByTitle(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByTitle.");

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText) =>
        throw new NotImplementedException("Part B: implement SearchByIngredient.");

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count) =>
        throw new NotImplementedException("Part B: implement GetHighestProteinRecipes.");

    public bool AddSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement AddSavedRecipe.");

    public bool RemoveSavedRecipe(int recipeId) =>
        throw new NotImplementedException("Part B: implement RemoveSavedRecipe.");

    public bool IsRecipeSaved(int recipeId) =>
        throw new NotImplementedException("Part B: implement IsRecipeSaved.");

    public IReadOnlyList<int> GetSavedRecipes() =>
        throw new NotImplementedException("Part B: implement GetSavedRecipes.");
}
