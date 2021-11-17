using UnityEngine;
using UnityEditor;

public static class CreateFolder
{
	[ContextMenu("Game Object/Create Folder", false, 8)]
	static void CreateFolderContext()
	{
		var folder = new GameObject("New Folder", typeof(Folder));
		Selection.activeGameObject = folder;
	}

	private const string _actionName = "Create Folder %#&N";
	/// <summary>Add new folder "prefab".</summary>
	/// <param name="command">Menu command information.</param>
	[MenuItem("GameObject/" + _actionName, isValidateFunction: false, priority: 0)]
	public static void AddFolderPrefab(MenuCommand command)
	{
		var obj = new GameObject("New Folder", typeof(Folder));

		Selection.activeGameObject = obj;
		GameObjectUtility.SetParentAndAlign(obj, (GameObject)command.context);
		Undo.RegisterCreatedObjectUndo(obj, _actionName);
	}
}
