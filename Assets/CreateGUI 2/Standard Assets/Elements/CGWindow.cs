using UnityEngine;
using System.Collections;

public class CGWindow : CGElement {
	
	public string title = "Window";
	int id = 0;
	public Rect dragWindow = new Rect(0,0,100,5);
	float scale = 1;
	
	public override void RecursiveRender(float UIScale){
		
		UpdateRenderBounds(scale);
		if(id == 0) id = GetHashCode();
		scale = UIScale;
		if(visible){
			if(skin != null) GUI.skin = skin;
			Rect orgRect = GetAdjustedRect(UIScale);
			Rect newRect = new Rect();
			if(style != "") newRect = GUI.Window(id,orgRect,RenderWindow,title,style);
			else newRect = GUI.Window(id,orgRect,RenderWindow,title);
			if(orgRect != newRect) rect = (new ScalingTool()).GetActualRect(UIScale,new Rect(0,0,Screen.width,Screen.height),newRect,relativePosition);
		}
	}
	
	public void RenderWindow(int windowID){
		Rect dragrect = (new ScalingTool()).GetActualRect(scale,rect,dragWindow,RelativePosition.TopLeft);
		GUI.DragWindow(dragrect);
		foreach(CGElement child in GetChildren()){
			if(child != null) child.RecursiveRender(scale);
			else GetChildren().Remove(child);
		}
	}
}