using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    public partial class Level
    {
        /// <summary>
        /// Level init phase.
        /// Indicates that the level is loading and has not been started yet (by calling <see cref="StartLevel"/>).
        /// </summary>
        public class LevelInitPhase : State<LevelPhase>
        {
            public override LevelPhase GetState() => LevelPhase.Init;
        }
    }
}
