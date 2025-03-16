using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using MoreMountains.TopDownEngine; 

namespace dungeonduell
{
    public class HexgridController : MonoBehaviour
    {
        [Header("Player Settings")]
        public bool isPlayerOne;  // oder eine Methode IsPlayer1Card() unten

        [Header("References")]
        public Tilemap tilemap;
        public GameObject cursor;                // Highlighter
        public TileClickHandler tileClickHandler; 
        public CardToHand cardToHand;           

        // Die Karte, die aktuell im CardHolder liegt
        public DisplayCard currentDisplayCard;    

        private PlayerInput playerInput;         
        private Vector3Int selectedTilePos;      

        // Even-/Odd-Offsets, wie gehabt:
        private static readonly Vector3Int[] OffsetsEven =
        {
            new Vector3Int(0, 1, 0),   // Oben links
            new Vector3Int(1, 1, 0),   // Oben rechts
            new Vector3Int(-1, 0, 0),  // Links
            new Vector3Int(1, 0, 0),   // Rechts
            new Vector3Int(0, -1, 0),  // Unten links
            new Vector3Int(1, -1, 0)   // Unten rechts
        };

        private static readonly Vector3Int[] OffsetsOdd =
        {
            new Vector3Int(-1, 1, 0),  // Oben links
            new Vector3Int(0, 1, 0),   // Oben rechts
            new Vector3Int(-1, 0, 0),  // Links
            new Vector3Int(1, 0, 0),   // Rechts
            new Vector3Int(-1, -1, 0), // Unten links
            new Vector3Int(0, -1, 0)   // Unten rechts
        };

        private void Start()
        {
            // Sucht  den PlayerInput:
            if (playerInput == null)
            {
                playerInput = FindCorrectPlayerInput();
            }

            // Dann das passende CardToHand holen
            if (cardToHand == null)
            {
                cardToHand = FindCorrectCardToHand();
            }

            // Cursor vorerst aus:
            if (cursor) cursor.SetActive(false);

            // Input-Hooks setzen:
            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed += OnNavigate;
                playerInput.actions["Submit"].performed += OnSubmit;
                playerInput.actions["Back"].performed += OnBack;
            }
        }

        private void OnDestroy()
        {
            // Abos lösen
            if (playerInput != null)
            {
                playerInput.actions["Navigation"].performed -= OnNavigate;
                playerInput.actions["Submit"].performed -= OnSubmit;
                playerInput.actions["Back"].performed -= OnBack;
            }
        }

        /// <summary>
        /// Aktiviert die Hex-Navigation, 
        /// z.B. aufgerufen wenn Spieler eine Karte in den CardHolder legt.
        /// </summary>
        public void ActivateNavigation()
        {
            // Lade die Grenzen (Bounds) der Tilemap
            BoundsInt bounds = tilemap.cellBounds;
            bool foundValid = false;

            // Durchlaufe jede Position innerhalb dieser Bounds
            foreach (var cellPos in bounds.allPositionsWithin)
            {
                // Prüfe: Ist das ein setzbares Feld?
                if (tileClickHandler.IsSetablePosition(cellPos))
                {
                    // Falls ja, setze das als selectedTilePos und brich die Schleife ab
                    selectedTilePos = cellPos;
                    foundValid = true;
                    break;
                }
            }

            // Wenn man nichts gefunden hat, kann man z. B. warnen:
            if (!foundValid)
            {
                Debug.LogWarning("Kein setzbares Feld in der Tilemap gefunden!");
                return;
            }

            // Cursor sichtbar & positionieren:
            if (cursor)
            {
                cursor.SetActive(true);
                cursor.transform.position = tilemap.GetCellCenterWorld(selectedTilePos);
            }
        }


        /// <summary>
        /// Cursor/Navigation aus.
        /// </summary>
        public void ResetNavigation()
        {
            if (cursor) cursor.SetActive(false);
        }

        private void OnNavigate(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf) return;

            Vector2 input = ctx.ReadValue<Vector2>();
            // Deadzone
            if (input.sqrMagnitude < 0.3f) return;

            float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;

            int sector = Mathf.RoundToInt(angle / 60f) % 6;
            Vector3Int[] offsets = (selectedTilePos.y % 2 == 0) ? OffsetsEven : OffsetsOdd;
            Vector3Int neighborPos = selectedTilePos + offsets[sector];

            // **Jetzt fragst du tileClickHandler**:
            if (tileClickHandler.IsSetablePosition(neighborPos))
            {
                selectedTilePos = neighborPos;
                cursor.transform.position = tilemap.GetCellCenterWorld(selectedTilePos);
            }
        }

        private void OnSubmit(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf) return;

            Vector3 worldPos = tilemap.GetCellCenterWorld(selectedTilePos);

            // Karte mit tileClickHandler.SpawnTile(...) platzieren:
            tileClickHandler.SpawnTile(
                worldPos,
                tileClickHandler.currentCard, // oder was immer du dort hast
                true, 
                true,
                tileClickHandler.isPlayer1Turn ? 1 : 2
            );

            // Danach beenden wir die Navigation:
            ResetNavigation();

            // Evtl. "NextTurn" etc.
        }

        private void OnBack(InputAction.CallbackContext ctx)
        {
            if (!cursor || !cursor.activeSelf) return;

            // Karte zurück auf die Hand legen:
            if (cardToHand != null && currentDisplayCard != null)
            {
                cardToHand.OnCardClicked(currentDisplayCard);
            }
            ResetNavigation();
        }

        private bool IsPlayer1Card()
        {
            return isPlayerOne;
        }

        private PlayerInput FindCorrectPlayerInput()
        {
            // Geh alle PlayerInputs durch und guck, 
            // wer "Player1" oder "Player2" ist:
            string neededPlayerID = IsPlayer1Card() ? "Player1" : "Player2";

            foreach (var player in FindObjectsOfType<PlayerInput>())
            {
                var manager = player.GetComponent<InputSystemManagerEventsBased>();
                if (manager != null && manager.PlayerID == neededPlayerID)
                {
                    return player; 
                }
            }
            return null; 
        }

        private CardToHand FindCorrectCardToHand()
        {
            // Durchsuche alle CardToHand-Skripte in der Szene
            foreach (var cth in FindObjectsOfType<CardToHand>())
            {
                // Wenn sie denselben isPlayerOne-Wert haben, ist es die richtige Hand
                if (cth.isPlayerOne == this.isPlayerOne)
                {
                    return cth;
                }
            }
            return null;
        }
    }
}
