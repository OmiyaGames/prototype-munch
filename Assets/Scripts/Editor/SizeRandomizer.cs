using UnityEngine;
using UnityEditor;

public class SizeRandomizer : EditorWindow
{
	private Vector2 sizeRange = Vector2.zero;
	
	[MenuItem ("Omiya Games/Size Randomizer")]
	private static void Init ()
	{
		// Get existing open window or if none, make a new one:
		EditorWindow.GetWindow<SizeRandomizer>();
	}
	
	private void OnGUI ()
	{
		Rect box = new Rect(3, 3, position.width - 6, 20);
		sizeRange = EditorGUI.Vector2Field(box, "Size range", sizeRange);
		box.y += 40;
		if(GUI.Button(box, "Resize Selected Objects") == true)
		{
			foreach(Transform selection in Selection.transforms)
			{
				if(selection != null)
				{
					ResizeObject(selection);
				}
			}
		}
		box.y += 26;
		if(GUI.Button(box, "Rotate Selected Objects") == true)
		{
			foreach(Transform selection in Selection.transforms)
			{
				if(selection != null)
				{
					RotateObject(selection);
				}
			}
		}
	}
	
	private void ResizeObject(Transform selection)
	{
		selection.localScale = Vector3.one * Random.Range(sizeRange.x, sizeRange.y);
	}
	
	private void RotateObject(Transform selection)
	{
		selection.localRotation = Quaternion.Euler(Vector3.up * Random.Range(0f, 360f));
	}
}