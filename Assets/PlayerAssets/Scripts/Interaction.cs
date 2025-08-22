using System.Text;
using UnityEngine;
using PlayerAssets;

[RequireComponent(typeof(Transform))]
public class PlayerAuraInteraction : MonoBehaviour
{
    [Header("Aura")]
    public float auraRadius = 3f;
    public LayerMask npcLayer; // set in inspector (use layer dropdown)

    [Header("Refs (optional - auto-find if null)")]
    public PlayerInputSystem playerInputSystem;

    [Header("Diagnostics")]
    public bool enableDebugLogs = true;
    [Tooltip("Use the non-alloc buffer (recommended). Turn off to use OverlapSphere for easier reading.")]
    public bool useNonAlloc = true;

    // buffer for non-alloc version
    private Collider[] _results = new Collider[32];

    public GameObject DialoguePanel;

    private void Awake()
    {
        // auto-find if not assigned
        if (playerInputSystem == null)
            playerInputSystem = PlayerInputSystem.Instance ?? FindObjectOfType<PlayerInputSystem>();

        if (enableDebugLogs)
        {
            Debug.Log($"[Aura] Awake: playerInputSystem={(playerInputSystem ? playerInputSystem.name : "null")}, auraRadius={auraRadius}, npcLayerMask={npcLayer.value} ({LayerMaskToString(npcLayer)})");
        }
    }

    private void Update()
    {
        // 1) find colliders
        int count = 0;
        if (useNonAlloc)
        {
            count = Physics.OverlapSphereNonAlloc(transform.position, auraRadius, _results, npcLayer, QueryTriggerInteraction.Collide);
        }
        else
        {
            var found = Physics.OverlapSphere(transform.position, auraRadius, npcLayer, QueryTriggerInteraction.Collide);
            count = found.Length;
            // copy into buffer for downstream code (safe)
            for (int i = 0; i < Mathf.Min(found.Length, _results.Length); i++) _results[i] = found[i];
        }

        if (enableDebugLogs)
        {
            if (count == 0)
            {
                Debug.Log($"[Aura] No NPC colliders found. auraRadius={auraRadius}, npcLayerMask={npcLayer.value} ({LayerMaskToString(npcLayer)})");
            }
            else
            {
                var sb = new StringBuilder();
                sb.Append($"[Aura] Found {count} colliders: ");
                for (int i = 0; i < count; i++)
                {
                    var c = _results[i];
                    if (c == null) continue;
                    sb.Append($"{c.name} (layer={LayerMask.LayerToName(c.gameObject.layer)}) ");
                }
                Debug.Log(sb.ToString());
            }
        }

        // 2) choose closest collider (if any)
        Collider best = null;
        float bestDistSqr = float.MaxValue;
        for (int i = 0; i < count; i++)
        {
            var col = _results[i];
            if (col == null) continue;
            if (!col.gameObject.activeInHierarchy) continue;

            // If collider belongs to the player itself, skip
            if (col.transform.IsChildOf(transform) || col.gameObject == gameObject) continue;

            float d = (col.ClosestPoint(transform.position) - transform.position).sqrMagnitude;
            if (d < bestDistSqr)
            {
                bestDistSqr = d;
                best = col;
            }
        }

        // 3) debug input state
        if (enableDebugLogs)
        {
            if (playerInputSystem == null)
            {
                Debug.LogWarning("[Aura] playerInputSystem is null ? input will not be detected. Assign it in inspector or ensure PlayerInputSystem.Instance exists.");
            }
            else
            {
                Debug.Log($"[Aura] inputToggle={playerInputSystem.inputToggle}, interact={playerInputSystem.interact}");
            }
        }

        // 4) attempt interaction when interact pressed
        if (playerInputSystem != null && playerInputSystem.inputToggle && playerInputSystem.interact)
        {
            if (best != null)
            {
                var npc = best.GetComponent<NPCInteractable>();
                if (npc != null)
                {
                    Debug.Log($"[Aura] Interacting with {npc.gameObject.name}");
                    npc.Interact();

                    DialoguePanel.SetActive(true);
                }
                else
                {
                    // Debug.Log($"[Aura] Closest collider {best.name} has no NPCInteractable component");
                }

                // consume the interact press so it doesn't retrigger instantly
                playerInputSystem.InteractInput(false);
            }
            else
            {
                // Debug.Log("[Aura] Interact pressed but no valid NPC in range.");
            }
        }

        // clear results when using non-alloc to avoid stale references if count < buffer length
        if (useNonAlloc)
        {
            for (int i = count; i < _results.Length; i++) _results[i] = null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 1f, 0.25f);
        Gizmos.DrawSphere(transform.position, auraRadius);
    }

    private string LayerMaskToString(LayerMask mask)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 32; i++)
        {
            if ((mask & (1 << i)) != 0)
            {
                sb.Append(LayerMask.LayerToName(i));
                sb.Append(",");
            }
        }
        return sb.Length > 0 ? sb.ToString().TrimEnd(',') : "(none)";
    }
}
