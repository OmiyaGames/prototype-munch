using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(ColoredGUISkin))]
public class StartGoalGui : MonoBehaviour
{
	public enum GuiState
	{
		None,
		Start,
		Goal
	}
	
	public float boxWidthRatio = 0.5f;
	public float boxHeightRatio = 0.5f;
	public float buttonHeightRatio = 0.3f;
	public float buttonXMarginPixel = 0.4f;
	public float buttonYMarginPixel = 0.3f;
	public Color primaryGuiColor = Color.cyan;
	public Color secondaryGuiColor = Color.blue;
	
	public GuiState State
	{
		get
		{
			return mState;
		}
	}
	
	private GuiState mState = GuiState.Start;
	private Rect mCacheRect = new Rect(0, 0, 0, 0);
	private bool mGuiColorUpdated = false;
	
	// Use this for initialization
	void Start ()
	{
		collider.isTrigger = true;
		collider.enabled = true;
	}
	
	// Update is called once per frame
	void OnGUI ()
	{
		if(mGuiColorUpdated == false)
		{
			ColoredGUISkin.Instance.UpdateGuiColors(primaryGuiColor, secondaryGuiColor);
			mGuiColorUpdated = true;
		}
		else
		{
			switch(mState)
			{
			case GuiState.Start:
			{
				GUI.skin = ColoredGUISkin.Skin;
				
				// Render box
				mCacheRect.xMin = (Screen.width - (Screen.width * boxWidthRatio)) / 2f;
				mCacheRect.xMax = (Screen.width + (Screen.width * boxWidthRatio)) / 2f;
				mCacheRect.yMin = (Screen.height - (Screen.height * boxHeightRatio)) / 2f;
				mCacheRect.yMax = (Screen.height + (Screen.height * boxHeightRatio)) / 2f;
				GUI.Box(mCacheRect, "Navigate the maze to the goal!\n\nControls:\nUse Arrow keys or WASD to move.");
				
				// Render the button
				float rightMostEdge = mCacheRect.xMax - buttonXMarginPixel;
				mCacheRect.xMin += buttonXMarginPixel;
				mCacheRect.xMax = ((Screen.width - buttonXMarginPixel) / 2f);
				mCacheRect.yMax -= buttonYMarginPixel;
				mCacheRect.yMin = mCacheRect.yMax - (Screen.height * buttonHeightRatio);
				if(GUI.Button(mCacheRect, "Start"))
				{
					mState = GuiState.None;
				}
				mCacheRect.xMin = (mCacheRect.xMax + buttonXMarginPixel);
				mCacheRect.xMax = rightMostEdge;
				if(GUI.Button(mCacheRect, "Quit"))
				{
					Application.Quit();
				}
				break;
			}
			case GuiState.Goal:
			{
				GUI.skin = ColoredGUISkin.Skin;
				
				// Render box
				mCacheRect.xMin = (Screen.width - (Screen.width * boxWidthRatio)) / 2f;
				mCacheRect.xMax = (Screen.width + (Screen.width * boxWidthRatio)) / 2f;
				mCacheRect.yMin = (Screen.height - (Screen.height * boxHeightRatio)) / 2f;
				mCacheRect.yMax = (Screen.height + (Screen.height * boxHeightRatio)) / 2f;
				GUI.Box(mCacheRect, "Goal!\n\nDeveloper: Taro Omiya\nMusic: Canopy (by DST)\nSound: Gravel Footsteps (by Bleep Bloop Audio)\nArt:\nTeddy Bear, Radar Tower, and Trees (by Unity 3D)\nMia (by Mixamo)\nRubble and Steel textures (by TLC Indie)\nSculptures (VIS Games)\nAtom, Bucky, Split Metal, Wheel, and Wooden Balls (Steve Wigley)");
				
				// Render the button
				float rightMostEdge = mCacheRect.xMax - buttonXMarginPixel;
				mCacheRect.xMin += buttonXMarginPixel;
				mCacheRect.xMax = ((Screen.width - buttonXMarginPixel) / 2f);
				mCacheRect.yMax -= buttonYMarginPixel;
				mCacheRect.yMin = mCacheRect.yMax - (Screen.height * buttonHeightRatio);
				if(GUI.Button(mCacheRect, "Restart"))
				{
					Application.LoadLevel(Application.loadedLevel);
				}
				mCacheRect.xMin = (mCacheRect.xMax + buttonXMarginPixel);
				mCacheRect.xMax = rightMostEdge;
				if(GUI.Button(mCacheRect, "Quit"))
				{
					Application.Quit();
				}
				break;
			}
			}
		}
	}
	
	void OnTriggerEnter(Collider other)
	{
		if((mState == GuiState.None) && (other.CompareTag("Player") == true))
		{
			mState = GuiState.Goal;
		}
	}
}
