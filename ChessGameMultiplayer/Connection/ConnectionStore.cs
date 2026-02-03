namespace ChessGameMultiplayer.Connection
{
    public class ConnectionStore
    {
        private string? playerOneConnectionId;
        private string? playerTwoConnectionId;

        public void Add(string connectionId)
        {
            if (playerOneConnectionId == null)
            {
                playerOneConnectionId = connectionId;
            }
            else if (playerTwoConnectionId == null)
            {
                playerTwoConnectionId = connectionId;
            }
        }

        public bool BothPlayersConnected =>
            playerOneConnectionId != null && playerTwoConnectionId != null;

        public string PlayerOne => playerOneConnectionId!;
        public string PlayerTwo => playerTwoConnectionId!;
    }

}
