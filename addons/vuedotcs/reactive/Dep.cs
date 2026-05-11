using System.Collections.Generic;
using Godot;

namespace vuedotcs.reactive;

public partial class Dep : RefCounted
{
    private Dictionary<int, Effect> _subs = new();

    public void Depend(Effect effect)
    {
        if (effect == null) return;
        _subs[effect.Id] = effect;
        effect.AddDep(this);
    }

    public void Notify()
    {
        foreach (var effect in new List<Effect>(_subs.Values))
        {
            if (effect.Active)
                effect.Run();
        }
    }

    public void Remove(Effect effect)
    {
        _subs.Remove(effect.Id);
    }
}