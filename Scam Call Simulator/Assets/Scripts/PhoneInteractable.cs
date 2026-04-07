using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 2f;
    public Transform player;

    [Header("UI")]
    public GameObject promptUI;   // drag your "Press E" UI element here

    private bool panelOpen = false;

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool inRange = distance <= interactRange;

        // Show/hide prompt
        promptUI.SetActive(inRange && !panelOpen);

        // Listen for E press only when in range
        if (inRange && !panelOpen && Input.GetKeyDown(KeyCode.E))
        {
            OpenPanel();
        }
    }

    void OpenPanel()
    {
        // GameManager.Instance.ToggleScenario(); // flips between Scammer and Victim each time
        GameManager.Instance.OpenPhone();
        promptUI.SetActive(false);
        panelOpen = true;
    }

    public void OnGameFinished()
    {
        panelOpen = false; // reset so player can interact again if needed
    }

    public void ClosePanel()
    {
        panelOpen = false;
    }
}