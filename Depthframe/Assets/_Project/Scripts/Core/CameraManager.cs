using UnityEngine;
using Unity.Cinemachine;
using System.Collections;


public class CameraManager : MonoBehaviour
{
    public CinemachineCamera cutsceneCamera;
    public CinemachineCamera playerCamera;
    public float cutsceneDuration = 5f; // Duration of the cutscene in seconds

    private void Start()
    {
        StartCoroutine(StartCutscene());
    }

    private IEnumerator StartCutscene()
    {
        // Set cutscene camera priority higher than player camera
        cutsceneCamera.Priority = 10;
        playerCamera.Priority = 0;

        // Wait for the duration of the cutscene
        yield return new WaitForSeconds(cutsceneDuration);

        // Switch to the player camera
        cutsceneCamera.Priority = 0;
        playerCamera.Priority = 10;
    }
}