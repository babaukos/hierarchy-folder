#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode, DisallowMultipleComponent]
public class Folder : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private Vector3 oldPos;
    [SerializeField, HideInInspector]
    private Quaternion oldRot;
    [SerializeField, HideInInspector]
    private Vector3 oldSc;

    public bool foldout = false;
    private void Awake()
    {
        HideEllemets(transform, this);
    }
    private void Update() 
    {
        ResetTransform (transform);
        RemoveAllComponent();
    }
    private void OnValidate()
    {
        
    }
    private void ResetTransform(Transform tran)
    {
        if(oldPos != tran.position || oldRot != tran.rotation || oldSc != tran.localScale)
        {
            tran.position = Vector3.zero;
            tran.rotation = Quaternion.identity;
            tran.localScale = Vector3.one;

            oldPos = tran.position;
            oldRot = tran.rotation;
            oldSc = tran.localScale;
        }
    }
    private void HideEllemets(Transform tran, Folder com)
	{
		tran.hideFlags = HideFlags.NotEditable | HideFlags.HideInInspector;
		com.hideFlags = HideFlags.NotEditable;
	}
    private void RemoveAllComponent()
    {
        Component[] allComp = GetComponents<Component>();
        for (int i = allComp.Length-1; i > 0; i--)
        {
            //Don't remove the Transform component
            if (!(allComp[i] is Transform))
            {
                //Don't remove this script until the loop has finished
                if (!(allComp[i] is Folder))
                {
                    DestroyImmediate(allComp[i]);
                }
            }
        }
        // for (var comp in allComp)
        // {
        //     //Don't remove the Transform component
        //     if (!(comp is Transform))
        //     {
        //         //Don't remove this script until the loop has finished
        //         if (!(comp is Folder))
        //         {
        //             DestroyImmediate(comp);
        //         }
        //     }
        // }
    }
    private void UnperentedChildren()
    {
        foreach(Transform child in transform)
        {
            child.SetParent(null);
        }
    }   
}
#endif