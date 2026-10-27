# Design

## 1. Saved recipe collection

For the saved recipe collection, I chose to use a HashSet<int> of recipe ids because :
- it has these methods: Add, Contains and Remove
- recipe ids are unique and a hash set prevents duplicate ids automatically
- it is better than a dictionary because it contains juste the useful information ie. the id. 

## 2. Integration

The new Part B features are integrated into the existing Part A `RecipeManager` because:
- the title and ingredient searches and the protein report use the values of the dictionary `recipes` that represent the recipes from the catalogue
- the hash set that represent saved recipes uses the same IDs as the keys of the dictionary

## 3. Basic complexity

| Operation | Structure | Expected complexity | Reason |
| --- | --- | --- | --- |
| Lookup recipe by ID | Dictionary | Average O(1) | Hash-based key lookup. |
| Traverse cooking plan | LinkedList | O(n) | Each planned recipe may need to be visited. |
| Complete next instruction | Queue | O(1) | The item at the front is removed. |
| LINQ title/ingredient search | Recipe collection | O(n) | Each recipe may need to be inspected. |
| Check whether a recipe is saved | Hash set | Average O(1) | The hash set uses the recipe ID to do a hash-based membership check |
