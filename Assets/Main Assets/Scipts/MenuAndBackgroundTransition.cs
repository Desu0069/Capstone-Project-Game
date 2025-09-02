using UnityEngine;
using System.Collections;

public class MenuAndBackgroundSwitch : MonoBehaviour
{
    public GameObject MainMenu; // Assign MainMenu in Inspector
    public GameObject BG;  // Background 1
    public GameObject BG2; // Background 2

    public float rotationAngle = 30f; // Rotation degrees
    public float transitionDuration = 3f; // How long the rotation takes
    public float loopDelay = 2f; // Delay before switching back

    private Quaternion menuStartRotation;
    private bool isBG1Active = true;

    void Start()
    {
        // Store original rotation
        menuStartRotation = MainMenu.transform.rotation;

        // Ensure only one background is active at the start
        BG.SetActive(true);
        BG2.SetActive(false);

        StartCoroutine(LoopTransition());
    }

    IEnumerator LoopTransition()
    {
        while (true)
        {
            yield return StartCoroutine(RotateMenu());

            yield return new WaitForSeconds(loopDelay); // Pause before switching

            // Switch backgrounds instantly
            isBG1Active = !isBG1Active;
            BG.SetActive(isBG1Active);
            BG2.SetActive(!isBG1Active);

            // Reset rotation before the next loop
            MainMenu.transform.rotation = menuStartRotation;
        }
    }

    IEnumerator RotateMenu()
    {
        float elapsedTime = 0f;
        Quaternion targetRotation = Quaternion.Euler(MainMenu.transform.eulerAngles + new Vector3(0, rotationAngle, 0));

        while (elapsedTime < transitionDuration)
        {
            MainMenu.transform.rotation = Quaternion.Slerp(menuStartRotation, targetRotation, elapsedTime / transitionDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        MainMenu.transform.rotation = targetRotation; // Ensure it stops at exact rotation
    }
}
