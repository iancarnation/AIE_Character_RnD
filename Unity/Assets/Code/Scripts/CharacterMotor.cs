using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
[Serializable]
public enum MovementTransferOnJump
{
	None,
	InitTransfer,
	PermaTransfer,
	PermaLocked
}
[AddComponentMenu("Character/Character Motor")]
[RequireComponent(typeof(CharacterController))]
[Serializable]
public class CharacterMotor : MonoBehaviour
{
	//[CompilerGenerated]
	//[Serializable]

	public bool canControl;
	public bool ghostControl = true;
	public int whocontroller = 1;
	public bool useFixedUpdate;
	[NonSerialized]
	public Vector3 inputMoveDirection;
	[NonSerialized]
	public bool inputJump;
	public CharacterMotorMovement movement;
	public CharacterMotorJumping jumping;
	public CharacterMotorMovingPlatform movingPlatform;
	public CharacterMotorSliding sliding;
	[NonSerialized]
	public bool grounded;
	[NonSerialized]
	public Vector3 groundNormal;
	private Vector3 lastGroundNormal;
	private Transform tr;
	private CharacterController controller;


	public CharacterMotor()
	{
		this.canControl = true;
		this.useFixedUpdate = true;
		this.inputMoveDirection = Vector3.zero;
		this.movement = new CharacterMotorMovement();
		this.jumping = new CharacterMotorJumping();
		this.movingPlatform = new CharacterMotorMovingPlatform();
		this.sliding = new CharacterMotorSliding();
		this.grounded = true;
		this.groundNormal = Vector3.zero;
		this.lastGroundNormal = Vector3.zero;
	}
	public void Awake()
	{
		this.controller = (CharacterController)this.GetComponent(typeof(CharacterController));
		this.tr = this.transform;
	}
	private void UpdateFunction()
	{
		Vector3 vector = this.movement.velocity;
		vector = this.ApplyInputVelocityChange(vector);
		vector = this.ApplyGravityAndJumping(vector);
		Vector3 vector2 = Vector3.zero;
		if (this.MoveWithPlatform())
		{
			Vector3 vector3 = this.movingPlatform.activePlatform.TransformPoint(this.movingPlatform.activeLocalPoint);
			vector2 = vector3 - this.movingPlatform.activeGlobalPoint;
			if (vector2 != Vector3.zero)
			{
				this.controller.Move(vector2);
			}
			Quaternion quaternion = this.movingPlatform.activePlatform.rotation * this.movingPlatform.activeLocalRotation;
			float y = (quaternion * Quaternion.Inverse(this.movingPlatform.activeGlobalRotation)).eulerAngles.y;
			if (y != (float)0)
			{
				this.tr.Rotate((float)0, y, (float)0);
			}
		}
		Vector3 position = this.tr.position;
		Vector3 vector4 = vector * Time.deltaTime;
		float num = Mathf.Max(this.controller.stepOffset, new Vector3(vector4.x, (float)0, vector4.z).magnitude);
		if (this.grounded)
		{
			vector4 -= num * Vector3.up;
		}
		this.movingPlatform.hitPlatform = null;
		this.groundNormal = Vector3.zero;
		this.movement.collisionFlags = this.controller.Move(vector4);
		this.movement.lastHitPoint = this.movement.hitPoint;
		this.lastGroundNormal = this.groundNormal;
		if (this.movingPlatform.enabled && this.movingPlatform.activePlatform != this.movingPlatform.hitPlatform && this.movingPlatform.hitPlatform != null)
		{
			this.movingPlatform.activePlatform = this.movingPlatform.hitPlatform;
			this.movingPlatform.lastMatrix = this.movingPlatform.hitPlatform.localToWorldMatrix;
			this.movingPlatform.newPlatform = true;
		}
		Vector3 vector5 = new Vector3(vector.x, (float)0, vector.z);
		this.movement.velocity = (this.tr.position - position) / Time.deltaTime;
		Vector3 vector6 = new Vector3(this.movement.velocity.x, (float)0, this.movement.velocity.z);
		if (vector5 == Vector3.zero)
		{
			this.movement.velocity = new Vector3((float)0, this.movement.velocity.y, (float)0);
		}
		else
		{
			float num2 = Vector3.Dot(vector6, vector5) / vector5.sqrMagnitude;
			this.movement.velocity = vector5 * Mathf.Clamp01(num2) + this.movement.velocity.y * Vector3.up;
		}
		if (this.movement.velocity.y < vector.y - 0.001f)
		{
			if (this.movement.velocity.y < (float)0)
			{
				this.movement.velocity.y = vector.y;
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		if (this.grounded && !this.IsGroundedTest())
		{
			this.grounded = false;
			if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
			{
				this.movement.frameVelocity = this.movingPlatform.platformVelocity;
				this.movement.velocity = this.movement.velocity + this.movingPlatform.platformVelocity;
			}
			//this.SendMessage("OnFall", 1);
			this.tr.position = (this.tr.position + num * Vector3.up);
		}
		else
		{
			if (!this.grounded && this.IsGroundedTest())
			{
				this.grounded = true;
				this.jumping.jumping = false;
				this.StartCoroutine_Auto(this.SubtractNewPlatformVelocity());
				//this.SendMessage("OnLand", 1);
			}
		}
		if (this.MoveWithPlatform())
		{
			this.movingPlatform.activeGlobalPoint = this.tr.position + Vector3.up * (this.controller.center.y - this.controller.height * 0.5f + this.controller.radius);
			this.movingPlatform.activeLocalPoint = this.movingPlatform.activePlatform.InverseTransformPoint(this.movingPlatform.activeGlobalPoint);
			this.movingPlatform.activeGlobalRotation = this.tr.rotation;
			this.movingPlatform.activeLocalRotation = Quaternion.Inverse(this.movingPlatform.activePlatform.rotation) * this.movingPlatform.activeGlobalRotation;
		}
	}
	public void FixedUpdate()
	{
		if (this.movingPlatform.enabled)
		{
			if (this.movingPlatform.activePlatform != null)
			{
				if (!this.movingPlatform.newPlatform)
				{
					Vector3 platformVelocity = this.movingPlatform.platformVelocity;
					this.movingPlatform.platformVelocity = (this.movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint) - this.movingPlatform.lastMatrix.MultiplyPoint3x4(this.movingPlatform.activeLocalPoint)) / Time.deltaTime;
				}
				this.movingPlatform.lastMatrix = this.movingPlatform.activePlatform.localToWorldMatrix;
				this.movingPlatform.newPlatform = false;
			}
			else
			{
				this.movingPlatform.platformVelocity = Vector3.zero;
			}
		}
		if (this.useFixedUpdate)
		{
			this.UpdateFunction();
		}
	}
	public void Update()
	{
		if (!this.useFixedUpdate)
		{
			this.UpdateFunction();
		}
	}
	private Vector3 ApplyInputVelocityChange(Vector3 velocity)
	{
		if (!this.canControl)
		{
			this.inputMoveDirection = Vector3.zero;
		}
		Vector3 vector = default(Vector3);
		if (this.grounded && this.TooSteep())
		{
			vector = new Vector3(this.groundNormal.x, (float)0, this.groundNormal.z).normalized;
			Vector3 vector2 = Vector3.Project(this.inputMoveDirection, vector);
			vector = vector + vector2 * this.sliding.speedControl + (this.inputMoveDirection - vector2) * this.sliding.sidewaysControl;
			vector *= this.sliding.slidingSpeed;
		}
		else
		{
			vector = this.GetDesiredHorizontalVelocity();
		}
		if (this.movingPlatform.enabled && this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer)
		{
			vector += this.movement.frameVelocity;
			vector.y = (float)0;
		}
		if (this.grounded)
		{
			vector = this.AdjustGroundVelocityToNormal(vector, this.groundNormal);
		}
		else
		{
			velocity.y = (float)0;
		}
		float num = this.GetMaxAcceleration(this.grounded) * Time.deltaTime;
		Vector3 vector3 = vector - velocity;
		if (vector3.sqrMagnitude > num * num)
		{
			vector3 = vector3.normalized * num;
		}
		if (this.grounded || this.canControl)
		{
			velocity += vector3;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min(velocity.y, (float)0);
		}
		return velocity;
	}
	private Vector3 ApplyGravityAndJumping(Vector3 velocity)
	{
		if (!this.inputJump || !this.canControl)
		{
			this.jumping.holdingJumpButton = false;
			this.jumping.lastButtonDownTime = (float)-100;
		}
		if (this.inputJump && this.jumping.lastButtonDownTime < (float)0 && this.canControl)
		{
			this.jumping.lastButtonDownTime = Time.time;
		}
		if (this.grounded)
		{
			velocity.y = Mathf.Min((float)0, velocity.y) - this.movement.gravity * Time.deltaTime;
		}
		else
		{
			velocity.y = this.movement.velocity.y - this.movement.gravity * Time.deltaTime;
			if (this.jumping.jumping && this.jumping.holdingJumpButton && Time.time < this.jumping.lastStartTime + this.jumping.extraHeight / this.CalculateJumpVerticalSpeed(this.jumping.baseHeight))
			{
				velocity += this.jumping.jumpDir * this.movement.gravity * Time.deltaTime;
			}
			velocity.y = Mathf.Max(velocity.y, -this.movement.maxFallSpeed);
		}
		if (this.grounded)
		{
			if (this.jumping.enabled && this.canControl && Time.time - this.jumping.lastButtonDownTime < 0.2f)
			{
				this.grounded = false;
				this.jumping.jumping = true;
				this.jumping.lastStartTime = Time.time;
				this.jumping.lastButtonDownTime = (float)-100;
				this.jumping.holdingJumpButton = true;
				if (this.TooSteep())
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.steepPerpAmount);
				}
				else
				{
					this.jumping.jumpDir = Vector3.Slerp(Vector3.up, this.groundNormal, this.jumping.perpAmount);
				}
				velocity.y = (float)0;
				velocity += this.jumping.jumpDir * this.CalculateJumpVerticalSpeed(this.jumping.baseHeight);
				if (this.movingPlatform.enabled && (this.movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer || this.movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
				{
					this.movement.frameVelocity = this.movingPlatform.platformVelocity;
					velocity += this.movingPlatform.platformVelocity;
				}
				//this.SendMessage("OnJump", 1);
			}
			else
			{
				this.jumping.holdingJumpButton = false;
			}
		}
		return velocity;
	}
	public void OnControllerColliderHit(ControllerColliderHit hit)
	{
		if (hit.normal.y > (float)0 && hit.normal.y > this.groundNormal.y && hit.moveDirection.y < (float)0)
		{
			if ((hit.point - this.movement.lastHitPoint).sqrMagnitude > 0.001f || this.lastGroundNormal == Vector3.zero)
			{
				this.groundNormal = hit.normal;
			}
			else
			{
				this.groundNormal = this.lastGroundNormal;
			}
			this.movingPlatform.hitPlatform = hit.collider.transform;
			this.movement.hitPoint = hit.point;
			this.movement.frameVelocity = Vector3.zero;
		}
	}
	private IEnumerator SubtractNewPlatformVelocity()
	{
		// When landing, subtract the velocity of the new ground from the character's velocity
		// since movement in ground is relative to the movement of the ground.
		if(movingPlatform.enabled &&
		   (movingPlatform.movementTransfer == MovementTransferOnJump.InitTransfer ||
		 movingPlatform.movementTransfer == MovementTransferOnJump.PermaTransfer))
		{
			// if we landed on a new platform, we have to wait for two FixedUpdates
			// before we know the velocity of the platform under the character
			if(movingPlatform.newPlatform)
			{
				Transform platform = movingPlatform.activePlatform;
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				if(grounded && platform == movingPlatform.activePlatform)
					yield break;
			}
			movement.velocity -= movingPlatform.platformVelocity;
		}
	}
	private bool MoveWithPlatform()
	{
		return (
			movingPlatform.enabled
			&& (grounded || movingPlatform.movementTransfer == MovementTransferOnJump.PermaLocked)
			&& movingPlatform.activePlatform != null
			);
	}
	private Vector3 GetDesiredHorizontalVelocity()
	{
		Vector3 vector = this.tr.InverseTransformDirection(this.inputMoveDirection);
		float num = this.MaxSpeedInDirection(vector);
		if (this.grounded)
		{
			float num2 = Mathf.Asin(this.movement.velocity.normalized.y) * 57.29578f;
			num *= this.movement.slopeSpeedMultiplier.Evaluate(num2);
		}
		return this.tr.TransformDirection(vector * num);
	}
	private Vector3 AdjustGroundVelocityToNormal(Vector3 hVelocity, Vector3 groundNormal)
	{
		Vector3 vector = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(vector, groundNormal).normalized * hVelocity.magnitude;
	}
	private bool IsGroundedTest()
	{
		return this.groundNormal.y > 0.01f;
	}
	public float GetMaxAcceleration(bool grounded)
	{
		return (!grounded) ? this.movement.maxAirAcceleration : this.movement.maxGroundAcceleration;
	}
	public float CalculateJumpVerticalSpeed(float targetJumpHeight)
	{
		return Mathf.Sqrt((float)2 * targetJumpHeight * this.movement.gravity);
	}
	public bool IsJumping()
	{
		return this.jumping.jumping;
	}
	public bool IsSliding()
	{
		bool arg_18_0;
		if (arg_18_0 = this.grounded)
		{
			arg_18_0 = this.sliding.enabled;
		}
		bool arg_25_0;
		if (arg_25_0 = arg_18_0)
		{
			arg_25_0 = this.TooSteep();
		}
		return arg_25_0;
	}
	public bool IsTouchingCeiling()
	{
		return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
	public bool IsGrounded()
	{
		return this.grounded;
	}
	public bool TooSteep()
	{
		return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
	}
	public Vector3 GetDirection()
	{
		return this.inputMoveDirection;
	}
	public void SetControllable(bool controllable)
	{
		this.canControl = controllable;
	}
	public float MaxSpeedInDirection(Vector3 desiredMovementDirection)
	{
		float arg_AC_0;
		if (desiredMovementDirection == Vector3.zero)
		{
			arg_AC_0 = (float)0;
		}
		else
		{
			float num = ((desiredMovementDirection.z <= (float)0) ? this.movement.maxBackwardsSpeed : this.movement.maxForwardSpeed) / this.movement.maxSidewaysSpeed;
			Vector3 normalized = new Vector3(desiredMovementDirection.x, (float)0, desiredMovementDirection.z / num).normalized;
			float num2 = new Vector3(normalized.x, (float)0, normalized.z * num).magnitude * this.movement.maxSidewaysSpeed;
			arg_AC_0 = num2;
		}
		return arg_AC_0;
	}
	public void SetVelocity(Vector3 velocity)
	{
		this.grounded = false;
		this.movement.velocity = velocity;
		this.movement.frameVelocity = Vector3.zero;
		//this.SendMessage("OnExternalVelocity");
	}
	public void Main()
	{
	}
}

[Serializable]
public class CharacterMotorJumping
{
	public bool enabled;
	public float baseHeight;
	public float extraHeight;
	public float perpAmount;
	public float steepPerpAmount;
	[NonSerialized]
	public bool jumping;
	[NonSerialized]
	public bool holdingJumpButton;
	[NonSerialized]
	public float lastStartTime;
	[NonSerialized]
	public float lastButtonDownTime;
	[NonSerialized]
	public Vector3 jumpDir;
	public CharacterMotorJumping()
	{
		this.enabled = true;
		this.baseHeight = 1f;
		this.extraHeight = 4.1f;
		this.steepPerpAmount = 0.5f;
		this.lastButtonDownTime = (float)-100;
		this.jumpDir = Vector3.up;
	}
}

[Serializable]
public class CharacterMotorMovement
{
	public float maxForwardSpeed;
	public float maxSidewaysSpeed;
	public float maxBackwardsSpeed;
	public AnimationCurve slopeSpeedMultiplier;
	public float maxGroundAcceleration;
	public float maxAirAcceleration;
	public float gravity;
	public float maxFallSpeed;
	[NonSerialized]
	public CollisionFlags collisionFlags;
	[NonSerialized]
	public Vector3 velocity;
	[NonSerialized]
	public Vector3 frameVelocity;
	[NonSerialized]
	public Vector3 hitPoint;
	[NonSerialized]
	public Vector3 lastHitPoint;
	public CharacterMotorMovement()
	{
		this.maxForwardSpeed = 10f;
		this.maxSidewaysSpeed = 10f;
		this.maxBackwardsSpeed = 10f;
		this.slopeSpeedMultiplier = new AnimationCurve(new Keyframe[]
		                                               {
			new Keyframe((float)-90, (float)1),
			new Keyframe((float)0, (float)1),
			new Keyframe((float)90, (float)0)
		});
		this.maxGroundAcceleration = 50f;
		this.maxAirAcceleration = 20f;
		this.gravity = 10f;
		this.maxFallSpeed = 20f;
		this.frameVelocity = Vector3.zero;
		this.hitPoint = Vector3.zero;
		this.lastHitPoint = new Vector3(float.PositiveInfinity, (float)0, (float)0);
	}
}
[Serializable]
public class CharacterMotorMovingPlatform
{

	public bool enabled;
	public MovementTransferOnJump movementTransfer;
	[NonSerialized]
	public Transform hitPlatform;
	[NonSerialized]
	public Transform activePlatform;
	[NonSerialized]
	public Vector3 activeLocalPoint;
	[NonSerialized]
	public Vector3 activeGlobalPoint;
	[NonSerialized]
	public Quaternion activeLocalRotation;
	[NonSerialized]
	public Quaternion activeGlobalRotation;
	[NonSerialized]
	public Matrix4x4 lastMatrix;
	[NonSerialized]
	public Vector3 platformVelocity;
	[NonSerialized]
	public bool newPlatform;
	public CharacterMotorMovingPlatform()
	{
		this.enabled = true;
		this.movementTransfer = MovementTransferOnJump.PermaTransfer;
	}
}
[Serializable]
public class CharacterMotorSliding
{
	public bool enabled;
	public float slidingSpeed;
	public float sidewaysControl;
	public float speedControl;
	public CharacterMotorSliding()
	{
		this.enabled = true;
		this.slidingSpeed = (float)15;
		this.sidewaysControl = 1f;
		this.speedControl = 0.4f;
	}
}
