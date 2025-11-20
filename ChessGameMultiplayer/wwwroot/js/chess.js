async function loadGame() {
    const res = await fetch("/Game/NewGame");
    const pieces = await res.json();

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

function getPieceFilename(piece) {
    if (piece.piece === piece.piece.toUpperCase()) {
        return "b" + piece.piece.toLowerCase(); // black
    } else {
        return piece.piece.toLowerCase();       // white
    }
}

let selectedSquare = null;

document.addEventListener("DOMContentLoaded", () => {
    loadGame();
    document.querySelectorAll(".square").forEach(square => {
        square.addEventListener("click", onSquareClick);
    });
});

function onSquareClick(event) {
    const square = event.currentTarget;
    const x = parseInt(square.dataset.x);
    const y = parseInt(square.dataset.y);

    if (selectedSquare) {
        const fromX = parseInt(selectedSquare.dataset.x);
        const fromY = parseInt(selectedSquare.dataset.y);

        sendMove(fromX, fromY, x, y);
        selectedSquare = null;
        clearHighlights();
    } else {
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

    if (response.ok) {
        const moveEffects = await response.json();

        if (!moveEffects || moveEffects.length === 0 || !moveEffects[0].IsValid) {
            alert("Invalid move.");
            return;
        }

        // Check for promotion effect FIRST
        if (moveEffects.some(e => e.Type === "Promotion")) {
            const effect = moveEffects.find(e => e.Type === "Promotion");
            await promotion(effect);   // ⬅ AUTO QUEEN PROMOTION
            return;
        }

        updateBoard(moveEffects);
    } else {
        alert("Server error.");
    }
}



// ----------------------------
//  AUTO-PROMOTION LOGIC
// ----------------------------

function showPromotionChoices(x, y, pieceSymbol) {
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
    document.getElementById("promotion-overlay").classList.add("hidden");

    const response = await fetch("/Game/PromotionChoice", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            promotionType: type,           // ENUM EXACT NAME
            pawnPosition: { x: x, y: y }
        })
    });

    if (!response.ok) {
        alert("Promotion failed: " + response.status);
        console.log(await response.text());
        return;
    }

    const finalEffects = await response.json();
    updateBoard(finalEffects);
}


async function promotion(effect) {
    console.log("Auto promotion triggered:", effect);

    // Move pawn visually
    const fromSquare = document.querySelector(`.square[data-x='${effect.FromX}'][data-y='${effect.FromY}']`);
    const toSquare = document.querySelector(`.square[data-x='${effect.ToX}'][data-y='${effect.ToY}']`);

    const img = fromSquare?.querySelector("img");
    if (img) {
        fromSquare.innerHTML = "";
        toSquare.innerHTML = "";
        toSquare.appendChild(img);
    }

    showPromotionChoices(effect.ToX, effect.ToY, img.alt);
}



// ----------------------------
//  BOARD UPDATE AFTER MOVE
// ----------------------------

function updateBoard(effects) {
    console.log("Updating pieces:", effects);

    effects.forEach(effect => {
        const fromSelector = `.square[data-x='${effect.FromX}'][data-y='${effect.FromY}']`;
        const toSelector = `.square[data-x='${effect.ToX}'][data-y='${effect.ToY}']`;
        const fromSquare = document.querySelector(fromSelector);
        const toSquare = document.querySelector(toSelector);

        if (effect.Type === "Move" || effect.Type === "Castling") {

            const img = fromSquare?.querySelector("img");
            if (img) {
                fromSquare.innerHTML = "";
                toSquare.innerHTML = "";
                toSquare.appendChild(img);
            }
        }

        else if (effect.Type === "Capture") {

            if (effect.Piece) {
                const filename = getPieceFilename({ piece: effect.Piece });
                const img = document.createElement("img");
                img.src = `/images/pieces/${filename}.png`;
                img.alt = effect.Piece;

                toSquare.innerHTML = "";

                const isWhite = effect.Piece === effect.Piece.toLowerCase();
                const panel = isWhite
                    ? document.getElementById("captured-bottom")
                    : document.getElementById("captured-top");

                panel.appendChild(img);
            }
        }
        else if (effect.Type === "Promoted") {
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

        else { // Promotion result or general piece placement
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
