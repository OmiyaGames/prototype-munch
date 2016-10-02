using UnityEngine;
using System.Collections;

public class CGScrollbar : CGElement {
	
	public enum ScrollbarOrientation{Horizontal,Vertical}
	public ScrollbarOrientation scrollbarOrientation = ScrollbarOrientation.Horizontal;
	public float scrollbarValue = 0;
	public float size = 3;
	public float leftTopLimit = 0;
	public float rightBottomLimit = 10;
	public string thumbStyle = "";
	
	public override void Render(float UIScale){
		
		if(scrollbarOrientation == ScrollbarOrientation.Horizontal){
			
			GUIStyle scrollbar = GUI.skin.horizontalScrollbar;
			if(style != "") scrollbar = GUI.skin.FindStyle(style);
			scrollbarValue = GUI.HorizontalScrollbar(GetAdjustedRect(UIScale),scrollbarValue,size,leftTopLimit,rightBottomLimit,scrollbar);
			
		}else{
			
			GUIStyle scrollbar = GUI.skin.verticalScrollbar;
			if(style != "") scrollbar = GUI.skin.FindStyle(style);
			scrollbarValue = GUI.VerticalScrollbar(GetAdjustedRect(UIScale),scrollbarValue,size,leftTopLimit,rightBottomLimit,scrollbar);
		}
	}
}
