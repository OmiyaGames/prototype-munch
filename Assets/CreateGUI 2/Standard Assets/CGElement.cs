using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public abstract class CGElement : ScriptableObject{
	#if UNITY_EDITOR
	[SerializeField]
	bool foldout = false;
	#endif
	[SerializeField]
	bool mouseOver = false;
	[SerializeField]
	bool beingDragged = false;
	/// <summary>Is this Element (and it's children) visible?</summary>
	public bool visible = true;
	/// <summary>Can this Element be Dragged Around?</summary>
	public bool draggable = false;
	/// <summary>Rect Dimensions in Percentages of Screen Size (Adjusted For Different Aspect Ratios)</summary>
	public Rect rect = new Rect(0,0,10,10);
	public RelativePosition relativePosition = RelativePosition.TopLeft;
	/// <summary>The Name of a Custom Style defined in the Assigned Skin</summary>
	public string style = "";
	public GUISkin skin;
	/// <summary>This Tool helps get Screen Coordinates from Rect Percentages</summary>
	ScalingTool scalingTool = new ScalingTool();
	[SerializeField]
	CGElement parent;
	[SerializeField]
	List<CGElement> children = new List<CGElement>();
	[SerializeField]
	Rect actualRenderBounds;
	
	/// <summary>This is called every GUI Update (if the Element and Elements above it in the Hierarchy are Visible) for most Elements</summary>
	public virtual void Render(float UIScale){}
	
	/// <summary>This is called every GUI Update (if the Element and Elements above it in the Hierarchy are Visible) for most Elements</summary>
	public virtual Rect DragOverride(Rect rect){
		
		foreach(CGElement child in children){
			rect = child.DragOverride(rect);
		}
		return rect;
	}
	
	/// <summary>This is to help get the Real Coordinates of Elements which have a Position that is Influenced by Elements above it in the Hierarchy such as a Group or Window</summary>
	public void UpdateRenderBounds(float UIScale){
		
		Rect adjustedRect = GetAdjustedRect(UIScale);
		Vector2 offsetOfCurrentArea = GUIUtility.GUIToScreenPoint(new Vector2(adjustedRect.x,adjustedRect.y));
		actualRenderBounds = new Rect(offsetOfCurrentArea.x,offsetOfCurrentArea.y,adjustedRect.width,adjustedRect.height);
	}
	
	/// <summary>Element and it's Children are Rendered if it is Visible (Other Essential Functions like UpdateRenderBounds can also be called)</summary>
	public virtual void RecursiveRender(float UIScale){
		
		if(visible){
			if(skin != null) GUI.skin = skin;
			UpdateRenderBounds(UIScale);
			Render(UIScale);
			foreach(CGElement child in children){
				if(child != null) child.RecursiveRender(UIScale);
				else GetChildren().Remove(child);
			}
		}
	}
	
	/// <summary>Get the  Coordinates of an Element (Adjusted for displacement caused by Elements above it in the Hierarchy)</summary>
	public Rect GetRect(float UIScale){
		
		Rect adjustedRect = GetAdjustedRect(UIScale);
		return new Rect(adjustedRect.x-actualRenderBounds.x,adjustedRect.y-actualRenderBounds.y,adjustedRect.width,adjustedRect.height);
	}
	
	/// <summary>An Element is removed from it's previous Parent and Parented to this Element</summary>
	public CGElement AddChild(CGElement element){
		
		if(element.GetParent() != null) element.GetParent().GetChildren().Remove(element);
		element.SetParentRaw(this);
		children.Add(element);
		return element;
	}
	
	/// <summary>New Element is Created through "ScriptableObject.CreateInstance(type) as CGElement" and it's skin set to that of the current element</summary>
	public CGElement AddNew(System.Type type){
		
		CGElement element = ScriptableObject.CreateInstance(type) as CGElement;
		element.SetParent(this);
		#if UNITY_EDITOR
		if(AssetDatabase.GetAssetPath(this).Length > 0){
				
			AssetDatabase.AddObjectToAsset(element,this);
			AssetDatabase.SaveAssets();
		}
		#endif
		element.skin = skin;
		return element;
	}
	
	/// <summary>The Coordinates of the Element without Adjustedment for displacement caused by Elements above it in the Hierarchy(see GetRect)</summary>
	public Rect GetAdjustedRect(float UIScale){
		
		return scalingTool.GetAdjustedRect(UIScale,new Rect(0,0,parent.GetActualRenderBounds().width,parent.GetActualRenderBounds().height),rect,relativePosition);
	}
	
	/// <summary>Get how much an Element was displaced by the Elements above it in the Hierarchy</summary>
	public Rect GetActualRenderBounds(){
		return actualRenderBounds;
	}
	/// <summary>Change how much an Element was displaced by the Elements above it in the Hierarchy</summary>
	public void SetActualRenderBounds(Rect arb){
		actualRenderBounds = arb;
	}
	#if UNITY_EDITOR
	public void SetFoldOut(bool fo){
		foldout = fo;
	}
	public bool GetFoldOut(){
		return foldout;
	}
	#endif
	
	/// <summary>Mouse is over this Element</summary>
	public bool GetMouseOver(){
		return mouseOver;
	}
	/// <summary>Is the Element currently being dragged</summary>
	public bool GetBeingDragged(){
		return beingDragged;
	}
	/// <summary>(Automatically Called) modifies the "beingDragged" Variable</summary>
	public void SetBeingDragged(bool bd){
		beingDragged = bd;
	}
	
	/// <summary>Returns the current Parent of this Element</summary>
	public CGElement GetParent(){
		if(parent == null) return null;
		return parent;
	}
	
	/// <summary>USE WITH CARE! changes the Parent of the Element without adjusting the Children of the previous and new parent Elements (see SetParent function)</summary>
	public void SetParentRaw(CGElement p){
		parent = p;
	}
	
	/// <summary>Modifies the Parent of the Element while adjusting the Children of the previous and new parent Elements</summary>
	public void SetParent(CGElement p){
		
		try{
			if(IsAboveInHierarchy(p)){
				
				p.GetParent().GetChildren().Remove(p);
				p.SetParentRaw(parent);
				parent.GetChildren().Add(p);
				SetParent(p);
			}else{
				p.GetChildren().Add(this);
				if(parent != null) parent.GetChildren().Remove(this);
				parent = p;
				#if UNITY_EDITOR
				foldout = true;
				#endif
			}
		}catch(System.Exception e){
			Debug.LogWarning("CreateGUI - \""+name+"\" could not set parent \""+p.name+"\" \n"+e.Message);
		}
	}
	
	/// <summary>Is this Element above the the specified Element (used in SetParent function)</summary>
	public bool IsAboveInHierarchy(CGElement element){
		
		if(element == null) return false;
		if(parent == element.GetParent()) return false;
		while(true){
			if(element.GetParent() == null) return false;
			if(element.GetParent() == this) return true;
			if(!element.GetParent().GetChildren().Contains(element)){
				break;
			}else element = element.GetParent();
		}
		return false;
	}
	
	public List<CGElement> GetChildren(){
		return children;
	}
	
	public void SetChildren(List<CGElement> c){
		children = c;
	}
	
	/// <summary>Find an Element with the following Name</summary>
	public static CGElement Find(string Name){
		
		foreach(CGElement element in ScriptableObject.FindObjectsOfType(typeof(CGElement))){
			if(element.name == Name) return element;
		}
		
		return null;
	}
}
