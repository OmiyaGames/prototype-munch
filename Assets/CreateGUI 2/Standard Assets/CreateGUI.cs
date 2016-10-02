using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

public enum RelativePosition{TopLeft,TopCenter,TopRight,MiddleLeft,MiddleCenter,MiddleRight,BottomLeft,BottomCenter,BottomRight}

[ExecuteInEditMode()]
public class CreateGUI : MonoBehaviour{
	
	public System.Type[] elementTypes = new System.Type[]{typeof(CGButton),typeof(CGGroup),typeof(CGLabel),typeof(CGScrollArea),typeof(CGScrollbar),typeof(CGSlider),typeof(CGTextArea),typeof(CGTextField),typeof(CGTexture),typeof(CGWindow)};
	public CGRoot root;
	#if UNITY_EDITOR
	public int loadPrefab = 0;
	
	public void OnEnable(){
		
		if(root == null){
			
			if(AssetDatabase.GetAssetPath(gameObject).Length == 0){
				
				loadPrefab = GetInstanceID();
				root = ScriptableObject.CreateInstance<CGRoot>();
				
			}else{
				
				CreateGUI source = null;
				
				foreach(CreateGUI cg in (CreateGUI[]) Object.FindObjectsOfType(typeof(CreateGUI))){
					
					if(cg.GetInstanceID() == loadPrefab){
						
						source = cg;
						break;
					}
				}
				
				if(source == null) root = ScriptableObject.CreateInstance<CGRoot>();
				else root = (CGRoot) Instantiate(source.root);
				
				string path = AssetDatabase.GetAssetPath(gameObject);
				path = path.TrimEnd(".prefab".ToCharArray());
				string[] splitpath = path.Split('/');
				path = path.TrimEnd(splitpath[splitpath.Length-1].ToCharArray());
				AssetDatabase.CreateFolder(path.TrimEnd('/'),name+"_CG");
				
				try{
					path = path+name+"_CG/"+splitpath[splitpath.Length-1]+"_CG.asset";
					AssetDatabase.CreateAsset(root,path);
					root.SetChildren(SaveRecursively(root.GetChildren(),path));
				}catch(System.Exception e){Debug.LogWarning("Create GUI could not create CreateGUI File ("+e.Message+")");} 
			}
		}
		
		if(AssetDatabase.GetAssetPath(gameObject).Length == 0){
			
			
			root = (CGRoot) Instantiate(root);
			try{
				root.SetChildren(SaveRecursively(root.GetChildren(),""));
			}catch(System.Exception){}
			loadPrefab = GetInstanceID();
		}
	}
	
	List<CGElement> SaveRecursively(List<CGElement> children, string save){
		
		List<CGElement> copies = new List<CGElement>();
		
		foreach(CGElement child in children){
			
			if(child != null){
				CGElement copy = (CGElement) Instantiate(child);
				copy.name = child.name;
				copies.Add(copy);
				if(save != "") AssetDatabase.AddObjectToAsset(copy,save);
				copy.SetChildren(SaveRecursively(copy.GetChildren(),save));
			}
		}
		return copies;
	}
	#endif
	
	void OnGUI(){
		#if UNITY_EDITOR
		
		#endif
		root.SetActualRenderBounds(new Rect(0,0,Screen.width,Screen.height));
		RenderElements(root.GetChildren());
		MouseOverElements(root.GetChildren());
	}
	
	void RenderElements(List<CGElement> elements){
		
		try{
			foreach(CGElement element in elements){
				
				if(element == null) elements.Remove(element);
				else{
					element.SetParentRaw(root);
					element.RecursiveRender(root.UIScale);
				}
			}
		}catch(System.InvalidOperationException){}
	}
	
	/// <summary>Update mouseOver,clicked and clickedDown Recursively</summary>
	void MouseOverElements(List<CGElement> elements){
		
		foreach(CGElement element in elements){
			
			if(element!=null) MouseOverElements(element.GetChildren());
		}
	}
	
	void GetAllElements(List<CGElement> source, List<CGElement> destination){
		
		foreach(CGElement element in source){
			
			if(element != null){
				destination.Add(element);
				GetAllElements(element.GetChildren(),destination);
			}
		}
	}
	
	public CGElement FindElement(string Name){
		
		return SearchElements(root,Name);
	}
	
	CGElement SearchElements(CGElement element,string Name){
		
		foreach(CGElement child in element.GetChildren()){
			
			if(child.name == Name) return child;
			else{
				CGElement result = SearchElements(child,Name);
				if(result != null) return result;
			}
		}
		return null;
	}
}

public class ScalingTool{
	//Converting Percentage Values to Actual Points
	public Rect GetAdjustedRect(float UIScale,Rect totalArea,Rect rect, RelativePosition relativePosition){
		
		float sf = GetScaleFactor(UIScale,totalArea);
		
		rect = new Rect(rect.x*sf,rect.y*sf,rect.width*sf,rect.height*sf);
		
		Vector2 position = new Vector2();
		
		switch(relativePosition){
		
			case RelativePosition.TopCenter:
				position = new Vector2(totalArea.width/2-rect.width/2,0);
				break;
				
			case RelativePosition.TopRight: 
				position = new Vector2(totalArea.width-rect.width,0);
				break;
				
			case RelativePosition.MiddleLeft: 
				position = new Vector2(0,totalArea.height/2-rect.height/2);
				break;
				
			case RelativePosition.MiddleCenter: 
				position = new Vector2(totalArea.width/2-rect.width/2,totalArea.height/2-rect.height/2);
				break;
				
			case RelativePosition.MiddleRight: 
				position = new Vector2(totalArea.width-rect.width,totalArea.height/2-rect.height/2);
				break;
				
			case RelativePosition.BottomLeft:
				position = new Vector2(0,totalArea.height-rect.height);
				break;
				
			case RelativePosition.BottomCenter:
				position = new Vector2(totalArea.width/2-rect.width/2,totalArea.height-rect.height);
				break;
				
			case RelativePosition.BottomRight:
				position = new Vector2(totalArea.width-rect.width,totalArea.height-rect.height);
				break;
		}
		
		position = new Vector2(position.x+rect.x,position.y+rect.y);
		
		return new Rect(position.x+totalArea.x,position.y+totalArea.y,rect.width,rect.height);
	}
	//Convert Actual Points to Percentage Values
	public Rect GetActualRect(float UIScale,Rect totalArea,Rect rect, RelativePosition relativePosition){
		
		Vector2 position = new Vector2();
		
		switch(relativePosition){
		
			case RelativePosition.TopCenter:
				position = new Vector2(totalArea.width/2-rect.width/2,0);
				break;
				
			case RelativePosition.TopRight: 
				position = new Vector2(totalArea.width-rect.width,0);
				break;
				
			case RelativePosition.MiddleLeft: 
				position = new Vector2(0,totalArea.height/2-rect.height/2);
				break;
				
			case RelativePosition.MiddleCenter:
				position = new Vector2(totalArea.width/2-rect.width/2,totalArea.height/2-rect.height/2);
				break;
				
			case RelativePosition.MiddleRight: 
				position = new Vector2(totalArea.width-rect.width,totalArea.height/2-rect.height/2);
				break;
				
			case RelativePosition.BottomLeft:
				position = new Vector2(0,totalArea.height-rect.height);
				break;
				
			case RelativePosition.BottomCenter:
				position = new Vector2(totalArea.width/2-rect.width/2,totalArea.height-rect.height);
				break;
				
			case RelativePosition.BottomRight:
				position = new Vector2(totalArea.width-rect.width,totalArea.height-rect.height);
				break;
		}
		
		position = new Vector2(rect.x-position.x,rect.y-position.y);
		
		float sf = GetScaleFactor(UIScale,totalArea);
		
		return new Rect(position.x/sf,position.y/sf,rect.width/sf,rect.height/sf);
	}
	
	public float GetScaleFactor(float UIScale,Rect totalArea){
		
		return (totalArea.width+totalArea.height)/200*UIScale;
	}

}
