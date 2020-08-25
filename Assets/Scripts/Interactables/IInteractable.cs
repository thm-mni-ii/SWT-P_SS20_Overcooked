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

        /// <summary>
        /// Sets the interactor that is currently looking at this interactable.
        /// </summary>
        /// <param name="watcher">The interactor that is looking at this interactable. `null` to remove the current one.</param>
        void SetWatcher(Interactor watcher);

        /// <summary>
        /// Returns the game object this interactable belongs to.
        /// </summary>
        /// <returns>The game object this interactable belongs to.</returns>
        GameObject GetGameObject();
    }
}
