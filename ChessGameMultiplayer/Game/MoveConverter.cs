using ChessGameMultiplayer.Dto;
using ChessGameMultiplayer.Game.Moves;

namespace ChessGameMultiplayer.Game
{
    public static class MoveConverter
    {
        public static List<MoveEffectDto> ConvertToDtoList(MoveResult result)
        {
            var dtoList = new List<MoveEffectDto>();

            if (result == null || result.Affected == null)
                return dtoList;

            foreach (var effect in result.Affected)
            {
                if (effect == null)
                    continue;

                var dto = new MoveEffectDto
                {
                    Type = effect.Type.ToString(),
                    FromX = effect.from?.X ?? -1,
                    FromY = effect.from?.Y ?? -1,
                    ToX = effect.to?.X ?? -1,
                    ToY = effect.to?.Y ?? -1,
                    Piece = effect.Piece?.GetSymbol().ToString(),
                    IsValid = result.IsValid
                };

                dtoList.Add(dto);
            }

            return dtoList;
        }

    }

}
