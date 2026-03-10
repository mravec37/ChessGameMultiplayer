
let serverConnection = null;
let selectedSquare = null;
let playerColor = null;
let gameEnded = false;

const moveSound = new Audio("/sounds/move.mp3");

// ----------------------------
// SIGNALR CONNECTION
// ----------------------------


async function connectToHub() {
    serverConnection = new signalR.HubConnectionBuilder()
        .withUrl("/gamehub")
        .withAutomaticReconnect()
        .build();


    serverConnection.on("MoveApplied", (moveResult) => {
        console.log("♟ try to apply move:", moveResult);

       if (!moveResult.isValid) {
            console.log("Move is not valid");
            console.log("isValid: " + moveResult.isValid + " " + moveResult.IsValid);
            return;
        }

        // Promotion still handled client-side UI-wise
        if (moveResult.affected.some(e => e.type === "PROMOTION")) {
            console.log("PROMOTION");
            const effect = moveResult.affected.find(e => e.type === "PROMOTION");
            clearCheckHighlights();
            updateBoard(moveResult.affected);
            updateMoveSquares(moveResult.moveSquares);
            promotion(effect); 
            return;
        }
        console.log("going to update board");
        clearCheckHighlights();
        updateBoard(moveResult.affected);
        updateMoveSquares(moveResult.moveSquares);

        if (moveResult.affected.some(e => e.type === "CHECKMATE")) {
            console.log("Checkmate detected in move applied");
            return;
        }

        syncAndStopClock(sideToMove, moveResult.remainingTime)

        // Toggle side to move
        const nextSide =
            sideToMove === "White" ? "Black" : "White";
        startClockForSide(nextSide);

    });

    serverConnection.on("MoveRejected", (errorMessage) => {
        alert("Invalid move: " + errorMessage);
    });

    serverConnection.on("GameEnded", (gameEndData) => {
        resolveGameEnd(gameEndData);
    });

    serverConnection.on("Promoted", (promotionResult) => {
        resolvePromotionResult(promotionResult);
    });

    serverConnection.on("PossibleMoves", (positions) => {
        highlightPossibleMoves(positions);
    });


    // 🔥 SERVER → CLIENT EVENT (GAME START)
    serverConnection.on("GameStarted", (data) => {
        console.log("Game started!", data);

        playerColor = data.color;

        if (playerColor === "Black") {
            rotateBoardForBlack();
        }

        renderBoard(data.pieces);

        // Set the clocks
        console.log("Going to set clocks");
        setInitialClocks(data.clockTime);
        console.log("Going to start clocks");
        startClockForSide("White");
    });


    try {
        await serverConnection.start();
        console.log("Connected to SignalR hub:", serverConnection.connectionId);
    } catch (err) {
        console.error("SignalR connection failed:", err);
    }
}

// ----------------------------
// BOARD RENDERING
// ----------------------------


// Call this after every move to show last move path
// Example: updateMoveSquares(moveResult.moveSquares);

function updateMoveSquares(moveSquares) {
    console.log("Going to update move squares");
    document.querySelectorAll(".square.last-move-start, .square.last-move-end, .square.last-move-path")
        .forEach(sq => sq.classList.remove("last-move-start", "last-move-end", "last-move-path"));

    if (!moveSquares || moveSquares.length === 0) return;

    moveSquares.forEach((pos, i) => {
        console.log("Updating move square");
        const square = document.querySelector(`.square[data-x='${pos.x}'][data-y='${pos.y}']`);
        if (!square) return;

        if (i === 0) square.classList.add("last-move-start");
        else if (i === moveSquares.length - 1) square.classList.add("last-move-end");
        else square.classList.add("last-move-path");
    });
}

function resolvePromotionResult(promotionResult) {
    console.log("Going to resolve promotion result");
    console.log(promotionResult);
    var effect = promotionResult.affected[0];
    console.log(effect);

    if (!effect) {
        console.log("Promotion effect is null");
        return;
    }

    if (effect.type !== "PROMOTED") {
        console.log("Effect is not Promoted");
        return;
    }

    clearCheckHighlights();

    const x = effect.toX;
    const y = effect.toY;
    const pieceSymbol = effect.piece;

    console.log(
        "Promotion at X:", x,
        "Y:", y,
        "piece:", pieceSymbol
    );

    const square = document.querySelector(
        `.square[data-x='${x}'][data-y='${y}']`
    );

    if (!square) {
        console.warn("Promotion square not found");
        return;
    }

    console.log("Resolve promotion 1")
    // Remove pawn
    square.innerHTML = "";

    // Add promoted piece
    const img = document.createElement("img");
    img.src = `/images/pieces/${getPieceFilename({ piece: pieceSymbol })}.png`;
    img.alt = pieceSymbol;
    img.style.width = "100%";
    img.style.height = "100%";

    console.log("Resolve promotion 2")
    square.appendChild(img);

    updateBoard(promotionResult.affected);

    console.log("Going to sync and switch clocks, current player remaining time: " + promotionResult.remainingTime);
    syncAndStopClock(sideToMove, promotionResult.remainingTime)

    // Toggle side to move
    const nextSide =
        sideToMove === "White" ? "Black" : "White";
    startClockForSide(nextSide);
    console.log("Clocks switched");
}


function resolveGameEnd(gameEndData) {
    stopClock();
    gameEnded = true;
    if (gameEndData.gameEndEvent === "Time") {
        const loserColor = gameEndData.winner === "White" ? "Black" : "White";
        setLoserClockToZero(loserColor);
        console.log(gameEndData.winner + " player won");
        const pTime = playerRemainingMs/1000;
        const oTime = opponentRemainingMs/1000;
        console.log("player remaining seconds: " + pTime + " opponent remaining seconds: " + oTime);
        showGameOverPopup(gameEndData.winner, "time");
    } else if (gameEndData.gameEndEvent === "Checkmate") {
        console.log(gameEndData.winner + " player won on checkmate!");
        showGameOverPopup(gameEndData.winner, "checkmate");
    } else if (gameEndData.gameEndEvent === "Stalemate") {
        console.log("Stalemate");
        stopClock();
        showStalematePopup();
    }
    else {
        console.log("Unknown game end event");
    }
}

function showGameOverPopup(winner, reason) {
    const overlay = document.getElementById("game-over-overlay");
    const text = document.getElementById("game-over-text");

    text.textContent = `${winner} wins by ${reason}!`;

    overlay.classList.remove("hidden");
}

function showStalematePopup(winner, reason) {
    const overlay = document.getElementById("game-over-overlay");
    const text = document.getElementById("game-over-text");

    text.textContent = `Stalemate! It's a draw.`;

    overlay.classList.remove("hidden");
}

function renderBoard(pieces) {
    // Clear board first (important for reconnects)
    document.querySelectorAll(".square").forEach(sq => sq.innerHTML = "");

    pieces.forEach(piece => {
        const selector = `.square[data-x='${piece.x}'][data-y='${piece.y}']`;
        const square = document.querySelector(selector);
        if (!square) return;

        const img = document.createElement("img");
        img.src = `/images/pieces/${getPieceFilename(piece)}.png`;
        img.alt = piece.piece;
        img.style.width = "100%";
        img.style.height = "100%";

        square.appendChild(img);
    });
}

function getPieceFilename(piece) {
    // Uppercase = Black, lowercase = White (your convention)
    if (piece.piece === piece.piece.toUpperCase()) {
        return "b" + piece.piece.toLowerCase();
    } else {
        return piece.piece.toLowerCase();
    }
}

// ----------------------------
// BOARD ORIENTATION
// ----------------------------

function rotateBoardForBlack() {
    document.getElementById("chessboard").classList.add("flipped");
}

// ----------------------------
// UI INIT
// ----------------------------

document.addEventListener("DOMContentLoaded", () => {
    connectToHub();

    document.querySelectorAll(".square").forEach(square => {
        square.addEventListener("click", onSquareClick);
    });
});

function onSquareClick(event) {
    if (gameEnded) return;

    const square = event.currentTarget;
    const x = parseInt(square.dataset.x);
    const y = parseInt(square.dataset.y);

    const img = square.querySelector("img");

    // ---- SELECTING A PIECE ----
    if (!selectedSquare) {
        if (!img) return;

        if (!isPlayersPiece(img.alt)) {
            console.log("⛔ Not your piece");
            return;
        }

        selectedSquare = square;
        highlightSquare(square);

        // Ask server for possible moves
        serverConnection.invoke("GetPiecePossibleMoves", {
            x: x,
            y: y
        }, playerColor);

        return;
    }

    // 🟡 NEW: CLICKED SAME SQUARE AGAIN → DESELECT
    if (selectedSquare === square) {
        console.log("Deselecting piece");

        selectedSquare = null;
        clearHighlights();
        clearPossibleMoves();
        return; // 🔥 IMPORTANT — stop here, no move sent
    }

    // ---- MOVING A PIECE ----
    const fromX = parseInt(selectedSquare.dataset.x);
    const fromY = parseInt(selectedSquare.dataset.y);

    sendMove(fromX, fromY, x, y);

    selectedSquare = null;
    clearHighlights();
    clearPossibleMoves();
}

function isPlayersPiece(pieceChar) {
    console.log("Player color " + playerColor);
    if (!playerColor) return false;

    const isWhitePiece = pieceChar === pieceChar.toLowerCase();
    const isBlackPiece = pieceChar === pieceChar.toUpperCase();
    const x = (playerColor === "White" && isWhitePiece) ||
        (playerColor === "Black" && isBlackPiece)
    console.log(x);
    return x;
}

function highlightSquare(square) {
    square.style.outline = "3px solid yellow";
}

function clearHighlights() {
    document.querySelectorAll(".square").forEach(s => {
        s.style.outline = "none";
    });
}

async function sendMove(fromX, fromY, toX, toY) {
    const fromSquare = document.querySelector(
        `.square[data-x='${fromX}'][data-y='${fromY}']`
    );
    const img = fromSquare?.querySelector("img");
    if (!img) {
        alert("No piece selected.");
        return;
    }

    const moveRequest = {
        from: { x: fromX, y: fromY },
        to: { x: toX, y: toY },
        piece: img.alt
    };

    try {
        await serverConnection.invoke("SendMove", moveRequest);
    } catch (err) {
        console.error("Failed to send move: ", err);
    }
}




// ----------------------------
//  AUTO-PROMOTION LOGIC
// ----------------------------

function showPromotionChoices(x, y, pieceSymbol) {
    console.log("Showing promotion choices");
    const isWhitePiece = pieceSymbol === pieceSymbol.toLowerCase();
    const isPlayersPawn =
        (isWhitePiece && playerColor === "White") ||
        (!isWhitePiece && playerColor === "Black");

    if (!isPlayersPawn) {
        console.log("Not players promotion");
        return;
    }

    const overlay = document.getElementById("promotion-overlay");
    const container = document.getElementById("promotion-button-container");

    overlay.classList.remove("hidden");

    // Position overlay
    const square = document.querySelector(`.square[data-x='${x}'][data-y='${y}']`);
    const rect = square.getBoundingClientRect();
    overlay.style.top = `${rect.top - 60}px`;
    overlay.style.left = `${rect.left}px`;

    // Determine pawn color
    const isWhite = pieceSymbol === pieceSymbol.toLowerCase();

    // 🔥 Apply white background if BLACK is promoting
    if (!isWhite) {
        container.classList.add("promotion-white-bg");
    } else {
        container.classList.remove("promotion-white-bg");
    }

    container.innerHTML = "";
    console.log("Showing promotion choices 2");
    const options = ["Queen", "Rook", "Bishop", "Knight"];
    options.forEach(type => {
        const filename = getPieceFilename({
            piece: isWhite ? type.charAt(0).toLowerCase() : type.charAt(0)
        });


        const img = document.createElement("img");
        img.src = `/images/pieces/${filename}.png`;
        img.dataset.type = type;
        img.onclick = () => selectPromotion(type, x, y);

        container.appendChild(img);
    });
}

async function selectPromotion(type, x, y) {
    // Hide UI immediately
    console.log("Promotion piece selected");
    document.getElementById("promotion-overlay").classList.add("hidden");

    const promotionRequest = {
        promotionType: type,
        pawnPosition: { x: x, y: y }
    };

    console.log(promotionRequest);
    try {
        await serverConnection.invoke("PromotionChoice", promotionRequest);
        // Do NOT update board here
        // Server will send MoveApplied / PromotionApplied event
    } catch (err) {
        console.error("Promotion failed:", err);
        alert("Promotion failed.");
    }
}



async function promotion(effect) {
    console.log("promotion 1", effect);

    // Move pawn visually
    const fromSquare = document.querySelector(`.square[data-x='${effect.fromX}'][data-y='${effect.fromY}']`);
    const toSquare = document.querySelector(`.square[data-x='${effect.toX}'][data-y='${effect.toY}']`);
    console.log("promotion 2");

    const img = fromSquare?.querySelector("img");
    console.log("promotion 3");
    if (img) {
        fromSquare.innerHTML = "";
        toSquare.innerHTML = "";
        toSquare.appendChild(img);
        console.log("promotion 4");
    }

    moveSound.currentTime = 0;
    moveSound.play();

    console.log("going to show promotion choices");
    showPromotionChoices(effect.toX, effect.toY, img.alt);
}



// ----------------------------
//  BOARD UPDATE AFTER MOVE
// ----------------------------

function updateBoard(effects) {
    console.log("Updating pieces:", effects);
    console.log("Type: ", effects[0].type);

    effects.forEach(effect => {
        const fromSelector = `.square[data-x='${effect.fromX}'][data-y='${effect.fromY}']`;
        const toSelector = `.square[data-x='${effect.toX}'][data-y='${effect.toY}']`;
        const fromSquare = document.querySelector(fromSelector);
        const toSquare = document.querySelector(toSelector);

        if (effect.type === "MOVE" || effect.type === "CASTLING") {
            console.log("Move effect type");

            const img = fromSquare?.querySelector("img");
            if (img) {
                fromSquare.innerHTML = "";
                toSquare.innerHTML = "";
                toSquare.appendChild(img);

                moveSound.currentTime = 0; 
                moveSound.play();
            }
        }

        else if (effect.type === "CAPTURE") {

            if (effect.piece) {

                const filename = getPieceFilename({ piece: effect.piece });
                const img = document.createElement("img");
                img.src = `/images/pieces/${filename}.png`;
                img.alt = effect.piece;

                toSquare.innerHTML = "";

                // Determine color of captured piece
                const isWhitePiece = effect.piece === effect.piece.toLowerCase();
                const isPlayersPiece =
                    (playerColor === "White" && isWhitePiece) ||
                    (playerColor === "Black" && !isWhitePiece);

                const panel = isPlayersPiece
                    ? document.getElementById("captured-bottom")
                    : document.getElementById("captured-top");

                panel.appendChild(img);
            }
        }
        else if (effect.type === "PROMOTED") {
            toSquare.innerHTML = "";
            if (effect.piece) {
                const filename = getPieceFilename({ piece: effect.piece });
                const img = document.createElement("img");
                img.src = `/images/pieces/${filename}.png`;
                img.alt = effect.piece;
                img.style.width = "100%";
                img.style.height = "100%";
                toSquare.appendChild(img);
            }
        }
       /* else if (effect.type === "PROMOTED") {
            toSquare.innerHTML = "";
            if (effect.piece) {
                const filename = getPieceFilename({ piece: effect.piece });
                const img = document.createElement("img");
                img.src = `/images/pieces/${filename}.png`;
                img.alt = effect.piece;
                img.style.width = "100%";
                img.style.height = "100%";
                toSquare.appendChild(img);
            }
        }*/
        else if (effect.type === "CHECK" || effect.type === "CHECKMATE") {
            if (toSquare) {
                toSquare.classList.add("in-check");
            }
        }
        else {
            if (!toSquare) return;

            toSquare.innerHTML = "";
            if (effect.piece) {
                const filename = getPieceFilename({ piece: effect.piece });
                const img = document.createElement("img");
                img.src = `/images/pieces/${filename}.png`;
                img.alt = effect.piece;
                img.style.width = "100%";
                img.style.height = "100%";
                toSquare.appendChild(img);
            }
        }
    });
}

function clearCheckHighlights() {
    document.querySelectorAll(".square.in-check")
        .forEach(sq => sq.classList.remove("in-check"));
}

function highlightPossibleMoves(positions) {

    positions.forEach(pos => {
        const square = document.querySelector(
            `.square[data-x='${pos.x}'][data-y='${pos.y}']`
        );

        if (!square) return;

        const hasPiece = square.querySelector("img") !== null;

        if (hasPiece) {
            square.classList.add("possible-capture");
        } else {
            square.classList.add("possible-dot");
        }
    });
}

function clearPossibleMoves() {
    document.querySelectorAll(".possible-dot, .possible-capture")
        .forEach(sq => {
            sq.classList.remove("possible-dot");
            sq.classList.remove("possible-capture");
  });
}





