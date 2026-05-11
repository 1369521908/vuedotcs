using Godot;
using vuedotcs.reactive;

namespace vuedotcs.api;

public partial class Vue : VueAPI
{
    public static Ref Ref(Variant val)
    {
        return CreateRef(val);
    }
    
    public new static Ref Computed(Callable getter)
    {
        return VueAPI.Computed(getter);
    }
    
    public new static void Bind(Variant node, Variant prop, Ref r)
    {
        VueAPI.Bind(node, prop, r);
    }
    
    public new static void On(Node node, StringName signalName, Callable fn)
    {
        VueAPI.On(node, signalName, fn);
    }
    
    public new static void Hyper(Node node, Callable fn)
    {
        VueAPI.Hyper(node, fn);
    }
    
    public new static void Model(Variant node, StringName prop, Ref r)
    {
        VueAPI.Model(node, prop, r);
    }
    
    public new static Effect Watch(Node node, Ref r, Callable fn)
    {
        return VueAPI.Watch(node, r, fn);
    }
    
    public new static void Clean(ref Effect effect)
    {
        VueAPI.Clean(ref effect);
    }
    
}