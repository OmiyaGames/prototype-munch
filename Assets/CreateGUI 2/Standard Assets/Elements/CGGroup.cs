using UnityEngine;
using System.Collections;

public class CGGroup : CGElement {
	
	public override void RecursiveRender(float UIScale){
		
		if(visible){
			
			if(style != "") GUI.BeginGroup(GetAdjustedRect(UIScale),style);
			else GUI.BeginGroup(GetAdjustedRect(UIScale));
			UpdateRenderBounds(UIScale);
			foreach(CGElement child in GetChildren()){
				if(child != null) child.RecursiveRender(UIScale);
				else GetChildren().Remove(child);
			}
			GUI.EndGroup();
		}
	}
}
