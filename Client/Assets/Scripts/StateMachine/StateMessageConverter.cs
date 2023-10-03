

public interface IStateMessage
{

}

public struct NoStateMessage : IStateMessage
{
	
}

public static class StateMessageConverter
{
	public static FrameEntityMessage ConvertToEntity(this FrameInputSnapShotMessage snapShotMessage)
	{
		return snapShotMessage.playerEntityMessage;
    }

	public static FrameEntityAnimationMessage ConvertToAnimationMessage(this FrameInputSnapShotMessage snapShotMessage)
	{
		return snapShotMessage.ConvertToEntity().ConvertToAnimationMessage();
	}

	public static FrameEntityAnimationMessage ConvertToAnimationMessage(this FrameEntityMessage entityMessage)
	{
		return entityMessage.animationMessage;
	}

	public static FrameInputMessage ConvertToInput(this FrameInputSnapShotMessage snapShotMessage)
	{
		return snapShotMessage.playerInputMessage;
    }
}