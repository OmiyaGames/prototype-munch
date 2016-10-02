using UnityEngine;
using System.Collections;

public class CGScrollArea : CGElement {
	
	public Vector2 scrollPosition = new Vector2(0,0);
	public Rect viewArea = new Rect(0,0,10,10);
	public bool alwaysShowHorizontal = false;
	public bool alwaysShowVertical = false;
	
	/// <summary>verticalStyle also needs to be defined for this to work</summary>
	public string horizontalStyle = "";
	/// <summary>horizontalStyle also needs to be defined for this to work</summary>
	public string verticalStyle = "";
	
	public override void RecursiveRender(float UIScale){
		
		if(visible){
			
			Rect actualViewArea = (new ScalingTool()).GetAdjustedRect(UIScale,new Rect(0,0,Screen.width,Screen.height),viewArea,RelativePosition.TopLeft);
			if(horizontalStyle=="" || verticalStyle=="") scrollPosition = GUI.BeginScrollView(GetAdjustedRect(UIScale),scrollPosition,actualViewArea,alwaysShowHorizontal,alwaysShowVertical);
			else scrollPosition = GUI.BeginScrollView(GetAdjustedRect(UIScale),scrollPosition,actualViewArea,alwaysShowHorizontal,alwaysShowVertical,horizontalStyle,verticalStyle);
			UpdateRenderBounds(UIScale);
			foreach(CGElement child in GetChildren()){
				if(child != null) child.RecursiveRender(UIScale);
			}
			GUI.EndScrollView();
		}
	}
}
