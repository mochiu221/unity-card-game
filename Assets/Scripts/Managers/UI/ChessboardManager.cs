using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.Netcode;

public class ChessboardManager : NetworkBehaviour
{
    public static ChessboardManager Instance { set; get; }

    public const int TILE_COUNT_X = 3;
    public const int TILE_COUNT_Y = 3;

    [Header("Tiles")]
    public GameObject tile1A;
    public GameObject tile1B;
    public GameObject tile1C;
    public GameObject tile2A;
    public GameObject tile2B;
    public GameObject tile2C;
    public GameObject tile3A;
    public GameObject tile3B;
    public GameObject tile3C;

    [Header("Tile types")]
    public TileAsset tileTypeWhiteWorld;
    public TileAsset tileTypeForest;

    [Header("Table")]
    public BoxCollider playCardArea;
    public Chessboard chessboard; // Chessboard logic

    // Init on awake
    private GameObject[,] tiles = new GameObject[TILE_COUNT_X,TILE_COUNT_Y];
    public OneTileManager[,] tilesManager = new OneTileManager[TILE_COUNT_X,TILE_COUNT_Y];
    public GameObject[,] creaturesOnTile = new GameObject[TILE_COUNT_X,TILE_COUNT_Y];

    private void Awake() 
    {
        SetTilesRef();
        Instance = this;
    }

    private void Start() 
    {
        if (GameModeManager.Instance.GetGameMode() == GameMode.Multiplayer)
        {
            if (PlayersManager.Instance.myPlayer.playerTeam == PlayerTeam.Blue)
            {
                RotateTheChessboard();
            }
        }
    }

    // CURSOR/MOUSE DETECTION
    void Update()
    {
        
    }

    private void RotateTheChessboard()
    {
        Vector3 tile1APos = tile1A.transform.position;
        Vector3 tile1BPos = tile1B.transform.position;
        Vector3 tile1CPos = tile1C.transform.position;
        Vector3 tile2APos = tile2A.transform.position;
        Vector3 tile2CPos = tile2C.transform.position;
        Vector3 tile3APos = tile3A.transform.position;
        Vector3 tile3BPos = tile3B.transform.position;
        Vector3 tile3CPos = tile3C.transform.position;

        tile1A.transform.position = tile3CPos;
        tile1B.transform.position = tile3BPos;
        tile1C.transform.position = tile3APos;
        tile2A.transform.position = tile2CPos;
        tile2C.transform.position = tile2APos;
        tile3A.transform.position = tile1CPos;
        tile3B.transform.position = tile1BPos;
        tile3C.transform.position = tile1APos;
    }

    // Init chessboard manager
    private void SetTilesRef()
    {
        // Set tiles
        tiles[0,0] = tile1A;
        tiles[1,0] = tile1B;
        tiles[2,0] = tile1C;
        tiles[0,1] = tile2A;
        tiles[1,1] = tile2B;
        tiles[2,1] = tile2C;
        tiles[0,2] = tile3A;
        tiles[1,2] = tile3B;
        tiles[2,2] = tile3C;

        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                // Set tile index for each single tile manager
                OneTileManager otm = tiles[x,y].GetComponent<OneTileManager>();
                otm.thisTileIndex = new Vector2Int(x,y);
                // Pass tile manager to this chessboard manager
                tilesManager[x,y] = otm;

                otm.tileAsset = tileTypeWhiteWorld;
                otm.ReadTileFromAsset();

                // Logic
                chessboard.tileAssets[x,y] = tileTypeWhiteWorld;
            }
        }
    }

    // Check if hover table or tile
    public bool IsCursorOverTable()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

        foreach (RaycastHit h in hits)
        {
            // check if the collider that we hit is the collider on this table
            if (h.collider == playCardArea)
                return true;
            
        }
        return false;
    }
    public OneTileManager GetCursorOverTile()
    {
        RaycastHit[] hits;
        hits = Physics.RaycastAll(Camera.main.ScreenPointToRay(Input.mousePosition), 30f);

        foreach (RaycastHit h in hits)
        {
            if (h.transform.gameObject.GetComponent<OneTileManager>() != null)
                return h.transform.gameObject.GetComponent<OneTileManager>();
        }
        return null;
    }

    // Get tile info
    public Transform GetTileSlot(int x, int y)
    {
        return tilesManager[x,y].slot;
    }
    public int GetEmptyTileCount()
    {
        int emptyTileCount = tiles.Length;
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (GetTileSlot(x, y).childCount > 0)
                {
                    emptyTileCount--;
                };
            }
        }
        return emptyTileCount;
    }

    // Check tile validation
    public bool VaildTileToPlaceCreature(CardLogic cardLogic, Vector2Int tileIndex)
    {
        CreatureLogic cl = chessboard.creaturesOnTile[tileIndex.x,tileIndex.y];
        CardAsset ca = cardLogic.cardAsset;
        int manaLeft = TurnManager.Instance.whoseTurn.ManaLeft;

        bool isMyCreature = cl != null && (!cl.IsKing && cl.owner.playerTeam == TurnManager.Instance.whoseTurn.playerTeam);
        bool enoughMana = manaLeft >= ca.manaCost;
        if (isMyCreature)
        {
            int cost = ca.manaCost;
            if (cl.effect != null)
            {
                cost += cl.effect.GetEffectCostChangeToCard(cardLogic);
            }
            enoughMana = manaLeft >= cost - cl.resummonCostReduction;
        }
        bool isEnemyCreature = cl != null && (!cl.IsKing && cl.owner.playerTeam != TurnManager.Instance.whoseTurn.playerTeam);
        bool canInvade = false;
        if (isEnemyCreature && ca.invasion)
        {
            canInvade = ca.attack >= cl.Health;
        }
        return enoughMana && (cl == null || isMyCreature || canInvade);
    }
    public bool VaildTileToPlaceCreature(CardLogic cardLogic, int x, int y)
    {
        return VaildTileToPlaceCreature(cardLogic, new Vector2Int(x,y));
    }
    public List<Vector2Int> GetValidTilesToPlaceCreature(CardLogic cardLogic)
    {
        List<Vector2Int> validTiles = new List<Vector2Int>();
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (VaildTileToPlaceCreature(cardLogic, x, y))
                {
                    validTiles.Add(new Vector2Int(x,y));
                }
            }
        }
        return validTiles;
    }

    public List<Vector2Int> GetAllValidTilesToActivateEffect(CardLogic cl)
    {
        List<Vector2Int> tilesList = new List<Vector2Int>();
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (cl.effect != null && cl.effect.IsEffectTarget(new Vector2Int(x,y)))
                {
                    tilesList.Add(new Vector2Int(x,y));
                }
            }
        }
        return tilesList;
    }
    public bool VaildToUseSpell(CardLogic cl)
    {
        // Check if enough Mana
        int manaLeft = TurnManager.Instance.whoseTurn.ManaLeft;
        bool enoughMana = manaLeft >= cl.CurrentManaCost;

        // Check if enough target amount
        int totalTargetNeeded = cl.cardAsset.numberOfTarget;
        bool enoughTarget = true;
        if (cl.effect == null || cl.effect.hasTarget)
        {
            enoughTarget = GetAllValidTilesToActivateEffect(cl).Count >= totalTargetNeeded;
        }

        return enoughMana && enoughTarget;
    }
    public bool VaildTileToActivateEffect(Vector2Int tileIndex, CardLogic cl)
    {
        return VaildTileToActivateEffect(tileIndex.x, tileIndex.y, cl);
    }
    public bool VaildTileToActivateEffect(int x, int y, CardLogic cl)
    {
        bool isValid = false;
        if (cl.effect != null && cl.effect.IsEffectTarget(new Vector2Int(x,y)))
        {
            isValid = true;
        }
        return isValid;
    }

    public List<Vector2Int> GetValidTilesToAttackOrMove(Vector2Int tileIndex, CreatureType ct, PlayerTeam pt)
    {
        List<Vector2Int> tilesList = new List<Vector2Int>();
        int dir = pt == PlayerTeam.Red ? 1 : -1;
        
        switch (ct)
        {
            case CreatureType.Soldier:
                AddValidTileIndexToList(tilesList, tileIndex.x - 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 1*dir, tileIndex.y + 1*dir, ct, pt);
                break;
            
            case CreatureType.Striker:
                // right
                for (int x = tileIndex.x; x < TILE_COUNT_X; x++)
                {
                    if (x != tileIndex.x)
                    {
                        AddValidTileIndexToList(tilesList, x, tileIndex.y, ct, pt);
                        if (chessboard.creaturesOnTile[x, tileIndex.y] != null)
                            break;
                    }
                }
                // left
                for (int x = tileIndex.x; x >= 0; x--)
                {
                    if (x != tileIndex.x)
                    {
                        AddValidTileIndexToList(tilesList, x, tileIndex.y, ct, pt);
                        if (chessboard.creaturesOnTile[x, tileIndex.y] != null)
                            break;
                    }
                }
                // up
                for (int y = tileIndex.y; y < TILE_COUNT_Y; y++)
                {
                    if (y != tileIndex.y)
                    {
                        AddValidTileIndexToList(tilesList, tileIndex.x, y, ct, pt);
                        if (chessboard.creaturesOnTile[tileIndex.x, y] != null)
                            break;
                    }
                }
                // down
                for (int y = tileIndex.y; y >= 0; y--)
                {
                    if (y != tileIndex.y)
                    {
                        AddValidTileIndexToList(tilesList, tileIndex.x, y, ct, pt);
                        if (chessboard.creaturesOnTile[tileIndex.x, y] != null)
                            break;
                    }
                }
                break;

            case CreatureType.Archer:
                AddValidTileIndexToList(tilesList, tileIndex.x - 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x - 2*dir, tileIndex.y + 2*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x, tileIndex.y + 2*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 2*dir, tileIndex.y + 2*dir, ct, pt);
                break;

            case CreatureType.Assassin:
                AddValidTileIndexToList(tilesList, tileIndex.x - 1*dir, tileIndex.y, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 1*dir, tileIndex.y, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x - 1*dir, tileIndex.y - 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x, tileIndex.y - 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 1*dir, tileIndex.y - 1*dir, ct, pt);
                break;

            case CreatureType.Throne:
                for (int x = 0; x < TILE_COUNT_X; x++)
                {
                    for (int y = 0; y < TILE_COUNT_Y; y++)
                    {
                        AddValidTileIndexToList(tilesList, x, y, ct, pt);
                    }
                }
                break;

            case CreatureType.King:
                AddValidTileIndexToList(tilesList, tileIndex.x - 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x - 1*dir, tileIndex.y, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 1*dir, tileIndex.y, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x - 1*dir, tileIndex.y - 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x, tileIndex.y - 1*dir, ct, pt);
                AddValidTileIndexToList(tilesList, tileIndex.x + 1*dir, tileIndex.y - 1*dir, ct, pt);
                break;

            default:
                break;
        }

        List<Vector2Int> allureCreaturesList = GetAllureCreaturesInsideARange(tilesList);
        if (allureCreaturesList.Count > 0)
        {
            return allureCreaturesList;
        }

        return tilesList;
    }
    private void AddValidTileIndexToList(List<Vector2Int> tilesList, int x, int y, CreatureType ct, PlayerTeam pt)
    {
        if (x >= 0 && x < TILE_COUNT_X && y >= 0 && y < TILE_COUNT_Y)
        {
            Vector2Int ti = new Vector2Int(x, y);
            if (ct != CreatureType.Throne)
            {
                // can move and attack
                if (chessboard.IsAValidTileToMove(ti) || chessboard.IsAValidTileToAttack(ti, pt))
                    tilesList.Add(ti);
            }
            else
            {
                // only can attack
                if (chessboard.IsAValidTileToAttack(ti, pt))
                    tilesList.Add(ti);
            }
        }
    }
    private List<Vector2Int> GetAllureCreaturesInsideARange(List<Vector2Int> tilesList)
    {
        List<Vector2Int> newList = new List<Vector2Int>();
        foreach (var tileIndex in tilesList)
        {
            if (chessboard.creaturesOnTile[tileIndex.x, tileIndex.y] != null && chessboard.creaturesOnTile[tileIndex.x, tileIndex.y].allure)
            {
                newList.Add(tileIndex);
            }
        }
        return newList;
    }

    public List<Vector2Int> GetAttackRange(Vector2Int tileIndex, CreatureType ct, PlayerTeam pt)
    {
        List<Vector2Int> tilesList = new List<Vector2Int>();
        int dir = pt == PlayerTeam.Red ? 1 : -1;

        if (tileIndex.x >= 0 && tileIndex.x < TILE_COUNT_X && tileIndex.y >= 0 && tileIndex.y < TILE_COUNT_Y)
        {

        }

        switch (ct)
        {
            case CreatureType.Soldier:
                AddAttackRangeTile(tilesList, tileIndex.x - 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 1*dir, tileIndex.y + 1*dir, ct, pt);
                break;
            
            case CreatureType.Striker:
                for (int x = 0; x < TILE_COUNT_X; x++)
                {
                    if (x != tileIndex.x)
                        AddAttackRangeTile(tilesList, x, tileIndex.y, ct, pt);
                    
                }
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (y != tileIndex.y)
                        AddAttackRangeTile(tilesList, tileIndex.x, y, ct, pt);
                }
                break;

            case CreatureType.Archer:
                AddAttackRangeTile(tilesList, tileIndex.x - 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x - 2*dir, tileIndex.y + 2*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x, tileIndex.y + 2*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 2*dir, tileIndex.y + 2*dir, ct, pt);
                break;

            case CreatureType.Assassin:
                AddAttackRangeTile(tilesList, tileIndex.x - 1*dir, tileIndex.y, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 1*dir, tileIndex.y, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x - 1*dir, tileIndex.y - 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x, tileIndex.y - 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 1*dir, tileIndex.y - 1*dir, ct, pt);
                break;

            case CreatureType.Throne:
                for (int x = 0; x < TILE_COUNT_X; x++)
                {
                    for (int y = 0; y < TILE_COUNT_Y; y++)
                    {
                        if (x != tileIndex.x && y != tileIndex.y)
                            AddAttackRangeTile(tilesList, x, y, ct, pt);
                    }
                }
                break;

            case CreatureType.King:
                AddAttackRangeTile(tilesList, tileIndex.x - 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 1*dir, tileIndex.y + 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x - 1*dir, tileIndex.y, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 1*dir, tileIndex.y, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x - 1*dir, tileIndex.y - 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x, tileIndex.y - 1*dir, ct, pt);
                AddAttackRangeTile(tilesList, tileIndex.x + 1*dir, tileIndex.y - 1*dir, ct, pt);
                break;

            default:
                break;
        }
        return tilesList;
    }
    private void AddAttackRangeTile(List<Vector2Int> tilesList, int x, int y, CreatureType ct, PlayerTeam pt)
    {
        if (x >= 0 && x < TILE_COUNT_X && y >= 0 && y < TILE_COUNT_Y)
        {
            Vector2Int ti = new Vector2Int(x, y);
            tilesList.Add(ti);
        }
    }

    // Highlight tiles
    public void HighlightTilesByIndexes(List<Vector2Int> validTileIndexes)
    {
        foreach (var i in validTileIndexes)
        {
            tilesManager[i.x, i.y].IsHighlight = true;
        }
    }
    public void HighlightValidTilesToPlaceCreature(CardLogic cl)
    {
        List<Vector2Int> tilesList = GetValidTilesToPlaceCreature(cl);
        foreach (var tileIndex in tilesList)
        {
            tilesManager[tileIndex.x,tileIndex.y].IsHighlight = true;
        }
    }
    public void HighlightEffectTargetValidTiles(CardLogic cl)
    {
        List<Vector2Int> tilesList = GetAllValidTilesToActivateEffect(cl);
        foreach (var tileIndex in tilesList)
        {
            tilesManager[tileIndex.x,tileIndex.y].IsHighlight = true;
        }
    }
    public void RemoveAllTileHighlight()
    {
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                if (tilesManager[x,y].IsHighlight)
                {
                    tilesManager[x,y].IsHighlight = false;
                };
            }
        }
    }

    // Add creature or king
    public void AddCreatureAtIndex(int x, int y, CardAsset cardAsset, int uniqueID, Player player)
    {
        // create a new creature from prefab
        GameObject creature = Instantiate(GameManager.Instance.creaturePrefab, GetTileSlot(x, y));

        // apply the look from CardAsset
        OneCreatureManager oneCreatureManager = creature.GetComponent<OneCreatureManager>();
        oneCreatureManager.cardAsset = cardAsset;
        oneCreatureManager.ReadCreatureFromAsset();

        // set player team & change creature color
        oneCreatureManager.playerTeam = player.playerTeam;
        if (player.playerTeam == PlayerTeam.Red)
        {
            oneCreatureManager.frameImage.color = PlayersManager.Instance.redTeamCreatureFrameColor;
        }
        else if (player.playerTeam == PlayerTeam.Blue)
        {
            oneCreatureManager.frameImage.color = PlayersManager.Instance.blueTeamCreatureFrameColor;
        }

        // Change creature frame by creature type
        if (PlayersManager.Instance.CanControlThisPlayer(player))
        {
            switch (cardAsset.creatureType)
            {
                case CreatureType.Lost:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.myLostFrame;
                    break;

                case CreatureType.Soldier:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.mySoldierFrame;
                    break;

                case CreatureType.Striker:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.myStrikerFrame;
                    break;

                case CreatureType.Archer:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.myArcherFrame;
                    break;

                case CreatureType.Assassin:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.myAssassinFrame;
                    break;

                case CreatureType.Throne:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.myThroneFrame;
                    break;

                default:
                    break;
            }
        }
        else
        {
            switch (cardAsset.creatureType)
            {
                case CreatureType.Lost:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.enemyLostFrame;
                    break;

                case CreatureType.Soldier:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.enemySoldierFrame;
                    break;

                case CreatureType.Striker:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.enemyStrikerFrame;
                    break;

                case CreatureType.Archer:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.enemyArcherFrame;
                    break;

                case CreatureType.Assassin:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.enemyAssassinFrame;
                    break;

                case CreatureType.Throne:
                    oneCreatureManager.frameImage.sprite = CreaturesManager.Instance.enemyThroneFrame;
                    break;

                default:
                    break;
            }
        }

        // add a new creature to the array
        creaturesOnTile[x, y] = creature;

        // add our unique ID to this creature
        IDHolder id = creature.AddComponent<IDHolder>();
        id.UniqueID = uniqueID;

        // Animation
        creature.transform.localPosition = new Vector3(0, 0, -1f);
        Sequence s = DOTween.Sequence();
        s.Append(creature.transform.DOLocalMoveZ(0, 0.3f).SetEase(Ease.InBack));
        s.OnComplete(() => {
            // end command execution
            Command.CommandExecutionComplete();
        });
    }
    public void AddKingAtIndex(int x, int y, int uniqueID, Player player)
    { 
        // create new king from prefab
        GameObject king = Instantiate(GameManager.Instance.kingPrefab, GetTileSlot(x, y));

        // apply the look from CardAsset
        OneCreatureManager oneCreatureManager = king.GetComponent<OneCreatureManager>();
        oneCreatureManager.charAsset = player.charAsset;
        oneCreatureManager.ReadCharacterFromAsset();

        // set player team & change creature color
        oneCreatureManager.playerTeam = player.playerTeam;
        if (player.playerTeam == PlayerTeam.Red)
        {
            oneCreatureManager.frameImage.color = PlayersManager.Instance.redTeamCreatureFrameColor;
            oneCreatureManager.crownImage.color = PlayersManager.Instance.redTeamCreatureFrameColor;
        }
        else if (player.playerTeam == PlayerTeam.Blue)
        {
            oneCreatureManager.frameImage.color = PlayersManager.Instance.blueTeamCreatureFrameColor;
            oneCreatureManager.crownImage.color = PlayersManager.Instance.blueTeamCreatureFrameColor;
        }

        // add king to the array
        creaturesOnTile[x, y] = king;

        // add our unique ID to this creature
        IDHolder id = king.AddComponent<IDHolder>();
        id.UniqueID = uniqueID;

        // add king to the player
        player.king = king;

        // end command execution
        Command.CommandExecutionComplete();
    }

    // Move creature
    public void MoveCreature(Vector2Int currentIndex, Vector2Int newIndex)
    {
        int oldX = currentIndex.x;
        int oldY = currentIndex.y;
        int newX = newIndex.x;
        int newY = newIndex.y;
        Transform newPos = GetTileSlot(newX, newY);
        if (GetTileSlot(oldX, oldY).childCount > 0){
            GameObject creature = GetTileSlot(oldX, oldY).GetChild(0).gameObject;

            CreatureVisualController cvc = creature.GetComponent<CreatureVisualController>();
            cvc.BringToFront();
            CreatureVisualStates tempState = cvc.VisualState;
            cvc.VisualState = CreatureVisualStates.Transition;

            creature.transform.DOMove(newPos.position, 0.4f).SetEase(Ease.OutSine).OnComplete(() => {
                // After move
                cvc.SetTableSortingOrder();
                cvc.VisualState = tempState;

                creaturesOnTile[oldX, oldY] = null;
                creaturesOnTile[newX, newY] = creature;
                creature.transform.SetParent(newPos);
            });
        }
        
        Command.CommandExecutionComplete();
    }

    // Destroy a creature
    public void RemoveCreatureByTileIndex(Vector2Int tileIndex)
    {
        // TODO: This has to last for some time
        // Adding delay here did not work because it shows one creature die, then another creature die. 
        // 
        //Sequence s = DOTween.Sequence();
        //s.AppendInterval(1f);
        //s.OnComplete(() =>
        //   {
                
        //    });
        GameObject creatureToRemove = creaturesOnTile[tileIndex.x, tileIndex.y];
        Destroy(creatureToRemove);

        Command.CommandExecutionComplete();
    }

    // Change tile world
    public void ChangeTileByIndex(Vector2Int tileIndex, TileAsset tileAsset)
    {
        OneTileManager otm = tilesManager[tileIndex.x, tileIndex.y];
        otm.tileAsset = tileAsset;
        otm.ReadTileFromAsset();

        Command.CommandExecutionComplete();
    }


}