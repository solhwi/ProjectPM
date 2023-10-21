
public interface ICommand
{

}

public static class CommandHelper
{
	public static FrameCommandMessage ToFrameMessage(this ICommand command)
	{
		return (FrameCommandMessage)command;
	}

	public static FrameEntityMessage ToEntity(this FrameCommandMessage snapShotMessage)
	{
		return snapShotMessage.playerEntityMessage;
    }

	public static float ToNormalizeTime(this FrameCommandMessage snapShotMessage)
	{
		return snapShotMessage.ToEntity().ToNormalizeTime();
	}

	public static float ToNormalizeTime(this FrameEntityMessage entityMessage)
	{
		return entityMessage.normalizedTime;
	}

	public static FrameInputMessage ToInput(this FrameCommandMessage snapShotMessage)
	{
		return snapShotMessage.playerInputMessage;
    }
}