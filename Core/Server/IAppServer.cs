namespace ModularSystem.Core
{
    public interface IAppServer
    {
        public void Run();
        public AppServerConfig Config { get; }
    }

    public class AppServerConfig
    {
        public int Port;
    }

}
