using UnityEngine;
using System.Collections;

public class CGButton : CGElement {
	
	public string text = "button";
	public bool clicked = false;
	
	public override void Render(float UIScale){
		
		if(style != "") clicked = GUI.Button(GetAdjustedRect(UIScale),text,style);
		else clicked = GUI.Button(GetAdjustedRect(UIScale),text);
	}
}
