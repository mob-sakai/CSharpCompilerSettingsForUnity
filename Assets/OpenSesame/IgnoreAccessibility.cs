using UnityEngine;
using UnityEngine.UI;

namespace OpenSesameTests
{
    public class IgnoreAccessibility : MonoBehaviour
    {
        private void Start()
        {
            var text = GetComponent<Text>();
#if IGNORE_ACCESSIBILITY
            var list = ListPool<int>.Get();

            list.Add(1);
            list.Add(2);

            ListPool<int>.Release(list);

            text.text = $"IGNORE_ACCESSIBILITY is included.";
            text.color = Color.green;
#else
            text.text = $"IGNORE_ACCESSIBILITY is not included.";
            text.color = Color.red;
#endif
        }
    }
}
