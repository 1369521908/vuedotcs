using System;
using Godot;
using Godot.Collections;
using vuedotcs.reactive;

namespace vuedotcs.api;

public partial class VueAPI : RefCounted
{
    // ---- Effect ----

    public static Effect CreateEffect(Callable fn)
    {
        var eff = new Effect(fn);
        eff.Run();
        return eff;
    }

    // ---- Ref ----

    public static Ref CreateRef(Variant initialValue)
    {
        return new Ref(initialValue);
    }

    // ---- Computed ----

    public static Ref Computed(Callable getter)
    {
        var result = CreateRef(new Variant());
        CreateEffect(Callable.From(() =>
        {
            result.Value = getter.Call();
        }));
        return result;
    }

    // ---- Bind ----

    public static void Bind(Variant nodes, Variant props, Ref r)
    {
        bool isNodeArray = nodes.VariantType == Variant.Type.Array;
        bool isPropArray = props.VariantType == Variant.Type.Array;

        if (!isNodeArray && !isPropArray)
        {
            // Node + String
            BindSingle(nodes.As<Node>(), props.AsString(), r);
        }
        else if (isNodeArray && !isPropArray)
        {
            // Array<Node> + String
            foreach (var node in nodes.As<Array<Node>>())
                BindSingle(node, props.AsString(), r);
        }
        else if (!isNodeArray && isPropArray)
        {
            // Node + Array<String>
            BindProps(nodes.As<Node>(), props.As<Array<string>>(), r);
        }
        else
        {
            // Array<Node> + Array<String>
            foreach (var node in nodes.As<Array<Node>>())
                BindProps(node, props.As<Array<string>>(), r);
        }
    }

    static void BindSingle(Node node, StringName prop, Ref r)
    {
        if (node == null)
        {
            GD.PushError("[VUEDOT:BIND_FAIL] 不是有效的节点");
            return;
        }
        if (prop == null)
        {
            GD.PushError("[VUEDOT:BIND_FAIL] 不是有效的属性");
            return;
        }
        var eff = CreateEffect(Callable.From(() =>
        {
            node.Set(prop, r.Value);
        }));
        if (GodotObject.IsInstanceValid(node))
            node.TreeExited += () => eff.Stop();
    }

    static void BindProps(Node node, Array<string> props, Ref r)
    {
        var eff = CreateEffect(Callable.From(() =>
        {
            foreach (var prop in props)
                node.Set(prop, r.Value);
        }));
        if (GodotObject.IsInstanceValid(node))
            node.TreeExited += () => eff.Stop();
    }

    // ---- On ----

    public static void On(Node node, StringName signalName, Callable fn)
    {
        if (!node.HasSignal(signalName))
        {
            GD.PushWarning($"[VUEDOT:ON_FAIL] {node.GetClass()} 不存在信号 {signalName}");
            return;
        }
        node.Connect(signalName, fn);
    }

    // ---- Hyper ----

    public static void Hyper(Node node, Callable fn)
    {
        if (node == null)
        {
            GD.PushWarning("[VUEDOT:HYPER_FAIL] 必须传入一个节点以标记副作用消失时机");
            return;
        }
        var eff = CreateEffect(fn);
        if (GodotObject.IsInstanceValid(node))
            node.TreeExited += eff.Stop;
    }

    // ---- Model ----

    public static void Model(Variant nodes, StringName prop, Ref r)
    {
        if (nodes.VariantType == Variant.Type.Array)
        {
            var nodeArr = nodes.As<Array<Node>>();
            foreach (var node in nodeArr)
                ModelSingle(node, prop, r);
        }
        else if (nodes.VariantType == Variant.Type.Object)
        {
            var singleNode = nodes.As<Node>();
            ModelSingle(singleNode, prop, r);
        }
    }

    static void BindModel(Node node, StringName prop, Ref r)
    {
        var eff = CreateEffect(Callable.From(() =>
        {
            var val = r.Value;
            if (node is Control ctrl && ctrl.HasFocus())
                return;
            if (!prop.IsEmpty)
                node.Set(prop, val);

        }));
        if (GodotObject.IsInstanceValid(node))
            node.TreeExited += () => eff.Stop();
    }

    static void ModelSingle(Node node, StringName prop, Ref r)
    {
        if (node == null)
        {
            GD.PushError("[VUEDOT:MODEL_FAIL] 不是有效的节点");
            return;
        }
        BindModel(node, prop, r);

        Action<Variant> setFn = (nv) => r.Value = nv;

        if (node is LineEdit lineEdit)
            lineEdit.TextChanged += (text) => r.Value = text;
        else if (node is TextEdit textEdit)
            textEdit.TextChanged += () => r.Value = textEdit.Text;
        else if (node is ColorPicker colorPicker)
            colorPicker.ColorChanged += (color) => r.Value = color;
    }

    // ---- Watch ----

    public static Effect Watch(Node node, Ref r, Callable callable)
    {
        var org = new Variant[] { r.Value };
        var weakRef = GodotObject.WeakRef(r);
        Effect eff = null;

        eff = CreateEffect(Callable.From(() =>
        {
            var target = weakRef.GetRef().As<Ref>();
            if (target == null)
            {
                eff?.Stop();
                return;
            }
            callable.Call(org[0], r.Value);
            org[0] = r.Value;
        }));

        if (node != null)
            node.TreeExited += eff.Stop;

        return eff;
    }

    // ---- Clean ----

    public static void Clean(ref Effect effect)
    {
        if (effect == null) return;
        effect.Stop();
        effect = null;
    }
}