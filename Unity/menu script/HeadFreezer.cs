using UnityEngine;

/// <summary>
/// يضغط اللاعب Esc لتحرير الماوس وإيقاف الكاميرا (Freeze Head).
/// تم تحديث هذا السكريبت ليكون متزامناً بالكامل مع PlayerController.
/// </summary>
public class HeadFreezer : MonoBehaviour
{
    private Behaviour cinemachineBrain;
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();

        // البحث عن عقل السينما-ماشين في الكاميرا الأساسية برمجياً لتجنب مشكلة الـ Assembly
        if (Camera.main != null)
        {
            Component[] components = Camera.main.GetComponents<Component>();
            foreach (Component comp in components)
            {
                if (comp != null && comp.GetType().Name.Contains("CinemachineBrain"))
                {
                    cinemachineBrain = comp as Behaviour;
                    Debug.Log("[HeadFreezer] ✅ Found CinemachineBrain successfully!");
                    break;
                }
            }
        }
        
        if (cinemachineBrain == null)
        {
            Debug.LogWarning("[HeadFreezer] ⚠️ CinemachineBrain not found on Main Camera!");
        }

        Debug.Log("[HeadFreezer] ✅ Initialized. Synced with PlayerController.");
    }

    private void LateUpdate()
    {
        if (playerController == null || cinemachineBrain == null) return;

        // --- التحكم في تجميد الكاميرا برمجياً ---
        // بدلاً من الاستماع لزرار معين، نحن نتبع حالة الماوس الحقيقية الموجودة في PlayerController
        // لو الماوس ظاهر (مفتوح منيو أو بانل) = الكاميرا تتجمد
        // لو الماوس مخفي (اللعب شغال) = الكاميرا تتحرك
        
        cinemachineBrain.enabled = playerController.IsCursorLocked;
    }
}
