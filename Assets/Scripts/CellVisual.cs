using UnityEngine;
using GameTest.DarioMunoz.Domain.Board;


namespace GameTest.DarioMunoz
{
    /// <summary>
    /// Visual representation of a single cell in the grid.
    /// Only handles displaying the piece sprite.
    /// Input is handled by GameManager using the New Input System.
    /// </summary>
    
    public class CellVisual : MonoBehaviour
    {
        [Header("Visual Settings")]
        [SerializeField] private SpriteRenderer pieceRenderer;
        [SerializeField] private Sprite greenSprite;
        [SerializeField] private Sprite purpleSprite;
        [SerializeField] private Sprite yellowSprite;
        [SerializeField] private Sprite brownSprite;
        [SerializeField] private Sprite redSprite;

        private CellModel _model;
        public CellModel Model => _model;

        private void Awake()
        {
            if (pieceRenderer == null)
                pieceRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        public void Initialize(CellModel model)
        {
            _model = model;
            Refresh();
        }
        public void Refresh()
        {
            if (_model == null) return;

            if (pieceRenderer == null) return;

            pieceRenderer.sprite = _model.IsEmpty
                ? null
                : GetSpriteForType(_model.Piece.Type);
        }

        public void PlayClearEffect()
        {
            if (pieceRenderer != null)
                pieceRenderer.sprite = null;
        }

        private Sprite GetSpriteForType(PieceType type)
        {
            return type switch
            {
                PieceType.Green => greenSprite,
                PieceType.Purple => purpleSprite,
                PieceType.Yellow => yellowSprite,
                PieceType.Brown => brownSprite,
                PieceType.Red => redSprite,
                _ => null
            };
        }
    }
}


