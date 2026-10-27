using System;
using System.Collections.Generic;
using System.Linq;

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
    private HashSet<int> savedRecipes = new HashSet<int>();

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
            var recipeId = removedRecipes.Peek();
            if (AddRecipeToCookingPlan(recipeId))
            {
                removedRecipes.Pop();
                return true;
            }
        }

        return false;
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

    public bool StartCooking(int recipeId)
    {
        var recipe = FindRecipe(recipeId);
        if (recipe is not null && recipe.Instructions.Count > 0)
        {
            instructions.Clear();
            foreach (var instruction in recipe.Instructions)
            {
                instructions.Enqueue(instruction);
            }
            return true;
        }
        return false;
    }

    public string? PeekNextInstruction()
    {
        if (instructions.Count > 0)
        {
            return instructions.Peek();
        }
        else
        {
            return null;
        }
    }

    public string? CompleteNextInstruction()
    {
        if (instructions.Count > 0)
        {
            return instructions.Dequeue();
        }
        else
        {
            return null;
        }
    }

    public IReadOnlyList<Recipe> SearchByTitle(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return recipes.Values.ToList();
        }

        string normalizedSearchText = searchText.ToLowerInvariant();

        return recipes.Values
            .Where(recipe => recipe.Title.ToLowerInvariant().Contains(normalizedSearchText))
            .ToList();
    }

    public IReadOnlyList<Recipe> SearchByIngredient(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            return new List<Recipe>();
        }

        string normalizedSearchText = searchText.ToLowerInvariant();

        return recipes.Values
            .Where(recipe => recipe.Ingredients.Any(ingredient =>
                ingredient.ToLowerInvariant().Contains(normalizedSearchText)))
            .ToList();
    }

    public IReadOnlyList<Recipe> GetHighestProteinRecipes(int count)
    {
        if (count <= 0)
        {
            return new List<Recipe>();
        }

        return recipes.Values
            .Where(recipe => recipe.Nutrition?.ProteinG.HasValue == true)
            .OrderByDescending(recipe => recipe.Nutrition!.ProteinG)
            .Take(count)
            .ToList();
    }

    public bool AddSavedRecipe(int recipeId)
    {
        if (!recipes.ContainsKey(recipeId))
        {
            return false;
        }

        return savedRecipes.Add(recipeId);
    }

    public bool RemoveSavedRecipe(int recipeId) =>
        savedRecipes.Remove(recipeId);

    public bool IsRecipeSaved(int recipeId) =>
        savedRecipes.Contains(recipeId);

    public IReadOnlyList<int> GetSavedRecipes() =>
        new List<int>(savedRecipes);
}
