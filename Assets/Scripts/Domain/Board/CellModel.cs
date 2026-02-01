namespace GameTest.DarioMunoz.Domain.Board
{
    public class CellModel
    {
        public int X { get; }
        public int Y { get; }

        public PieceModel Piece { get; private set; }

        public bool IsEmpty => Piece == null;

        public CellModel(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetPiece(PieceModel piece)
        {
            Piece = piece;
        }

        public void Clear()
        {
            Piece = null;
        }
    }
}