using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class FollowCharacter : MonoBehaviour
{
	public const float MovingThreshold = 0.1f;
	
	public Transform characterToFollow = null;
	public Renderer characterRenderer = null;
	public Camera lookCamera = null;
	public float maxTurnSpeed = 1f;
	public Animator animation = null;
	public Transform runCameraPosition = null;
	public float cameraLocalMovementSmoothFactor = 1;
	public float cameraLocalMovementTopSpeed = 1;
	public Renderer cameraCharacterRenderer = null;
	public float hideCameraCharacterSeconds = 1f;
	public AudioSource footsteps = null;
	public StartGoalGui guiState = null;
	
	private NavMeshAgent mAgent = null;
	private Vector3 mCacheVector = Vector3.zero;
	private Quaternion mCacheRotation = Quaternion.identity;
	private float mVelocity = 0;
	private Vector3 mLocalCameraMovementSpeed = Vector3.zero;
	private Vector3 mOriginalCameraLocalPosition = Vector3.zero;
	private StartGoalGui.GuiState mLastState = StartGoalGui.GuiState.Start;
	
	// Use this for initialization
	void Start ()
	{
		mAgent = GetComponent<NavMeshAgent>();
		mOriginalCameraLocalPosition = lookCamera.transform.localPosition;
		cameraCharacterRenderer.enabled = false;
		footsteps.Stop();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		if(guiState.State == StartGoalGui.GuiState.Start)
		{
			// Stop sound
			if(footsteps.isPlaying == true)
			{
				footsteps.Stop();
			}
			animation.SetFloat("Speed", 0);
			return;
		}
		else if(mLastState == StartGoalGui.GuiState.Start)
		{
			StartCoroutine(HideCharacter());
		}
		
		if(characterToFollow != null)
		{
			mAgent.SetDestination(characterToFollow.position);
		}
		
		if(animation != null)
		{
			mVelocity = mAgent.desiredVelocity.magnitude;
			if(mVelocity > MovingThreshold)
			{
				animation.SetFloat("Speed", mVelocity);
				
				// Play sound
				if(footsteps.isPlaying == false)
				{
					footsteps.Play();
				}
			}
			else
			{
				animation.SetFloat("Speed", 0);
				
				// Stop sound
				if(footsteps.isPlaying == true)
				{
					footsteps.Stop();
				}
			}
		}
		mLastState = guiState.State;
	}
	
	void Update()
	{
		mCacheVector = characterToFollow.position - transform.position;
		if(mCacheVector.sqrMagnitude <= (mAgent.stoppingDistance * mAgent.stoppingDistance))
		{
			// Rotate towards the character
			mCacheRotation = Quaternion.LookRotation(mCacheVector);
			mCacheVector = transform.rotation.eulerAngles;
			mCacheVector.y = Mathf.MoveTowardsAngle(mCacheVector.y, mCacheRotation.eulerAngles.y, (maxTurnSpeed * Time.deltaTime));
			transform.rotation = Quaternion.Euler(mCacheVector);
		}
	}
	
	void LateUpdate()
	{
		if((lookCamera != null) && (characterToFollow != null))
		{
			// Position the camera
			mCacheVector = mOriginalCameraLocalPosition;
			if(mAgent.desiredVelocity.magnitude > MovingThreshold)
			{
				mCacheVector = runCameraPosition.localPosition;
			}
			lookCamera.transform.localPosition = Vector3.SmoothDamp(lookCamera.transform.localPosition, mCacheVector, ref mLocalCameraMovementSpeed, cameraLocalMovementSmoothFactor, cameraLocalMovementTopSpeed, Time.deltaTime);
			
			// Adjust camera angle
			mCacheVector = lookCamera.transform.localRotation.eulerAngles;
			mCacheRotation = Quaternion.LookRotation(characterToFollow.position - lookCamera.transform.position);
			mCacheVector.x = Mathf.MoveTowardsAngle(mCacheVector.x, mCacheRotation.eulerAngles.x, (maxTurnSpeed * Time.deltaTime));
			lookCamera.transform.localRotation = Quaternion.Euler(mCacheVector);
		}
	}
	
	IEnumerator HideCharacter()
	{
		cameraCharacterRenderer.enabled = false;
		yield return new WaitForSeconds(hideCameraCharacterSeconds);
		cameraCharacterRenderer.enabled = true;
	}
}
