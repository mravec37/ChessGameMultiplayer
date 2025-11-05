async function loadGame() {
    const res = await fetch("/Game/NewGame");
    const pieces = await res.json();

    console.log("Load Game");

    pieces.forEach(piece => {
        const selector = `.square[data-x='${piece.x}'][data-y='${piece.y}']`;
        const square = document.querySelector(selector);
        if (square) {
            let filename = getPieceFilename(piece);
            const img = document.createElement("img");
            img.src = `/images/pieces/${filename}.png`; 
            img.alt = piece.piece;
            img.style.width = "100%";
            img.style.height = "100%";
            square.innerHTML = ""; 
            square.appendChild(img);
        }
    })
}

// Helper function to get filename based on piece color
function getPieceFilename(piece) {
    if (piece.piece === piece.piece.toUpperCase()) {
        return "b" + piece.piece.toLowerCase(); // Black piece
    } else {
        return piece.piece.toLowerCase();       // White piece
    }
}

let selectedSquare = null;

document.addEventListener("DOMContentLoaded", () => {
    loadGame();
    console.log("Event Listener");
    // Attach click handlers to squares
    document.querySelectorAll(".square").forEach(square => {
        square.addEventListener("click", onSquareClick);
    });
});

function onSquareClick(event) {
    const square = event.currentTarget;
    const x = parseInt(square.dataset.x);
    const y = parseInt(square.dataset.y);

    if (selectedSquare) {
        // Second click — attempt move
        const fromX = parseInt(selectedSquare.dataset.x);
        const fromY = parseInt(selectedSquare.dataset.y);

        sendMove(fromX, fromY, x, y);
        selectedSquare = null; // reset
        clearHighlights();
    } else {
        // First click — select only if there's a piece
        if (square.querySelector("img")) {
            selectedSquare = square;
            highlightSquare(square);
        }
    }
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
    const fromSquare = document.querySelector(`.square[data-x='${fromX}'][data-y='${fromY}']`);
    const img = fromSquare.querySelector("img");
    if (!img) return alert("No piece selected.");

    const piece = img.alt;

    const response = await fetch("/Game/MovePiece", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            from: { x: fromX, y: fromY },
            to: { x: toX, y: toY },
            piece
        })
    });

    console.log("Checking response...");
    if (response.ok) {
        console.log("Response is OK");
        const moveEffects = await response.json();
        console.log("Received effects:", moveEffects);

        if (!moveEffects || moveEffects.length === 0 || !moveEffects[0].IsValid) {
            alert("Invalid move.");
            return;
        }

        updateBoard(moveEffects);
    } else {
        alert("Server error.");
    }
}



// You'd need to implement this to re-render pieces after a move
function updateBoard(effects) {
    console.log("Updating pieces:", effects);

    effects.forEach(effect => {
        const fromSelector = `.square[data-x='${effect.FromX}'][data-y='${effect.FromY}']`;
        const toSelector = `.square[data-x='${effect.ToX}'][data-y='${effect.ToY}']`;
        const fromSquare = document.querySelector(fromSelector);
        const toSquare = document.querySelector(toSelector);

        if (effect.Type === "Move") {
            if (!fromSquare || !toSquare) {
                console.warn("Move effect has invalid coordinates.");
                return;
            }

            const img = fromSquare.querySelector("img");
            fromSquare.innerHTML = "";
            toSquare.innerHTML = "";

            if (img) {
                toSquare.appendChild(img);
            }
        }

        else if (effect.Type === "Capture") {
            // Don't manipulate board, just add to captured pieces
            if (effect.Piece) {
                const filename = getPieceFilename({ piece: effect.Piece });
                const img = document.createElement("img");
                img.src = `/images/pieces/${filename}.png`;
                img.alt = effect.Piece;
                img.title = effect.Piece;

                // Determine if the captured piece is ours or theirs
                const isWhite = effect.Piece === effect.Piece.toLowerCase();
                const panel = isWhite ? document.getElementById("captured-bottom") : document.getElementById("captured-top");

                panel.appendChild(img);
            }
        }

        else {
            if (!toSquare) return;

            toSquare.innerHTML = "";

            if (effect.Piece) {
                const filename = getPieceFilename({ piece: effect.Piece });
                const img = document.createElement("img");
                img.src = `/images/pieces/${filename}.png`;
                img.alt = effect.Piece;
                img.style.width = "100%";
                img.style.height = "100%";
                toSquare.appendChild(img);
            }
        }
    });
}


//document.addEventListener("DOMContentLoaded", loadGame);
