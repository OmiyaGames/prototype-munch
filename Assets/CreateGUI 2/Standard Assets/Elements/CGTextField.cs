using UnityEngine;
using System.Collections;

public class CGTextField : CGElement {
	
	public string text = "text field";
	
	public override void Render(float UIScale){
		
		if(style != "") text = GUI.TextField(GetAdjustedRect(UIScale),text,style);
		else text = GUI.TextField(GetAdjustedRect(UIScale),text);
	}
}
