using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Connect.Core
{
    /// <summary>
    /// Block logic for paint mode
    /// </summary>
    public class BlockPaint : MonoBehaviour
    {
        [HideInInspector] public bool Blocked;
        [HideInInspector] public bool Filled;

        [SerializeField] private SpriteRenderer _blockSprite;
        [SerializeField] private Color _emptyColor;
        [SerializeField] private Color _blockedColor;
        [SerializeField] private Color _activeColor;

        public void OnServerInitialized()
        {
            
        }




    }
}

