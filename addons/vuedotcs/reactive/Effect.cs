using System;
using System.Collections.Generic;
using Godot;

namespace vuedotcs.reactive;

public partial class Effect : RefCounted
{
    private static int _nextId = 0;
    public static Effect ActiveEffect = null;

    public int Id { get; } = _nextId++;
    public bool Active { get; private set; } = true;

    private Callable _fn;
    private bool _running = false;
    private List<WeakReference<Dep>> _deps = new();

    public Effect(Callable fn)
    {
        _fn = fn;
    }

    public Effect()
    {
    }

    public void Run()
    {
        if (!Active || _running) return;
        _running = true;

        CleanupDeps();

        var prev = ActiveEffect;
        ActiveEffect = this;
        _fn.Call();
        ActiveEffect = prev;

        _running = false;
    }

    public void AddDep(Dep dep)
    {
        foreach (var w in _deps)
        {
            if (w.TryGetTarget(out var existing) && existing == dep)
                return;
        }
        _deps.Add(new WeakReference<Dep>(dep));
    }

    public void CleanupDeps()
    {
        foreach (var w in _deps)
        {
            if (w.TryGetTarget(out var dep))
                dep.Remove(this);
        }
        _deps.Clear();
    }

    public void Stop()
    {
        if (Active)
        {
            Active = false;
            CleanupDeps();
        }
    }
}