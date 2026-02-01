namespace GameTest.DarioMunoz.Domain.Board
{
    public class PieceModel
    {
        public PieceType Type { get; }

        public PieceModel(PieceType type)
        {
            Type = type;
        }
    }
}