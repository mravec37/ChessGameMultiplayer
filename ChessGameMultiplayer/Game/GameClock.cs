using ChessGameMultiplayer.Game.ChessPieces;
using System.Diagnostics;

namespace ChessGameMultiplayer.Game
{
    public class GameClock
    {
        private TimeSpan whiteRemainingTime;
        private TimeSpan blackRemainingTime;
        private const int CLOCK_TIME_MINUTES = 1;
        private const int CLOCK_TIME_SECONDS = 300;

        private Stopwatch stopwatch;

        private ChessPieceColor currentTurn;

        private Task turnCountown;
        private CancellationTokenSource? turnCts;

        public event EventHandler? TimeExpired;


        public GameClock()
        {
            //whiteRemainingTime = TimeSpan.FromMinutes(CLOCK_TIME_MINUTES);
           // blackRemainingTime = TimeSpan.FromMinutes(CLOCK_TIME_MINUTES);
            whiteRemainingTime = TimeSpan.FromSeconds(CLOCK_TIME_SECONDS);
            blackRemainingTime = TimeSpan.FromSeconds(CLOCK_TIME_SECONDS);
            currentTurn = ChessPieceColor.White;
        }

        public void StartGameCountdown()
        {
            stopwatch = Stopwatch.StartNew();
            StartTurnCountown(whiteRemainingTime);
        }

        private void StartTurnCountown(TimeSpan duration)
        {
            turnCts = new CancellationTokenSource();
            turnCountown = Task.Run(async () =>
            {
                try
                {
                    await Task.Delay(duration, turnCts.Token);
                    // If we reach here, the time expired
                    OnTurnTimeExpired(currentTurn); // trigger your event
                }
                catch (TaskCanceledException)
                {
                    // Countdown was cancelled because a move happened
                }
            });
        }

        private void OnTurnTimeExpired(ChessPieceColor currentTurn)
        {
            TimeExpired?.Invoke(this, new TimeExpiredEventArgs(currentTurn));
        }

        public TimeSpan SwitchTurnCountdown()
        {
            turnCts.Cancel();
            TimeSpan turnTime = stopwatch.Elapsed;
            TimeSpan currentPlayerRemainingTime;
            if (currentTurn == ChessPieceColor.White)
            {
                whiteRemainingTime = whiteRemainingTime - turnTime;
                currentPlayerRemainingTime = whiteRemainingTime;
            }
            else
            {
                blackRemainingTime = blackRemainingTime - turnTime;
                currentPlayerRemainingTime = blackRemainingTime;
            }

            //Starting turn for other player
            currentTurn = MoveConverter.GetOppositeColor(currentTurn);
            StartTurnCountdownForCurrentPlayer();
            stopwatch.Restart();
            return currentPlayerRemainingTime;
        }

        private void StartTurnCountdownForCurrentPlayer()
        {
            if(currentTurn == ChessPieceColor.White)
            {
                StartTurnCountown(whiteRemainingTime);
            } else
            {
                StartTurnCountown(blackRemainingTime);
            }
        }

        public TimeSpan ClockStartTime { get { return TimeSpan.FromSeconds(CLOCK_TIME_SECONDS); } }
    }
}
