using UnityEngine;
using System.Collections;

public class CGTextArea : CGElement {
	
	public string text = "text area";
	
	public override void Render(float UIScale){
		
		if(style != "") text = GUI.TextArea(GetAdjustedRect(UIScale),text,style);
		else text = GUI.TextArea(GetAdjustedRect(UIScale),text);
	}
}
