public class LogicModules : Singleton<LogicModules>
{

    public override void Initialize()
    {
        BackController.Instance.Initialize();
    }

    public override void UnInitialize()
    {
        BackController.Instance.UnInitialize();
    }
}


