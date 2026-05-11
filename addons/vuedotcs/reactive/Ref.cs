using Godot;

namespace vuedotcs.reactive;

public partial class Ref : RefCounted
{
    private Variant _value;
    private Dep _dep = new Dep();

    public Ref(Variant initialValue)
    {
        _value = initialValue;
    }

    public Ref()
    {
    }

    public Variant Value
    {
        get
        {
            if (Effect.ActiveEffect != null)
                _dep.Depend(Effect.ActiveEffect);
            return _value;
        }
        set
        {
            if (!_value.Equals(value))
            {
                _value = value;
                _dep.Notify();
            }
        }
    }
}