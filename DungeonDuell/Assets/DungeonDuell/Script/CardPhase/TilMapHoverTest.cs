using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class TilMapHoverTest : MonoBehaviour, IObserver
    {
        private const string VisualAniNameRef = "Visible";
        public Camera cam;
        public TileBase hoverTile;

        public TileBase blockTile;

        public Tilemap tilemap;

        public Vector3Int currentCellPosition;

        public DoorIndicator indiactorDoorPrefab;
        public DoorIndicator runTimeIndicatorDoor;

        [TagField] [SerializeField] private string cursorTag; // To Not Lose Refrence to Curi
        public GameObject controllerCursor;

        private Animator _animator;

        private bool _currentlyvisble;

        private void Awake()
        {
            runTimeIndicatorDoor = Instantiate(indiactorDoorPrefab, transform.position, Quaternion.identity);
            runTimeIndicatorDoor.transform.parent = transform;
            runTimeIndicatorDoor.transform.gameObject.SetActive(false);
            _animator = GetComponent<Animator>();
            tilemap = GetComponent<Tilemap>();
        }

        private void Update()
        {
            if (_currentlyvisble)
            {
                var mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                    -cam.transform.position.z));
                HandleHover(mouseWorldPos);
                if (controllerCursor == null) controllerCursor = GameObject.FindGameObjectWithTag(cursorTag);
                HandleHover(controllerCursor.transform.position);
            }
        }

        private void OnEnable()
        {
            cam = FindAnyObjectByType<Camera>();

            SubscribeToEvents();
        }

        private void OnDisable()
        {
            UnsubscribeToAllEvents();
        }

        public void SubscribeToEvents()
        {
            DdCodeEventHandler.CardPlayed += OnCardPlayed;
            DdCodeEventHandler.PreSetCardSetOnTilemap += OnPreSetCardSetOnTilemap;
            DdCodeEventHandler.CardSelected += OnCardSelected;
            DdCodeEventHandler.CardRotating += UpdateIndicator;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.CardPlayed -= OnCardPlayed;
            DdCodeEventHandler.PreSetCardSetOnTilemap -= OnPreSetCardSetOnTilemap;
            DdCodeEventHandler.CardSelected -= OnCardSelected;
            DdCodeEventHandler.CardRotating -= UpdateIndicator;
        }

        private void HandleHover(Vector3 mouseWorldPos)
        {
            var cellPosition =
                tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

            if (cellPosition != currentCellPosition)
            {
                if (tilemap.GetTile(cellPosition) != blockTile)
                {
                    ResetTileCheck();
                    tilemap.SetTile(cellPosition, hoverTile);
                    runTimeIndicatorDoor.transform.position = tilemap.CellToWorld(cellPosition);
                    runTimeIndicatorDoor.transform.gameObject.SetActive(_currentlyvisble ? true : false);
                }
                else
                {
                    ResetTileCheck();
                    runTimeIndicatorDoor.transform.gameObject.SetActive(false);
                }

                currentCellPosition = cellPosition;
            }
        }

        private void ResetTileCheck()
        {
            if (tilemap.GetTile(currentCellPosition) != blockTile) tilemap.SetTile(currentCellPosition, null);
        }

        public void SetHoverMapVisable(bool visual)
        {
            runTimeIndicatorDoor.transform.gameObject.SetActive(visual);
            _animator.SetBool(VisualAniNameRef, visual);
            _currentlyvisble = visual;
        }

        public void OnCardPlayed(Card card, bool p)
        {
            SetBlockOnTile(currentCellPosition);
            SetHoverMapVisable(false);
        }

        public void OnCardSelected(DisplayCard displayCard)
        {
            if (displayCard != null)
            {
                hoverTile = displayCard.card.tile;
                SetHoverMapVisable(true);

                UpdateIndicator(displayCard.card.GetAllowedDirection());
            }
        }

        public void UpdateIndicator(bool[] allowedDoors)
        {
            runTimeIndicatorDoor.SetDoorIndiactor(allowedDoors);
        }

        public void OnPreSetCardSetOnTilemap(Card card, Vector3Int point)
        {
            SetBlockOnTile(point);
        }

        private void SetBlockOnTile(Vector3Int postion)
        {
            tilemap.SetTile(postion, blockTile);
        }
    }
}