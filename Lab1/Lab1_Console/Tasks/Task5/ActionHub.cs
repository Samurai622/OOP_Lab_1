using System;
using System.Threading.Tasks;

namespace Lab1_Task5;

public sealed class ActionHub
{
    public Action ChangeOpacity { get; }
    public Action ChangeBackground { get; }
    public Func<Task> Hello { get; }

    public ActionHub(Action changeOpacity, Action changeBackground, Func<Task> hello)
    {
        ChangeOpacity = changeOpacity;
        ChangeBackground = changeBackground;
        Hello = hello;
    }
}
