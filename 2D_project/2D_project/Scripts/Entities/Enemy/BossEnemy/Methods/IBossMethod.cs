internal interface IBossMethod
{
    void ResetSubsteps();

    /// <summary>
    /// Sets the parameters for the movement logic.
    /// </summary>
    void SetParameters(params object[] parameters);

    /// <summary>
    /// Executes the movement logic.
    /// </summary>
    void Execute(float? deltaTime = null);
}
