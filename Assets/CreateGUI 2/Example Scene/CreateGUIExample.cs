using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class CreateGUIExample : MonoBehaviour {
	
	CreateGUI main;
	CGButton sampleButton;
	CGSlider sampleSlider;
	CGLabel sliderValue;
	public int sampleButtonClicks = 0;
	
	void Start () {
		
		main = GetComponent<CreateGUI>();
		sampleButton = (CGButton) main.FindElement("SampleButton");
		sampleSlider = (CGSlider) main.FindElement("SampleSlider");
		sliderValue = (CGLabel) main.FindElement("SliderValue");
	}
	
	void Update(){
		
		if(sampleButton.clicked) sampleButtonClicks++;
		sampleButton.text = "Clicks: "+sampleButtonClicks;
		sliderValue.text = "Value: "+sampleSlider.sliderValue;
	}
}
