namespace K4os.Json.Messages.Interfaces
{
	public interface IRequest: IMessage { }

	public interface IRequest<TResponse>: IRequest where TResponse: IResponse { }
}
