using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Underconnected
{
    /// <summary>
    /// Represents an object that can be interacted with by <see cref="Interactor"/> objects (like players).
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Interact with this object.
        /// </summary>
        /// <param name="interactor">The initiator of this interaction.</param>
        void Interact(Interactor interactor);
    }
}
