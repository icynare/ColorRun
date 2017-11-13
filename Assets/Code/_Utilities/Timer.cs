/// <summary>
/// 定时器模式
/// </summary>
public class Timer
{
    //  模式
    private int _count = 0;

    //  定时器时长
    private float _duration = 0.0f;

    //  剩余时间
    private float _leftTime = 0.0f;

    //  定时器委托
    private CallBack<object[]> _callback = null;

    //  参数列表
    private object[] _args = null;

    //是否不被Time.scale影响
    private bool _unScale = false;

    /// <summary>
    /// 初始化函数
    /// </summary>
    public void Initialize(int count, float duration, bool unScale, CallBack<object[]> handler, params object[] args)
    {
        _count = count;
        _duration = duration;
        _unScale = unScale;
        _leftTime = duration;
        _callback = handler;
        _args = args;
    }

    /// <summary>
    /// 运行事件
    /// </summary>
    public bool Run(float delta, float unScaleDelta)
    {
        if (null == _callback)
        {
            return false;
        }
        if (_unScale)
            _leftTime -= unScaleDelta;
        else
            _leftTime -= delta;
        if (_leftTime > 0.0f)
            return true;

        if (_count >= 0)
        {
            if ((1 == _count) || (0 == _count))
            {
                _callback(_args);

                // 通知删除定时器
                return false;
            }
            --_count;
        }

        _callback(_args);
        _leftTime += _duration;
        return true;
    }

    /// <summary>
    /// 重置
    /// </summary>
    public void Reset()
    {
        _count = 0;
        _duration = 0.0f;
        _leftTime = 0.0f;
        _unScale = false;
        _callback = null;
        _args = null;
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void Dispose()
    {
        Reset();
    }

    public void Finish()
    {
        _callback(_args);
        Reset();
    }
}
