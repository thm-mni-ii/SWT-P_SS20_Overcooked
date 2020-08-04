/// <summary>
/// Tells how much of a recipe has been matched.
/// </summary>
public enum RecipeMatchState
{
    /// <summary>
    /// The recipe has not been matched at all (there are foreign elements which are not allowed).
    /// </summary>
    NoMatch,
    /// <summary>
    /// The recipe has been partially matched (there are a few missing elements).
    /// </summary>
    PartialMatch,
    /// <summary>
    /// The recipe has been fully matched and can produce its output.
    /// </summary>
    FullMatch
}
