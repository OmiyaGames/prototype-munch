using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CGTexture : CGElement {
	
	public Texture2D texture;
	public ScaleMode scaleMode = ScaleMode.ScaleAndCrop;
	
	public override void Render(float UIScale){
		
		if(texture != null) GUI.DrawTexture(GetAdjustedRect(UIScale),texture,scaleMode);
	}
}
