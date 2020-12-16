using UnityEngine.UI;

public class IgnoreAccessibility
{
    public int GetNumber()
    {
        var l = ListPool<int>.Get();

        l.Add(1);
        l.Add(2);
        var count = l.Count;

        ListPool<int>.Release(l);
        return count;
    }
}
