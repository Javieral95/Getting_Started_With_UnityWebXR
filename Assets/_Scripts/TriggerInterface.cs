public interface TriggerInterface
{
    /// <summary>
    /// Call all functions
    /// </summary>
    void Press();

    // - - - - -

    /// <summary>
    /// Logic of button
    /// </summary>
    void PressFunction();

    /// <summary>
    /// Play the animation if its exists
    /// </summary>
    void PlayPressAnimation();

    /// <summary>
    /// Play sound if its exists
    /// </summary>
    void PlayPressSound();
}
