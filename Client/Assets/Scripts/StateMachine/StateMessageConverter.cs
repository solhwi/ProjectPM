public interface IStateMessage
{

}

public static class StateMessageConverter
{
	public static FrameEntityMessage ConvertToEntity(this IStateMessage stateInfo)
	{
		if (stateInfo is FrameInputSnapShotMessage snapShotMessage)
		{
			return snapShotMessage.playerEntityMessage;
		}
		else if (stateInfo is FrameEntityMessage entityMessage)
		{
			return entityMessage;
		}

		return new FrameEntityMessage();
	}

	public static FrameEntityAnimationMessage ConvertToAnimationMessage(this IStateMessage stateInfo)
	{
		return stateInfo.ConvertToEntity().ConvertAnimationFrame();
	}

	public static FrameEntityAnimationMessage ConvertAnimationFrame(this FrameEntityMessage entityMessage)
	{
		return entityMessage.animationMessage;
	}

	public static FrameInputMessage ConvertToInput(this IStateMessage stateInfo)
	{
		if (stateInfo is FrameInputSnapShotMessage snapShotMessage)
		{
			return snapShotMessage.playerInputMessage;
		}
		else if (stateInfo is FrameInputMessage inputMessage)
		{
			return inputMessage;
		}

		return new FrameInputMessage();
	}
}