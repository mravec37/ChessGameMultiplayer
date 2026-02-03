let sideToMove = "White"; // "White" or "Black"

// REAL remaining time (authoritative)
let playerRemainingMs = 0;
let opponentRemainingMs = 0;

// UI interval
let clockIntervalId = null;

// ----------------------------
// INITIALIZATION
// ----------------------------

function setInitialClocks(milliseconds) {
    playerRemainingMs = milliseconds;
    opponentRemainingMs = milliseconds;

    updateClockFromMs("player-clock", playerRemainingMs);
    updateClockFromMs("opponent-clock", opponentRemainingMs);
}

// ----------------------------
// CLOCK DISPLAY
// ----------------------------

function updateClockFromMs(clockId, remainingMs) {
    const totalSeconds = Math.floor(remainingMs / 1000);

    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    document.getElementById(clockId).textContent =
        `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
}

// ----------------------------
// START COUNTDOWN
// ----------------------------

function startClockForSide(colorToMove) {
    console.log("Starting the clock for side: " + colorToMove);
    stopClock(); 

    sideToMove = colorToMove;

    clockIntervalId = setInterval(() => {
        if (sideToMove === playerColor) {
            playerRemainingMs -= 1000;

            if (playerRemainingMs <= 0) {
                playerRemainingMs = 0;
                stopClock();
                console.log("⏰ Player ran out of time");
            }

            updateClockFromMs("player-clock", playerRemainingMs);
        } else {
            opponentRemainingMs -= 1000;

            if (opponentRemainingMs <= 0) {
                opponentRemainingMs = 0;
                stopClock();
                console.log("⏰ Opponent ran out of time");
            }

            updateClockFromMs("opponent-clock", opponentRemainingMs);
        }
    }, 1000);
}

// ----------------------------
// STOP CLOCK (LOCAL)
// ----------------------------

function stopClock() {
    if (clockIntervalId !== null) {
        clearInterval(clockIntervalId);
        clockIntervalId = null;
    }
}

// ----------------------------
// SERVER SYNC (AUTHORITATIVE)
// ----------------------------

function syncAndStopClock(turnColor, remainingMilliseconds) {
    stopClock();

    const isPlayer = (turnColor === playerColor);

    if (isPlayer) {
        playerRemainingMs = remainingMilliseconds;
        updateClockFromMs("player-clock", playerRemainingMs);
    } else {
        opponentRemainingMs = remainingMilliseconds;
        updateClockFromMs("opponent-clock", opponentRemainingMs);
    }
}

function updateClock(clockId, totalSeconds) {
    const minutes = Math.floor(totalSeconds / 60);
    const seconds = totalSeconds % 60;

    document.getElementById(clockId).textContent =
        `${String(minutes).padStart(2, "0")}:${String(seconds).padStart(2, "0")}`;
}

function setLoserClockToZero(loserColor) {
    stopClock();

    const isPlayerLoser = (loserColor === playerColor);

    if (isPlayerLoser) {
        playerRemainingMs = 0;
        updateClock("player-clock", 0);
    } else {
        opponentRemainingMs = 0;
        updateClock("opponent-clock", 0);
    }
}

