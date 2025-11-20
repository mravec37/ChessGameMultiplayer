using ChessGameMultiplayer.Game;

namespace ChessGameMultiplayer.Dto
{
    public class PromotionRequest
    {
       public String promotionType { get; set; }
       public Position pawnPosition { get; set; }
    }
}