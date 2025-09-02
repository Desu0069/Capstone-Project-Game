using UnityEngine;

public class TutorialCompletionManager : MonoBehaviour
{
    public void MarkTutorialAsComplete()
    {
        if (UserSlotsManager.Instance != null && UserSlotsManager.Instance.activeSlotIndex >= 0)
        {
            UserSlotsManager.Instance.SetTutorialComplete(UserSlotsManager.Instance.activeSlotIndex);
        }
    }
}