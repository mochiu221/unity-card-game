using UnityEngine;

public class CursorClickController : MonoBehaviour
{
    Camera mainCamera;

    // Logic
    public bool isMouseClickable = true;

    public GameObject selectedObject;
    private bool isMouseDown = false;
    private bool isMoved;
    private Vector2 mouseDownPos;
    private float isClickDistance = 2f;
    private float currentDistance = 0f;

    private void Awake() 
    {
        mainCamera = Camera.main;
    }

    private void Update() 
    {
        // Click action
        if (isMouseClickable)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isMouseDown = true;
                mouseDownPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }

            if (isMouseDown)
            {
                currentDistance =  Vector2.Distance( new Vector2(Input.mousePosition.x, Input.mousePosition.y), mouseDownPos);
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (currentDistance < isClickDistance)
                {
                    onMouseClick();
                }
                if (isMouseDown)
                {
                    isMouseDown = false;
                    currentDistance = 0f;
                }
            }
        }
    }

    private void onMouseClick()
    {
        if (!UIManager.Instance.IsSelectingTile)
        {
            // Reset selected object
            selectedObject = null;

            // Get the selected object
            RaycastHit[] hits;
            hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

            foreach (RaycastHit h in hits)
            {
                GameObject obj = h.transform.gameObject;
                if (obj.GetComponent<OneCardManager>() != null)
                {
                    selectedObject = obj;
                    break;
                }
                if (obj.GetComponentInParent<OneCreatureManager>() != null)
                {
                    selectedObject = obj.transform.parent.gameObject;
                    break;
                }
            }

            // Preview card panel
            if (selectedObject != null)
            {
                OneCardManager oneCardManager = selectedObject.GetComponent<OneCardManager>();
                OneCreatureManager oneCreatureManager = selectedObject.GetComponent<OneCreatureManager>();
                if ((oneCardManager != null && PlayersManager.Instance.CanViewThisPlayerHand(oneCardManager.playerTeam)) || oneCreatureManager != null)
                {
                    UIManager.Instance.StartPreviewCard(selectedObject);
                }
                else if (selectedObject.name == "PreviewCardPanel")
                {
                    // do nothing
                }
                else
                {
                    UIManager.Instance.StopPreviewCard();
                }
            }
            else
            {
                UIManager.Instance.StopPreviewCard();
            }
        }

        // Select target tiles
        if (UIManager.Instance.IsSelectingTile && UIManager.Instance.SelectingTileCardDA != null)
        {
            CardDraggingActions da = UIManager.Instance.SelectingTileCardDA;
            CardLogic cl = da.GetCardLogic();

            OneTileManager oneTileManager = ChessboardManager.Instance.GetCursorOverTile();
            
            if (oneTileManager != null)
            {
                if (ChessboardManager.Instance.VaildTileToActivateEffect(oneTileManager.thisTileIndex, cl))
                {
                    if (!da.tilesList.Contains(oneTileManager))
                    {
                        da.tilesList.Add(oneTileManager);
                        oneTileManager.IsSelected = true;
                        if (da.numberOfTarget == da.tilesList.Count) // selected all targets
                        {
                            UIManager.Instance.SelectingTileCardDA = null;
                            da.PlayCardOnTarget();
                        }
                    }
                    else
                    {
                        da.tilesList.Remove(oneTileManager);
                        oneTileManager.IsSelected = false;
                    }
                }
                else
                {
                    UIManager.Instance.SelectingTileCardDA = null;
                    da.BackToHand();
                }
            }
            else
            {
                UIManager.Instance.SelectingTileCardDA = null;
                da.BackToHand();
            }
            
        }
    }
}
