using UnityEngine;
using System.Collections;

public class CGLabel : CGElement {
	
	public string text = "label";
	
	public override void Render(float UIScale){
		
		if(style != "") GUI.Label(GetAdjustedRect(UIScale),text,style);
		else GUI.Label(GetAdjustedRect(UIScale),text);
	}
}
