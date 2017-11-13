
/*
 * 全局事件定义在EventName文件里，模块之内的事件在类里面创建一个LocalEvent枚举类型
 */

public enum EventName
{
    GEStart = -1,
    ReStart,
    ApplicationStart,
    GameLoop,
    GameLoopLaterUpdate,
	GameStart, //加载场景前
    LoginStart,
    IntoLastScene,
    SceneLoadBegin,

    //Button
    SpaceDown,


    //ui
    ShowView,
    CloseView,
    ShowBlock,
    CloseBlock,

    //New
    ShowBack,

    GECount,
    ReFocus,
    ForceReconnect
}

