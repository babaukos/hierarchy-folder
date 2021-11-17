using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

[CustomEditor(typeof(Folder)), CanEditMultipleObjects]
public class FolderEditor : Editor 
{
	private static Texture2D _openFolderText;
	private static Texture2D _closedFolderText;

	private static List<int> folders = new List<int>();
	private static Dictionary <int, Folder> folderDict = new Dictionary<int, Folder>();

	private static Tool lastTool;
	private static Folder folder;
	

	[InitializeOnLoadMethod]
	private static void Startup()
	{
		// EditorGUIUtility.IconContent();
		_closedFolderText = EditorGUIUtility.Load("Folder Icon") as Texture2D;
		_openFolderText = EditorGUIUtility.Load("FolderEmpty Icon") as Texture2D;

		//EditorApplication.update += RefreshFolderIcons;	
		EditorApplication.hierarchyWindowChanged += PickFolder;
		EditorApplication.hierarchyWindowItemOnGUI += RefreshFolderIcons;
	}
	static void OnDestroy() 
	{
		EditorApplication.hierarchyWindowChanged -= PickFolder;
		EditorApplication.hierarchyWindowItemOnGUI -= RefreshFolderIcons;
	}
	private void OnEnable() 
	{
		folder = target as Folder;

		SetObjectIcon(folder, _closedFolderText);
		//SetObjectIcon(folder.gameObject, _closedFolderText);
		DisableIconInSceneView(folder, false);

		UnityEditorInternal.InternalEditorUtility.SetIsInspectorExpanded(folder, true);
		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

		//EditorApplication.RepaintHierarchyWindow();	
	}
	private static void EditorUpdate() 
	{
        var currentWindow = EditorWindow.mouseOverWindow;
        // if (currentWindow && currentWindow.GetType() == HierarchyWindowType) 
		// {
        //     if (!currentWindow.wantsMouseMove) {
        //         //allow the hierarchy window to use mouse move events!
        //         currentWindow.wantsMouseMove = true;
        //     }
        // }
		//  else 
		// {
        //     //_hoveredInstance = 0;
        // }
    }
	static void PickFolder()
	{
		folders.Clear();
		GameObject[] fold = FindObjectsOfType<GameObject>();
		for (int i = 0; i < fold.Length; i++)
		{
			Folder f = fold[i].GetComponent<Folder>();
			if(f != null)
			{
				folders.Add(fold[i].GetInstanceID());
			}
		}
	}
	static void RefreshFolderIcons(int instaceID, Rect selectionRect)
	{
        // place the icoon to the right of the list:
        Rect r = new Rect (selectionRect); 
		Rect selRect = new Rect (selectionRect.x - 25, selectionRect.y, selectionRect.height, selectionRect.width + 25); 
        GameObject obj = EditorUtility.InstanceIDToObject (instaceID) as GameObject;
		if(obj != null)
		{
			//Folder fold = obj.GetComponent<Folder>();
			if (folders.Contains(instaceID)) 
			{
				if (Selection.Contains(instaceID))
				{
					lastTool = Tools.current;
     				Tools.current = Tool.None;
				}
#if UNITY_5		
				r.x -= 17;
				GUI.DrawTexture(new Rect(r.x, r.y + 2, 16, 16), _openFolderText);
				//EditorGUI.Foldout(new Rect(r.x, r.y, 32, 32), true, "");
				//EditorGUI.LabelField (r, FolderContent); 
#elif UNITY_2018
				GUI.DrawTexture(new Rect(r.x -1, r.y - 1, 16, 17), _openFolderText);
#endif
				//EditorGUI.DrawRect(selectionRect,   new Color32 (56, 56, 56, 255));
				//EditorGUI.LabelField(r, openFolderContent);
				//EditorGUI.Foldout(r, true, openFolderContent, EditorStyles.foldout);
			}
		}
		//EditorApplication.RepaintHierarchyWindow();	
	}
	private static void SetObjectIcon( Object gObj, Texture2D texture) 
	{
        var ty = typeof( EditorGUIUtility );
        var mi = ty.GetMethod( "SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static );
        mi.Invoke( null, new object[] { gObj, texture } );
    }
    private static void DisableIconInSceneView(Object gObj, bool gizmosOn)
    {
		int val = gizmosOn ? 1 : 0;
        Assembly asm = Assembly.GetAssembly(typeof(Editor));
        System.Type type = asm.GetType("UnityEditor.AnnotationUtility");
        if (type != null) {
            MethodInfo getAnnotations = type.GetMethod("GetAnnotations", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo setGizmoEnabled = type.GetMethod("SetGizmoEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            MethodInfo setIconEnabled = type.GetMethod("SetIconEnabled", BindingFlags.Static | BindingFlags.NonPublic);
            var annotations = (System.Array)getAnnotations.Invoke(null, null);
            foreach (object annotation in annotations) 
			{
                System.Type annotationType = annotation.GetType();
                FieldInfo classIdField = annotationType.GetField("classID", BindingFlags.Public | BindingFlags.Instance);
                FieldInfo scriptClassField = annotationType.GetField("scriptClass", BindingFlags.Public | BindingFlags.Instance);
                if (classIdField != null && scriptClassField != null) 
				{
                    int classId = (int)classIdField.GetValue(annotation);
                    string scriptClass = (string)scriptClassField.GetValue(annotation);
                    //setGizmoEnabled.Invoke(null, new object[] { classId, scriptClass, val });
                    setIconEnabled.Invoke(null, new object[] { classId, scriptClass, val });
                }
            }
        }
    }
	
	private void ResetTransform(Transform tran)
	{
		tran.position = Vector3.zero;
		tran.rotation = Quaternion.identity;
		tran.localScale = Vector3.one;
	}
	private void HideEllemets(Transform tran, Folder com)
	{
		tran.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
		com.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
	}
	public override void OnInspectorGUI() 
	{
		GUI.DrawTexture(new Rect(4, 4, 32, 32),  _closedFolderText);
		EditorGUILayout.HelpBox("This is folder object", MessageType.Info);
	}   
    public static Texture2D ConvertToTexture(byte[] bytes, string name) 
	{
		var texture = new Texture2D(0, 0, TextureFormat.ARGB32, false, true);
		texture.LoadImage(bytes);
		texture.name = name;
		texture.alphaIsTransparency = true;
		texture.filterMode = FilterMode.Bilinear;
		texture.hideFlags = HideFlags.HideAndDontSave;
		texture.Apply();

		return texture;
	}
}
