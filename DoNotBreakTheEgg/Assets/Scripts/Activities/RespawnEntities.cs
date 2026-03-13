using R3;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RespawnEntities : MonoBehaviour
{

    [SerializeField] GameObject transporterPadPrefab;
    [SerializeField] LayerMask spawnLayerMask;
    [SerializeField] float moveSpeed;
    [SerializeField] float stoppingDistance = 0.1f;

    [SerializeField] Tilemap tilemap;

    [SerializeField] TagScriptableObject respawnTag;

    [Header("Audio")]
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData destinationReachedSoundData;


    Subject<(Transform, Vector2, Vector2, IEntity)> moveStream = new Subject<(Transform, Vector2, Vector2, IEntity)>();

    Subject<(IEntity, Transform)> destinationReached = new Subject<(IEntity, Transform)>();

    private void Start()
    {

        var subscriptionBag = Disposable.CreateBuilder();

        moveStream
            .SelectMany(tuple =>
            {
                var (transporterPad, spawnLocation, landingZone, entity) = tuple;

                var adjustedLandingZone = new Vector3(landingZone.x, landingZone.y + 1); 

                return Observable
                    .EveryUpdate()
                    .Scan(0f, (acc, _) => acc + Time.deltaTime * moveSpeed)
                    .Select(t => (transporterPad, spawnLocation, adjustedLandingZone, entity, t))
                    .TakeUntil(destinationReached.Where(reachedTuple => reachedTuple.Item1 == tuple.Item4));
            })
            .Subscribe(tuple =>
            {

                SoundStreams.instance.PlaySound(destinationReachedSoundData);

                var (transporterPad, spawnLocation, landingZone, entity, t) = tuple;

                // Ensure t doesn't exceed 1, so it stays between 0 and 1 for Lerp
                t = Mathf.Clamp01(t);

                // Use Lerp to smoothly transition between point A and point B
                transporterPad.position = Vector3.Lerp(spawnLocation, landingZone, t);

                if (Vector2.Distance(transporterPad.position, landingZone) < stoppingDistance)
                {
                    destinationReached.OnNext((entity, transporterPad));
                }
            })
            .AddTo(ref subscriptionBag);

        destinationReached
            .Subscribe(tuple =>
            {
                var (entity, transporterPad) = tuple;

                var movementComponent = entity.GetEntityComponent<IMovementComponent>();

                movementComponent.EnableGravity();
                movementComponent.EnableMovement();
                entity.GetEntityComponent<IAnchoringComponent>().SetParent(null);
                entity.GetEntityComponent<ITagComponent>().RemoveTag(respawnTag);
                Destroy(transporterPad.gameObject);
            })
            .AddTo(ref subscriptionBag);

        subscriptionBag.RegisterTo(this.destroyCancellationToken);
    }

    public void RespawnEntity(IEntity entity)
    {

        entity.GetEntityComponent<ITagComponent>().AddTag(respawnTag);

        var spawnLocation = CameraUtility.GetRandomLocationOutsideViewport(Camera.main, 1, ViewportSide.XAxis);

        var landingZone = FindLandingZone(spawnLocation);

        var transportPad = Instantiate(transporterPadPrefab, spawnLocation, Quaternion.identity).transform;

        var anchoringComponent = entity.GetEntityComponent<IAnchoringComponent>();
        var movementComponent = entity.GetEntityComponent<IMovementComponent>();

        anchoringComponent.SetPosition(transportPad.position);
        anchoringComponent.SetParent(transportPad);
        movementComponent.DisableMovement();
        movementComponent.DisableGravity();

        //tween to the location
        moveStream.OnNext((transportPad, spawnLocation, landingZone, entity));
    }

    Vector2 FindLandingZone(Vector2 location)
    {
        // Get screen bounds in world space
        Vector2 screenMin = Camera.main.ViewportToWorldPoint(Vector2.zero);
        Vector2 screenMax = Camera.main.ViewportToWorldPoint(Vector2.one);

        int verticalLayerCount = 10;  // How many height levels to check (e.g., 0.1 to 1)
        int horizontalRayCount = 20;  // How many rays per height level

        float startHeightNormalized = 0.1f;
        float endHeightNormalized = 1f;

        float heightStep = (endHeightNormalized - startHeightNormalized) / (verticalLayerCount - 1);
        float horizontalSpacing = (screenMax.x - screenMin.x) / (horizontalRayCount - 1);

        // Check if location is on the left or right side of the screen
        bool isOnLeftSide = location.x < (screenMin.x + screenMax.x) / 2;

        for (int v = 0; v < verticalLayerCount; v++)
        {
            float yNormalized = startHeightNormalized + v * heightStep;
            float worldY = Camera.main.ViewportToWorldPoint(new Vector2(0, yNormalized)).y;

            for (int h = 0; h < horizontalRayCount; h++)
            {
                // Calculate the X position
                float x = isOnLeftSide ? screenMin.x + h * horizontalSpacing : screenMax.x - h * horizontalSpacing;
                Vector2 rayOrigin = new Vector2(x, worldY);

                // Calculate ray length to stay within screen bounds
                float rayLength = worldY - screenMin.y;

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, spawnLayerMask);
                Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red, 2f);

                if (hit.collider != null)
                {
                    // Get the tile position of the hit point
                    Vector3Int hitTilePosition = tilemap.WorldToCell(hit.point);

                    // Calculate the tile position directly above the hit tile (adjust Y by 1 unit)
                    Vector3Int tileAbovePosition = new Vector3Int(hitTilePosition.x, hitTilePosition.y + 1, hitTilePosition.z);

                    // Check if there's a tile above the hit tile
                    TileBase tileAbove = tilemap.GetTile(tileAbovePosition);

                    if (tileAbove != null)
                    {
                        // If there is a tile above, skip this hit
                        continue;  // Skip this hit if the tile above exists
                    }

                    // Return the valid hit point if no tile is above
                    return hit.point;
                }
            }
        }

        return screenMin; // Fallback
    }
}
