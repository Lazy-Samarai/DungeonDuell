using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace dungeonduell
{
    public class TilMapHoverTest : MonoBehaviour, IObserver
    {
        private static readonly int Visible = Animator.StringToHash(VisualAniNameRef);
        private const string VisualAniNameRef = "Visible";
        public Camera cam;
        public TileBase hoverTile;

        public Tilemap tilemap;

        public Vector3Int currentCellPosition;

        public DoorIndicator indicatorDoorPrefab;
        public DoorIndicator runTimeIndicatorDoor;

        [TagField] [SerializeField] private string cursorTag; // To Not Lose Refrence to Curi
        public GameObject controllerCursor;

        private Animator _animator;

        private bool _currentlyVisible;

        public TileBase hoverBridgeTile;
        private bool _isBridged;

        private void Awake()
        {
            runTimeIndicatorDoor = Instantiate(indicatorDoorPrefab, transform.position, Quaternion.identity);
            runTimeIndicatorDoor.transform.parent = transform;
            runTimeIndicatorDoor.transform.gameObject.SetActive(false);
            _animator = GetComponent<Animator>();
            tilemap = GetComponent<Tilemap>();
        }

        private void Update()
        {
            if (_currentlyVisible)
            {
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
            DdCodeEventHandler.CardSelected += OnCardSelected;
            DdCodeEventHandler.CardRotating += UpdateIndicator;
            DdCodeEventHandler.BridgeMode += GoBridgeMode;
        }

        public void UnsubscribeToAllEvents()
        {
            DdCodeEventHandler.CardPlayed -= OnCardPlayed;
            DdCodeEventHandler.CardSelected -= OnCardSelected;
            DdCodeEventHandler.CardRotating -= UpdateIndicator;
            DdCodeEventHandler.BridgeMode -= GoBridgeMode;
        }

        private void HandleHover(Vector3 mouseWorldPos)
        {
            var cellPosition =
                tilemap.WorldToCell(new Vector3(mouseWorldPos.x, mouseWorldPos.y, cam.transform.position.z));

            ResetTileCheck();

            tilemap.SetTile(cellPosition, _isBridged ? hoverBridgeTile : hoverTile);
            runTimeIndicatorDoor.transform.position = tilemap.CellToWorld(cellPosition);
            runTimeIndicatorDoor.transform.gameObject.SetActive(_currentlyVisible);

            currentCellPosition = cellPosition;
        }

        private void ResetTileCheck()
        {
            tilemap.SetTile(currentCellPosition, null);
        }

        private void SetHoverMapVisable(bool visual)
        {
            runTimeIndicatorDoor.transform.gameObject.SetActive(visual);
            _animator.SetBool(Visible, visual);
            _currentlyVisible = visual;
        }

        private void OnCardPlayed(Card card, bool p)
        {
            _isBridged = false;
            ResetTileCheck();
            SetHoverMapVisable(false);
        }

        private void OnCardSelected(DisplayCard displayCard)
        {
            if (displayCard != null)
            {
                hoverTile = displayCard.card.tile;
                SetHoverMapVisable(true);

                UpdateIndicator(displayCard.card.GetAllowedDirection());
            }
        }

        private void UpdateIndicator(bool[] allowedDoors)
        {
            runTimeIndicatorDoor.SetDoorIndiactor(_isBridged
                ? new[] { false, false, false, false, false, false }
                : allowedDoors);
        }

        private void GoBridgeMode()
        {
            _isBridged = true;
        }
    }
}