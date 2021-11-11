using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebXR;

/// <summary>
/// Script to close your hand with trigger or grip buttons
/// </summary>
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(WebXRController))]
public class CloseHands : MonoBehaviour
{
    /// <summary>
    /// Reference to the animator of the hand
    /// </summary>
    private Animator m_animator;

    /// <summary>
    /// Reference to the controller of the hand
    /// </summary>
    private WebXRController m_controller;

    /// <summary>
    /// Awake
    /// </summary>
    void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_controller = GetComponent<WebXRController>();
    }

    /// <summary>
    /// Update
    /// </summary>
    void Update()
    {
        // Understand what frame of the animation to show:
        // - If trigger is pressed, the hand is closed
        // - If the grip is pressed, see how much it is pressed, and close the hand of that amount
        float normalizedTime = m_controller.GetButton(WebXRController.ButtonTypes.Trigger) ? 1 : m_controller.GetAxis(WebXRController.AxisTypes.Grip);

        // Use the animator to show that pose
        m_animator.Play("Take", -1, normalizedTime);
    }
}
