using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(CreateGUI)),CanEditMultipleObjects]
public class CreateGUIEditor : Editor{
	
	public static Color lightBlue = new Color(0,0.38f,0.81f,1);
	Vector2 scrollPosition_SelectedElement = new Vector2();
	Vector2 scrollPosition_ElementList = new Vector2();
	CGElement selectedElement;
	System.Type selectedElementType = typeof(CGButton);
	CreateGUI main;
	CGElement addChild = null;
	CGElement deleteThis = null;
	CGElement cloneThis = null;
	
	override public void OnInspectorGUI(){
		
		
		main = target as CreateGUI;
		
		if(main.root == null) main.OnEnable();
		
		GUI.SetNextControlName("unfocus");
		GUI.TextField(new Rect(-3,0,0,0),"");
		
		if(selectedElement == null) selectedElement = main.root;
		
		if(deleteThis != null){
			
			DestroyImmediate(deleteThis,true);
			AssetDatabase.SaveAssets();
			selectedElement = main.root;
		}
		if(addChild != null && selectedElement != null){
			addChild.SetParent(selectedElement);
			addChild = null;
		}
		if(cloneThis != null){
			cloneThis.GetParent().GetChildren().Add((CGElement)ScriptableObject.Instantiate(cloneThis));
			cloneThis = null;
		}
		
		GUIStyle selectedItemArea = new GUIStyle();
		selectedItemArea.fixedWidth = Screen.width/2;
		
		GUILayout.BeginHorizontal();
		scrollPosition_SelectedElement = GUILayout.BeginScrollView(scrollPosition_SelectedElement,selectedItemArea);
		
		if(selectedElement == main.root) GeneralOptions(main);
		else EditElement(selectedElement,main);
		
		GUILayout.EndScrollView();
		
		scrollPosition_ElementList = GUILayout.BeginScrollView(scrollPosition_ElementList,selectedItemArea);
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		
		GUIStyle createGUISelectButtonStyle = new GUIStyle(GUI.skin.FindStyle("TL tab left"));
		if(selectedElement == main.root) createGUISelectButtonStyle.normal.textColor = lightBlue;
		if(GUILayout.Button("CREATE GUI",createGUISelectButtonStyle)){
			if(Event.current.shift && selectedElement != main.root){
				Debug.LogWarning("The root cannot become the child of Element \""+selectedElement.name+"\"");
			}else{
				GUI.FocusControl("unfocus");
				selectedElement = main.root;
			}
		}
		GUIStyle popupStyle = new GUIStyle(GUI.skin.FindStyle("TL tab plus right"));
		popupStyle.fixedWidth = 20;
		string[] options = new string[main.elementTypes.Length+1];
		options[0] = "";
		int count = 0;
		foreach(System.Type elementType in main.elementTypes){
			count++;
			options[count] = elementType.Name;
		}
		int action = EditorGUILayout.Popup(0,options,popupStyle);
		if(action != 0)
		if(0 < action && action <= main.elementTypes.Length){
			CGElement addition = main.root.AddNew(main.elementTypes[action-1]);
			addition.name = "Element "+MonoScript.FindObjectsOfType(main.elementTypes[action-1]).Length;
		}
		
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		ListElements(main.root.GetChildren());
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
		GUILayout.EndScrollView();
		GUILayout.EndHorizontal();
		if(GUI.changed) main.transform.position = main.transform.position;
	}
	
	void GeneralOptions(CreateGUI main){
		
		GUILayout.BeginHorizontal("helpbox");
		GUILayout.Label("Create GUI","BoldLabel");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("helpbox");
		main.root.UIScale = EditorGUILayout.FloatField("UI Scale",main.root.UIScale);
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical("helpbox");
		GUILayout.Label("Note: To Parent an Element to the\nSelected Element, Hold Down the Shift Key\nand Click it");
		GUILayout.EndVertical();
		GUILayout.BeginVertical("helpbox");
		GUILayout.Label("Element Types:");
		GUILayout.Space(10);
		foreach(System.Type type in main.elementTypes){
			EditorGUILayout.LabelField(type.Name);
		}
		GUILayout.EndVertical();
		GUILayout.Space(30);
	}
	
	void EditElement(CGElement element,CreateGUI main){
		
		GUILayout.BeginHorizontal("helpbox");
		GUILayout.Label(element.GetType()+" - "+element.name+"","BoldLabel");
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal("helpbox");
		element.name = EditorGUILayout.TextField("Name",element.name);
		GUILayout.EndHorizontal();
		GUILayout.BeginVertical();
		FieldInfo [] fields = element.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
		foreach(FieldInfo fi in fields){
			if(!fi.IsNotSerialized){
				GUILayout.BeginHorizontal("helpbox");
				char[] propertyName = fi.Name.ToCharArray();
				if(propertyName.Length>0) propertyName[0] = char.ToUpper(propertyName[0]);
				SerializedObject tempSerializedObj = new SerializedObject(element);
				SerializedProperty targetProperty = tempSerializedObj.FindProperty(fi.Name);
				EditorGUILayout.PropertyField(targetProperty,true);
				tempSerializedObj.ApplyModifiedProperties();
				tempSerializedObj.Dispose();
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndVertical();
		GUILayout.Space(10);
	}
	
	void ListElements(List<CGElement> elements){
		
		foreach(CGElement e in elements){
			if(e != null){
				GUILayout.BeginHorizontal();
				int space = 19;
				if(e.GetChildren().Count == 0) space = 36;
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				GUILayout.BeginHorizontal();
				if(e.GetChildren().Count > 0){
					string expandButtonStyle = "OL Plus";
					if(e.GetFoldOut()) expandButtonStyle = "OL Minus";
					if(GUILayout.Button(" ",expandButtonStyle)) e.SetFoldOut(!e.GetFoldOut());
				}
				GUIStyle elementSelectButtonStyle = new GUIStyle(GUI.skin.FindStyle("TL tab left"));
				if(selectedElement == e){
					elementSelectButtonStyle.normal.textColor = lightBlue;
				} 
				if(GUILayout.Button(e.name,elementSelectButtonStyle)){
					if(Event.current.shift){
						if(selectedElement != e ){
							addChild = e;
						}
					}else{
						GUI.FocusControl("unfocus");
						selectedElement = e;
					}
				}
				GUIStyle popupStyle = new GUIStyle(GUI.skin.FindStyle("TL tab plus right"));
				popupStyle.fixedWidth = 20;
				string[] options = new string[main.elementTypes.Length+3];
				options[0] = "";
				int count = 0;
				foreach(System.Type elementType in main.elementTypes){
					count++;
					options[count] = elementType.Name;
				}
				options[count+1] = "Clone";
				options[count+2] = "Delete";
				
				int action = EditorGUILayout.Popup(0,options,popupStyle);
				if(action != 0)
				if(0 < action && action <= main.elementTypes.Length){
					CGElement addition = e.AddNew(main.elementTypes[action-1]);
					addition.name = "Element "+MonoScript.FindObjectsOfType(main.elementTypes[action-1]).Length;
				}else if(action == main.elementTypes.Length+1){
					cloneThis = e;
				}else if(action == main.elementTypes.Length+2){
					deleteThis = e;
				}
				
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				
				if(e!=null && e.GetFoldOut() && e.GetChildren().Count>0) ListElements(e.GetChildren());
				GUILayout.EndVertical();
				GUILayout.EndHorizontal();
			}
		}
	}
	
	public CGElement AddElement(string baseName,CGElement parent){
		
		CGElement element = ScriptableObject.CreateInstance(selectedElementType) as CGElement;
		Undo.RegisterCreatedObjectUndo(element,"CreateGUI Add Element");
		element.name = baseName;
		element.SetParentRaw(parent);
		parent.GetChildren().Add(element);
		return element;
	}
}
