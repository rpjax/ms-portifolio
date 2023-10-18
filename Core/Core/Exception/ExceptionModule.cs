namespace ModularSystem.Core;

public class ExceptionModule
{
    public static ExceptionModule Singleton { get; } = new ExceptionModule();
    public static AppException HandleException(Exception e) => Singleton.Handle(e);
    public static void AddHandler(ExceptionHandler handler) => Singleton.Handlers.Add(handler);

    public List<ExceptionHandler> Handlers { get; } = new List<ExceptionHandler>();

    public AppException Handle(Exception e)
    {
        try
        {
            var list = Handlers.FindAll(i => i.ShouldHanlde(e));
            if (list.Count > 0)
            {
                var handler = list[0];
                return handler.Handle(e);
            }
            return HandleDefaultException(e);
        }
        catch (Exception)
        {
            throw;
        }
    }

    ExceptionModule()
    {

    }

    AppException HandleDefaultException(Exception e)
    {
        return new AppException(e.Message, ExceptionCode.Internal);
    }
}

public abstract class ExceptionHandler
{
    public abstract bool ShouldHanlde(Exception e);
    public abstract AppException Handle(Exception e);
}

