using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum ENUM_ATTACK_KEY
{
	NONE, // 공격 안함
	ATTACK, // 일반 공격
	SKILL, // 스킬
	ULTIMATE, // 궁극기
	MAX 
}

public class FrameInputData
{
	public readonly int frameCount;

	public FrameInputData(int frameCount)
	{
		this.frameCount = frameCount;
	}	
}

public class MoveInputData : FrameInputData
{
	public readonly Vector2 moveVector = Vector2.zero; // 전후좌우 이동

	public MoveInputData() : base(0)
	{
		this.moveVector = default;
	}

	public MoveInputData(Vector2 moveVector, int frameCount) : base(frameCount)
	{
		this.moveVector = moveVector;
	}
}

public class AttackInputData : FrameInputData
{
	public readonly ENUM_ATTACK_KEY key;
	public readonly bool isPress = false;

	public AttackInputData(ENUM_ATTACK_KEY key, bool isPress, int frameCount) : base(frameCount)
	{
		this.key = key;
		this.isPress = isPress;
	}
}

public abstract class PressInputData : FrameInputData
{
	public readonly bool isPress = false; 

	public PressInputData(bool isPress, int frameCount) : base(frameCount)
	{
		this.isPress = isPress;
	}
}

public class DashInputData : PressInputData
{
	public DashInputData(bool isPress, int frameCount) : base(isPress, frameCount)
	{
	}
}

public class GuardInputData : PressInputData
{
	public GuardInputData(bool isPress, int frameCount) : base(isPress, frameCount)
	{
	}
}

public struct FrameSyncInputData
{
	public readonly Vector2 MoveInput;
	public readonly ENUM_ATTACK_KEY PressedAttackKey; 
	public readonly bool isDash;
	public readonly bool isGuard;
	public readonly int frameCount;

	public FrameSyncInputData(Vector2 moveInput, ENUM_ATTACK_KEY pressedAttackKey, bool isDash, bool isGuard, int frameCount)
	{
		this.MoveInput = moveInput;
		this.PressedAttackKey = pressedAttackKey;
		this.isDash = isDash;
		this.isGuard = isGuard;
		this.frameCount = frameCount;
	}
}

public interface IInputReceiver
{
	void OnInput(FrameSyncInputData resultInput);
}

public class InputManager : SingletonComponent<InputManager>
{
	public static float MoveThreshold { get; private set; } = 1;
	public static JoystickType JoystickType { get; private set; } = JoystickType.Fixed;
	public static float HandleRange { get; private set; } = 1;
	public static float DeadZone { get; private set; } = 0;
	public static AxisOptions AxisOptions { get; private set; } = AxisOptions.Both;
	public static bool SnapX { get; private set; } = true;
	public static bool SnapY { get; private set; } = true;

	private List<IInputReceiver> inputReceivers = new List<IInputReceiver>();
	private Queue<FrameInputData> inputDataQueue = new Queue<FrameInputData>();

	private FrameSyncInputData prevInputData = new FrameSyncInputData();

	protected override void OnAwakeInstance()
	{
		SceneManager.Instance.onSceneChanged += SetJoystick;
		SetJoystick();
	}

	private void SetJoystick(SceneType type = SceneType.Title)
	{
		var joyStick = FindObjectOfType<VariableJoystick>();
		if (joyStick != null)
		{
			joyStick.SetMode(JoystickType);
		}
	}

	protected override void OnReleaseInstance()
	{
		SceneManager.Instance.onSceneChanged -= SetJoystick;
	}

	public void RegisterInputReceiver(IInputReceiver inputReceiver)
	{
		if (inputReceivers.Contains(inputReceiver))
			return;

		inputReceivers.Add(inputReceiver);
	}

	public void UnregisterInputReceiver(IInputReceiver inputReceiver)
	{
		if (!inputReceivers.Contains(inputReceiver))
			return;

		inputReceivers.Remove(inputReceiver);
	}

	public void OnMoveInputChanged(Vector2 input, int frameCount)
	{
		float x = SnapX ? SnapFloat(input, input.x, AxisOptions.Horizontal) : input.x;
		float y = SnapY ? SnapFloat(input, input.y, AxisOptions.Vertical) : input.y;

		var inputData = new MoveInputData(new Vector2(x, y), frameCount);
		inputDataQueue.Enqueue(inputData);
	}

	public void OnDashInputChanged(bool isPress, int frameCount)
	{
		var inputData = new DashInputData(isPress, frameCount);
		inputDataQueue.Enqueue(inputData);
	}

	public void OnGuardInputChanged(bool isPress, int frameCount)
	{
		var inputData = new GuardInputData(isPress, frameCount);
		inputDataQueue.Enqueue(inputData);
	}

	public void OnAttackInputChanged(ENUM_ATTACK_KEY key, bool isAttack, int frameCount)
	{
		var inputData = new AttackInputData(key, isAttack, frameCount);
		inputDataQueue.Enqueue(inputData);
	}

	// 이 업데이트 문에서 인풋 큐를 모두 빼는데, 인풋을 받는 리시버의 상황에 따라 인풋이 무시될 수도 있다.
	// 프레임에 마지막으로 누른 인풋들만을 넣는 것으로 한다.
	public override void OnUpdate(int deltaFrameCount, float deltaTime)
	{
		int validFrameCount = Time.frameCount;

		Vector2 moveVec = prevInputData.MoveInput;
		ENUM_ATTACK_KEY pressedAttackKey = ENUM_ATTACK_KEY.MAX;
		bool isDash = false;
		bool isGuard = false;

		while (inputDataQueue.TryDequeue(out var result))
		{
			if (result.frameCount < validFrameCount)
				continue;

			if (result is MoveInputData moveInputResult)
			{
				moveVec = moveInputResult.moveVector;
			}
			else if (result is AttackInputData attackInputResult)
			{
				if (attackInputResult.isPress)
				{
					pressedAttackKey = attackInputResult.key;
				}
			}
			else if (result is PressInputData jumpInputResult)
			{
				isDash = jumpInputResult.isPress;
			}
			else if(result is GuardInputData guardInputData)
			{
				isGuard = guardInputData.isPress;
			}
		}

		prevInputData = new FrameSyncInputData(moveVec, pressedAttackKey, isDash, isGuard, validFrameCount);

		foreach (var receiver in inputReceivers)
		{
			receiver.OnInput(prevInputData);
		}
	}

	private float SnapFloat(Vector2 input, float value, AxisOptions snapAxis)
	{
		if (value == 0)
			return value;

		if (AxisOptions == AxisOptions.Both)
		{
			float angle = Vector2.Angle(input, Vector2.up);
			if (snapAxis == AxisOptions.Horizontal)
			{
				if (angle < 22.5f || angle > 157.5f)
					return 0;
				else
					return (value > 0) ? 1 : -1;
			}
			else if (snapAxis == AxisOptions.Vertical)
			{
				if (angle > 67.5f && angle < 112.5f)
					return 0;
				else
					return (value > 0) ? 1 : -1;
			}
			return value;
		}
		else
		{
			if (value > 0)
				return 1;
			if (value < 0)
				return -1;
		}
		return 0;
	}
}
