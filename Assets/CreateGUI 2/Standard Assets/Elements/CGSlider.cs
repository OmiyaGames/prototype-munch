using UnityEngine;
using System.Collections;

public class CGSlider : CGElement {
	
	public enum SliderOrientation{Horizontal,Vertical}
	public SliderOrientation sliderOrientation = SliderOrientation.Horizontal;
	public float sliderValue = 0;
	public float leftTopLimit = 0;
	public float rightBottomLimit = 10;
	public string thumbStyle = "";
	
	public override void Render(float UIScale){
		
		if(sliderOrientation == SliderOrientation.Horizontal){
			
			GUIStyle thumb = GUI.skin.horizontalSliderThumb;
			GUIStyle slider = GUI.skin.horizontalSlider;
			if(thumbStyle != "") thumb = GUI.skin.FindStyle(thumbStyle);
			if(style != "") slider = GUI.skin.FindStyle(style);
			
			sliderValue = GUI.HorizontalSlider(GetAdjustedRect(UIScale),sliderValue,leftTopLimit,rightBottomLimit,slider,thumb);
			
		}else{
			
			GUIStyle thumb = GUI.skin.verticalSliderThumb;
			GUIStyle slider = GUI.skin.verticalSlider;
			if(thumbStyle != "") thumb = GUI.skin.FindStyle(thumbStyle);
			if(style != "") slider = GUI.skin.FindStyle(style);
			
			sliderValue = GUI.VerticalSlider(GetAdjustedRect(UIScale),sliderValue,leftTopLimit,rightBottomLimit,slider,thumb);
		}
	}
}
