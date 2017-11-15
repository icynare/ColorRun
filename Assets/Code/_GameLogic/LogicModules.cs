public class LogicModules : Singleton<LogicModules>
{

    public override void Initialize()
    {
        BackController.Instance.Initialize();
        MapController.Instance.Initialize();
        GameOverController.Instance.Initialize();
    }

    public override void UnInitialize()
    {
        BackController.Instance.UnInitialize();
        MapController.Instance.UnInitialize();
        GameOverController.Instance.UnInitialize();
    }
}


