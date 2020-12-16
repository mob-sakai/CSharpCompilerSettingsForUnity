using UnityEngine;
using UnityEngine.UI;

public class Logging : MonoBehaviour
{
    [SerializeField] private Text m_Text;

    void Start()
    {
        // C# 9.0 feature: Target-typed new.
        IgnoreAccessibility lib = new();

        // C# 9.0 feature: Record.
        Person p = new("Foo", "Bar");

        m_Text.text = $"IgnoreAccessibility.GetNumber = {lib.GetNumber()}\nRecord = {p}";
        m_Text.color = Color.green;
    }
}


public record Person
{
    public string LastName { get; }
    public string FirstName { get; }
    public Person(string first, string last) => (FirstName, LastName) = (first, last);
}
