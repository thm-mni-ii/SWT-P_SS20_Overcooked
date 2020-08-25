using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Underconnected
{
    public class ObjectHighlighting : MonoBehaviour
    {
        [SerializeField] Material highlightMaterial;


        /// <summary>
        /// Tells whether the highlighting should be rendered.
        /// </summary>
        private bool isHighlightEnabled;

        /// <summary>
        /// Holds the command buffer to render highlighting.
        /// </summary>
        private CommandBuffer commandBuffer;


        private void Awake()
        {
            if (this.commandBuffer == null)
            {
                this.commandBuffer = new CommandBuffer();
                this.commandBuffer.name = "Highlighting";
                foreach (Renderer renderer in this.GetComponentsInChildren<Renderer>())
                    for (int i = 0; i < renderer.materials.Length; i++)
                        this.commandBuffer.DrawRenderer(renderer, this.highlightMaterial, i);
            }
        }

        /// <summary>
        /// Enables the highlighting effect on this object.
        /// </summary>
        public void ShowHighlighting()
        {
            if (!this.isHighlightEnabled)
            {
                GameManager.Camera.AddCommandBuffer(CameraEvent.AfterForwardOpaque, this.commandBuffer);
                this.isHighlightEnabled = true;
            }
        }
        /// <summary>
        /// Disables the highlighting effect on this object.
        /// </summary>
        public void HideHighlighting()
        {
            if (this.isHighlightEnabled)
            {
                GameManager.Camera.RemoveCommandBuffer(CameraEvent.AfterForwardOpaque, this.commandBuffer);
                this.isHighlightEnabled = false;
            }
        }
    }
}
