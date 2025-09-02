using UnityEngine;

public class MechanicStateManager : MonoBehaviour
{
    public static MechanicStateManager Instance { get; private set; }

    public bool CanRun = false;
    public bool CanJump = false;
    public bool CanEquipWeapon = false;
    public bool CanUnequipWeapon = false;
    public bool CanAttack = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}