using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class CharacterControls : MonoBehaviour
{
	public const float MovingThreshold = 0.2f;
	public float moveVelocity = 10f;
	public float DirectionDampTime = 0.25f;
	public AudioSource footsteps = null;
	public StartGoalGui guiState = null;
	
	private Vector3 mVelocity = Vector3.zero;
	private Animator mAnimation = null;
	private Quaternion mLookRotation = Quaternion.identity;
	private Vector3 mAngles;

	void Start()
	{
		mAnimation = GetComponent<Animator>();
		footsteps.Stop();
	}
	
	void FixedUpdate ()
	{
		// Check if we need to reload the game
		if(Input.GetKeyDown(KeyCode.Escape) == true)
		{
			Application.LoadLevel(Application.loadedLevel);
			return;
		}
		else if(guiState.State != StartGoalGui.GuiState.None)
		{
			// Stop sound
			if(footsteps.isPlaying == true)
			{
				footsteps.Stop();
			}
			mAnimation.SetFloat("Speed", 0);
			mAnimation.SetFloat("Direction", 0, DirectionDampTime, Time.deltaTime);
			return;
		}
		
		// Get the forward direction
		Vector3 characterForwardDirection = Camera.main.transform.forward;
		Vector3 characterRightDirection = Camera.main.transform.right;
		characterForwardDirection.y = 0;
		characterRightDirection.y = 0;
		characterForwardDirection.Normalize();
		characterRightDirection.Normalize();
		
		// Set the axis in this particular direction
		characterForwardDirection *= Input.GetAxis("Vertical");
		characterRightDirection *= Input.GetAxis("Horizontal");
		mVelocity = characterForwardDirection + characterRightDirection;
		
		// Update the animation
		float speed = mVelocity.magnitude;
		if(speed > MovingThreshold)
		{
			// Set the speed
			rigidbody.AddForce((mVelocity * moveVelocity), ForceMode.Impulse);
			mAnimation.SetFloat("Speed", speed);
			
			// Set the direction
			mVelocity.Normalize();
			mLookRotation = Quaternion.LookRotation(mVelocity);
			/*
			mAngles = Quaternion.LookRotation(transform.forward).eulerAngles - mLookRotation.eulerAngles;
			if((mAngles.y > -90) && (mAngles.y < 90))
			{
				mAnimation.SetFloat("Direction", (mAngles.y / 90), DirectionDampTime, Time.deltaTime);
			}
			else
			{
				mAnimation.SetFloat("Direction", 0, DirectionDampTime, Time.deltaTime);
			}
			*/
			mAnimation.SetFloat("Direction", 0);
			rigidbody.MoveRotation(mLookRotation);
			
			// Play sound
			if(footsteps.isPlaying == false)
			{
				footsteps.Play();
			}
		}
		else
		{
			mAnimation.SetFloat("Speed", 0);
			mAnimation.SetFloat("Direction", 0, DirectionDampTime, Time.deltaTime);
			
			// Stop sound
			if(footsteps.isPlaying == true)
			{
				footsteps.Stop();
			}
		}
	}
}
