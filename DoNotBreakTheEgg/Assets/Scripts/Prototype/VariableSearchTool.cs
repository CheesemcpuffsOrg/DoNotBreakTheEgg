#if UNITY_EDITOR
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class VariableSearchTool : EditorWindow
{
    private string searchTypeName = "int";
    private static string[] favoriteTypes;
    private static int selectedFavoriteIndex;

    [MenuItem("Tools/Hierarchy Variable Search")]
    public static void OpenWindow()
    {
        var window = GetWindow<VariableSearchTool>("Variable Search");
        window.Show();
    }

    private void OnEnable()
    {
        // Initialize the list of favorite types
        favoriteTypes = new string[] { "int", "float", "string", "Vector3" };  // Default favorites
        selectedFavoriteIndex = 0; // Default to the first item
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Search GameObjects by Variable Type", EditorStyles.boldLabel);

        // Dropdown for selecting from favorites
        selectedFavoriteIndex = EditorGUILayout.Popup("Favorites:", selectedFavoriteIndex, favoriteTypes);

        // Textfield for custom search
        searchTypeName = EditorGUILayout.TextField("Variable Name:", searchTypeName);

        if (GUILayout.Button("Search"))
        {
            TrySearch();
        }

        if (GUILayout.Button("Reset"))
        {
            ResetHierarchy();
        }

        EditorGUILayout.Space();

        // Allow adding a custom type to favorites
        if (GUILayout.Button("Add to Favorites"))
        {
            AddToFavorites(searchTypeName);
        }

        // Allow removing a custom type from favorites
        if (GUILayout.Button("Remove from Favorites"))
        {
            RemoveFromFavorites(searchTypeName);
        }
    }

    private void TrySearch()
    {
        // Use the selected favorite type or custom type name
        string typeToSearch = favoriteTypes[selectedFavoriteIndex];
        if (!string.IsNullOrEmpty(searchTypeName))
        {
            typeToSearch = searchTypeName;
        }

        Type searchType = ResolveType(typeToSearch);

        if (searchType == null)
        {
            Debug.LogError($"Type '{typeToSearch}' not found. Ensure it's a valid C# or Unity type.");
            return;
        }

        SearchByType(searchType);
    }

    private void SearchByType(Type searchType)
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (var obj in allObjects)
        {
            bool hasVariable = HasVariableOfType(obj, searchType);

            // Show objects with the variable, hide others
            obj.hideFlags = hasVariable ? HideFlags.None : HideFlags.HideInHierarchy;
        }

        Debug.Log($"Filtered hierarchy by type: {searchType}");
    }

    private bool HasVariableOfType(GameObject obj, Type searchType)
    {
        MonoBehaviour[] components = obj.GetComponents<MonoBehaviour>();

        foreach (var component in components)
        {
            if (component == null) continue; // Skip missing scripts

            FieldInfo[] fields = component.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fields.Any(field => field.FieldType == searchType))
                return true;
        }

        return false;
    }

    private void ResetHierarchy()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>(true);

        foreach (var obj in allObjects)
        {
            obj.hideFlags = HideFlags.None; // Show everything again
        }

        Debug.Log("Reset hierarchy visibility.");
    }

    private Type ResolveType(string typeName)
    {
        // Handle common C# and Unity types
        switch (typeName.ToLower())
        {
            case "int": return typeof(int);
            case "float": return typeof(float);
            case "bool": return typeof(bool);
            case "string": return typeof(string);
            case "double": return typeof(double);
            case "vector2": return typeof(Vector2);
            case "vector3": return typeof(Vector3);
            case "quaternion": return typeof(Quaternion);
            case "transform": return typeof(Transform);
        }

        // Try to resolve other Unity/C# types from assemblies
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .FirstOrDefault(t => t.Name == typeName || t.FullName == typeName);
    }

    private void AddToFavorites(string typeToAdd)
    {
        if (favoriteTypes.Contains(typeToAdd)) return; // Avoid duplicates
        Array.Resize(ref favoriteTypes, favoriteTypes.Length + 1);
        favoriteTypes[favoriteTypes.Length - 1] = typeToAdd;

        Debug.Log($"Added '{typeToAdd}' to favorites.");
    }

    private void RemoveFromFavorites(string typeToRemove)
    {
        int index = Array.IndexOf(favoriteTypes, typeToRemove);
        if (index < 0) return;

        // Remove the type and resize the array
        for (int i = index; i < favoriteTypes.Length - 1; i++)
        {
            favoriteTypes[i] = favoriteTypes[i + 1];
        }
        Array.Resize(ref favoriteTypes, favoriteTypes.Length - 1);

        Debug.Log($"Removed '{typeToRemove}' from favorites.");
    }

    // Automatically reset visibility when the window is closed
    private void OnDestroy()
    {
        ResetHierarchy();
    }
}
#endif
