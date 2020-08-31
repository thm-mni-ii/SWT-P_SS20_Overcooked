/* Created by: SWT-P_SS20_Overcooked (Team Drai Studios) */
namespace Underconnected
{
    /// <summary>
    /// Contains phases for game levels.
    /// </summary>
    public enum LevelPhase
    {
        /// <summary>
        /// The initial state.
        /// </summary>
        Init,
        /// <summary>
        /// The preparing state which shows a timer running down.
        /// </summary>
        Preparing,
        /// <summary>
        /// The level is being played.
        /// Players can move around and deliver recipes.
        /// </summary>
        Playing,
        /// <summary>
        /// The level is finished.
        /// Players can no longer move and see a 'finished' screen.
        /// </summary>
        Finished
    }
}
