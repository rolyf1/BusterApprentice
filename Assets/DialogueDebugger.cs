// DialogueDebugger.cs ? attach to the GameObject that has the UIDocument (dialoguePanelGameObject)
using UnityEngine;
using UnityEngine.UIElements;
using System.Text;

public class DialogueDebugger : MonoBehaviour
{
    public UIDocument targetDocument; // leave empty to auto-find on this GameObject

    void Start()
    {
        if (targetDocument == null)
            targetDocument = GetComponent<UIDocument>();

        if (targetDocument == null)
        {
            Debug.LogError("[DialogueDebugger] No UIDocument found on this GameObject. Assign targetDocument or attach to UIDocument object.");
            return;
        }

        Debug.Log($"[DialogueDebugger] UIDocument found on GameObject '{targetDocument.gameObject.name}'. PanelSettings: {(targetDocument.panelSettings ? targetDocument.panelSettings.name : "null")}");

        var root = targetDocument.rootVisualElement;
        if (root == null)
        {
            Debug.LogError("[DialogueDebugger] rootVisualElement is null. UIDocument may not be initialized or the VisualTreeAsset is missing.");
            return;
        }

        // list top-level children
        var sb = new StringBuilder();
        foreach (var c in root.Children())
        {
            var classes = GetElementClassList(c);
            sb.AppendFormat("[{0} classes={1}]", string.IsNullOrEmpty(c.name) ? "(unnamed)" : c.name, classes);
        }
        Debug.Log("[DialogueDebugger] root children: " + sb.ToString());

        // attempt to find common fields
        string[] namesToCheck = new string[] { "DialoguePanel", "Panel", "Dialogue", "DialogueBox", "StartMenuRoot", "StartMenu" };
        foreach (var nm in namesToCheck)
        {
            var element = root.Q<VisualElement>(nm);
            Debug.Log($"[DialogueDebugger] Query root.Q<VisualElement>(\"{nm}\") => {(element == null ? "null" : element.name + " (classes=" + GetElementClassList(element) + ")")}");
            if (element != null)
            {
                Debug.Log($"   worldBound: {element.worldBound}  visible: {(element.visible ? "true" : "false")}  display: {element.style.display}");
            }
        }

        // try to find Speaker/Content labels by name and set test text
        var speaker = root.Q<Label>("Speaker");
        var content = root.Q<Label>("Content");
        Debug.Log($"[DialogueDebugger] Speaker found: {(speaker != null ? "yes" : "no")}, Content found: {(content != null ? "yes" : "no")}");

        if (speaker != null)
        {
            speaker.text = "DEBUG: Speaker visible?";
            speaker.style.minHeight = 24;
            speaker.style.minWidth = 200;
            speaker.style.unityBackgroundImageTintColor = new StyleColor(Color.clear);
        }
        if (content != null)
        {
            content.text = "DEBUG: Content visible? If you don't see this, something else is hiding the element (USS, z-order, or panel settings).";
            content.style.minHeight = 48;
            content.style.minWidth = 300;
        }

        // Force the most obvious visual changes on the first matching panel element we can find
        VisualElement panel = root.Q<VisualElement>("DialoguePanel") ?? root.Q<VisualElement>("Panel") ?? root;
        if (panel != null)
        {
            Debug.Log("[DialogueDebugger] Forcing panel visible/fallback styling and bringing to front.");
            panel.EnableInClassList("hide", false);
            panel.style.display = DisplayStyle.Flex;
            panel.style.minHeight = 100;
            panel.style.minWidth = 300;
            panel.style.backgroundColor = new StyleColor(Color.magenta);
            panel.BringToFront();
            Debug.Log($"[DialogueDebugger] panel.worldBound: {panel.worldBound}");
        }

        // if root looks tiny or offscreen, enlarge it temporarily
        if (root.worldBound.size.magnitude < 1f)
        {
            Debug.LogWarning("[DialogueDebugger] root worldBound size is very small ? forcing root min size for visibility.");
            root.style.minHeight = 200;
            root.style.minWidth = 400;
            root.style.display = DisplayStyle.Flex;
            root.BringToFront();
        }

        Debug.Log("[DialogueDebugger] Done diagnostics. If you still can't see the UI, paste the console output here.");
    }

    // ---------- Helper to read VisualElement class list using GetClasses() ----------
    private string GetElementClassList(VisualElement element)
    {
        if (element == null) return "(null)";
        // Direct API available in modern UI Toolkit: GetClasses() -> IEnumerable<string>
        var classes = element.GetClasses();
        if (classes == null) return "(none)";
        return string.Join(",", classes);
    }
}
