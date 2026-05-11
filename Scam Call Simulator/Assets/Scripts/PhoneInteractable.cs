using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneInteractable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactRange = 2f;
    public Transform player; // drag Player GameObject here in Inspector

    [Header("UI")]
    public GameObject promptUI;

    private bool panelOpen = false;

    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            if (player == null) return;
        }

        Vector2 phonePos = new Vector2(transform.position.x, transform.position.y);
        Vector2 playerPos = new Vector2(player.position.x, player.position.y);
        float distance = Vector2.Distance(phonePos, playerPos);
        bool inRange = distance <= interactRange;

        promptUI.SetActive(inRange && !panelOpen);

        if (inRange && !panelOpen && Input.GetKeyDown(KeyCode.E))
            OpenPanel();
    }

    void OpenPanel()
    {
        GameManager.Instance.OpenPhone();
        promptUI.SetActive(false);
        panelOpen = true;
    }

    public void OnGameFinished()
    {
        panelOpen = false;
    }

    public void ClosePanel()
    {
        panelOpen = false;
    }
}