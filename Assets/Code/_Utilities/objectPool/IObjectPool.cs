using System;

public interface IObjectPool
{

    void Dispose();
    void GiveBackObject(int objHashCode);
    object GetObject();
}
