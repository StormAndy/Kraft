using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class RecipeJSONEditor : EditorWindow
{
    private const string recipesFolderPath = "Assets/Resources/Data/Recipes";

    private List<string> jsonFiles = new List<string>();
    private int selectedFileIndex = -1;
    private RecipeData currentRecipe;

    private Vector2 sidebarScroll;
    private Vector2 inspectorScroll;

    [MenuItem("Tools/Recipe JSON Editor")]
    public static void OpenWindow()
    {
        GetWindow<RecipeJSONEditor>("Recipe JSON Editor");
    }

    /// <summary> Loads all JSON files from the recipes directory </summary>
    private void OnEnable() => LoadRecipeJSONFiles();

    /// <summary> Loads recipe JSON files from the specified folder </summary>
    private void LoadRecipeJSONFiles()
    {
        if (!Directory.Exists(recipesFolderPath))
            Directory.CreateDirectory(recipesFolderPath);

        jsonFiles = new List<string>(Directory.GetFiles(recipesFolderPath, "*.json"));
    }

    /// <summary> Draws the editor window layout </summary>
    private void OnGUI()
    {
        EditorGUILayout.BeginHorizontal();

        // Sidebar
        DrawSidebar();

        // Inspector
        DrawInspector();

        EditorGUILayout.EndHorizontal();
    }

    /// <summary> Draws the sidebar for selecting JSON files </summary>
    private void DrawSidebar()
    {
        EditorGUILayout.BeginVertical(GUILayout.Width(200));
        EditorGUILayout.LabelField("Recipe Files", EditorStyles.boldLabel);

        sidebarScroll = EditorGUILayout.BeginScrollView(sidebarScroll);

        for (int i = 0; i < jsonFiles.Count; i++)
            if (GUILayout.Button(Path.GetFileName(jsonFiles[i]), selectedFileIndex == i ? EditorStyles.toolbarButton : EditorStyles.miniButton))
            {
                selectedFileIndex = i;
                LoadJsonFile(jsonFiles[i]);
            }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Refresh", GUILayout.Height(25)))
            LoadRecipeJSONFiles();

        EditorGUILayout.EndVertical();
    }

    /// <summary> Draws the inspector for editing the selected recipe </summary>
    private void DrawInspector()
    {
        EditorGUILayout.BeginVertical();

        if (currentRecipe != null)
        {
            EditorGUILayout.LabelField("Inspector", EditorStyles.boldLabel);

            inspectorScroll = EditorGUILayout.BeginScrollView(inspectorScroll);

            currentRecipe.uniqueID = EditorGUILayout.TextField("Unique ID", currentRecipe.uniqueID);
            currentRecipe.recipeName = EditorGUILayout.TextField("Recipe Name", currentRecipe.recipeName);
            currentRecipe.outputItemID = EditorGUILayout.TextField("Output Item ID", currentRecipe.outputItemID);
            currentRecipe.outputQuantity = EditorGUILayout.IntField("Output Quantity", currentRecipe.outputQuantity);

            // Ingredients
            DrawIngredients();

            if (GUILayout.Button("Save Recipe", GUILayout.Height(25)))
                SaveJsonFile(jsonFiles[selectedFileIndex]);

            EditorGUILayout.EndScrollView();
        }
        else
        {
            EditorGUILayout.LabelField("Select a recipe file to edit.");
        }

        EditorGUILayout.EndVertical();
    }

    /// <summary> Draws the ingredients list as text input fields </summary>
    private void DrawIngredients()
    {
        EditorGUILayout.LabelField("Ingredients", EditorStyles.boldLabel);

        if (currentRecipe.ingredients == null)
            currentRecipe.ingredients = new List<Ingredient>();

        for (int i = 0; i < currentRecipe.ingredients.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            currentRecipe.ingredients[i].itemID = EditorGUILayout.TextField($"Ingredient {i + 1} ID", currentRecipe.ingredients[i].itemID);
            currentRecipe.ingredients[i].quantity = EditorGUILayout.IntField("Quantity", currentRecipe.ingredients[i].quantity);

            if (GUILayout.Button("Remove", GUILayout.Width(60)))
            {
                currentRecipe.ingredients.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add Ingredient"))
            currentRecipe.ingredients.Add(new Ingredient { itemID = "", quantity = 1 });
    }

    /// <summary> Loads a recipe JSON file into the editor </summary>
    private void LoadJsonFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        currentRecipe = JsonUtility.FromJson<RecipeData>(json) ?? new RecipeData();
    }

    /// <summary> Saves the current recipe to its JSON file </summary>
    private void SaveJsonFile(string filePath)
    {
        string json = JsonUtility.ToJson(currentRecipe, true);
        File.WriteAllText(filePath, json);
        AssetDatabase.Refresh();
    }
}
