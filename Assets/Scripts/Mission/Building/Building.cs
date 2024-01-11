using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Building : MonoBehaviour
{
    public UnityEvent<Building> OnBuildingDestroyed = new UnityEvent<Building>();

    public int hitPoints = 10;
    public bool isBeingPlaced = false;
    public BuildingType buildingType;
    [HideInInspector] public bool isTargeted;

    private SpriteRenderer spriteRenderer;

    public Sprite vertWallSprite;
    public Sprite vertWallCapTopSprite;
    public Sprite vertWallCapBotSprite;
    public Sprite horizWallSprite;
    public Sprite horizWallCapRightSprite;
    public Sprite horizWallCapLeftSprite;
    public Sprite topRightWallCornerSprite;
    public Sprite topLeftWallCornerSprite;
    public Sprite bottomRightWallCornerSprite;
    public Sprite bottomLeftWallCornerSprite;
    public Sprite singleWallSprite;
    public Sprite centerWallSprite;
    public Sprite centerRightWallSprite;
    public Sprite centerLeftWallSprite;
    public Sprite centerTopWallSprite;
    public Sprite centerBottomWallSprite;

    public enum BuildingType
    {
        Wall,
        Barricades,
        Stairs
    }

    private void Start()
    {
        // would have a child script for wall specific functionality, but since this building stuff is still getting figured out, not doing it yet.
        if (buildingType == BuildingType.Wall)
        {
            BuildingManager.Instance.OnBuildingModified.AddListener(HandleWallPlaced);
        }

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnDestroy()
    {
        if (buildingType == BuildingType.Wall)
        {
            BuildingManager.Instance.OnBuildingModified.RemoveListener(HandleWallPlaced);
        }

        OnBuildingDestroyed.RemoveAllListeners();
    }

    public void HandleWallPlaced()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        bool isUpOccupiedByWall = BuildingManager.Instance.IsSpaceOccupiedBy(new Vector3(transform.position.x, transform.position.y, transform.position.z + 5f), buildingType);
        bool isRightOccupiedByWall = BuildingManager.Instance.IsSpaceOccupiedBy(new Vector3(transform.position.x + 5f, transform.position.y, transform.position.z), buildingType);
        bool isLeftOccupiedByWall = BuildingManager.Instance.IsSpaceOccupiedBy(new Vector3(transform.position.x - 5f, transform.position.y, transform.position.z), buildingType);
        bool isDownOccupiedByWall = BuildingManager.Instance.IsSpaceOccupiedBy(new Vector3(transform.position.x, transform.position.y, transform.position.z - 5f), buildingType);

        // pick the sprite for this wall depending on which other nearby positions are occupied.

        if (!isRightOccupiedByWall && !isLeftOccupiedByWall && !isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = singleWallSprite;
        }
        else if (isRightOccupiedByWall && !isLeftOccupiedByWall && !isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = horizWallCapLeftSprite;
        }
        else if (isRightOccupiedByWall && isLeftOccupiedByWall && !isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = horizWallSprite;
        }
        else if (!isRightOccupiedByWall && isLeftOccupiedByWall && !isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = horizWallCapRightSprite;
        }
        else if (!isRightOccupiedByWall && !isLeftOccupiedByWall && isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = vertWallSprite;
        }
        else if (!isRightOccupiedByWall && !isLeftOccupiedByWall && !isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = vertWallCapTopSprite;
        }
        else if (!isRightOccupiedByWall && !isLeftOccupiedByWall && isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = vertWallCapBotSprite;
        }
        else if (!isRightOccupiedByWall && isLeftOccupiedByWall && !isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = topRightWallCornerSprite;
        }
        else if (isRightOccupiedByWall && !isLeftOccupiedByWall && !isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = topLeftWallCornerSprite;
        }
        else if (isRightOccupiedByWall && !isLeftOccupiedByWall && isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = bottomLeftWallCornerSprite;
        }
        else if (!isRightOccupiedByWall && isLeftOccupiedByWall && isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = bottomRightWallCornerSprite;
        }
        else if (isRightOccupiedByWall && isLeftOccupiedByWall && isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = centerWallSprite;
        }
        else if (!isRightOccupiedByWall && isLeftOccupiedByWall && isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = centerRightWallSprite;
        }
        else if (isRightOccupiedByWall && !isLeftOccupiedByWall && isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = centerLeftWallSprite;
        }
        else if (isRightOccupiedByWall && isLeftOccupiedByWall && !isUpOccupiedByWall && isDownOccupiedByWall)
        {
            spriteRenderer.sprite = centerTopWallSprite;
        }
        else if (isRightOccupiedByWall && isLeftOccupiedByWall && isUpOccupiedByWall && !isDownOccupiedByWall)
        {
            spriteRenderer.sprite = centerBottomWallSprite;
        }
    }



    private void Update()
    {
        if (hitPoints <= 0)
        {
            GetComponentInChildren<Collider>().enabled = false;

            OnBuildingDestroyed.Invoke(this);

            Destroy(gameObject);
        }

        if (isBeingPlaced)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = BuildingManager.Instance.GetClosestSnapPos(mousePos);
        }
    }
}
