using R3;
using System.ComponentModel;
using UnityEngine;


public class RespawnEntities : MonoBehaviour
{

    [SerializeField] GameObject transporterPadPrefab;
    [SerializeField] LayerMask spawnLayerMask;
    [SerializeField] float moveSpeed;
    [SerializeField] float stoppingDistance;

    Subject<(Transform, Vector2, Vector2, IEntity)> moveStream = new Subject<(Transform, Vector2, Vector2, IEntity)>();

    Subject<(IEntity, Transform)> destinationReached = new Subject<(IEntity, Transform)>();

    private void Start()
    {

        moveStream
            .SelectMany(tuple =>
            {
                var (transporterPad, spawnLocation, landingZone, entity) = tuple;

                return Observable
                    .EveryUpdate()
                    .Scan(0f, (acc, _) => acc + Time.deltaTime * moveSpeed)
                    .Select(t => (transporterPad, spawnLocation, landingZone, entity, t))
                    .TakeUntil(destinationReached);
            })
            .TakeUntilDestroy(this)
            .Subscribe(tuple =>
            {

                var (transporterPad, spawnLocation, landingZone, entity, t) = tuple;

                // Ensure t doesn't exceed 1, so it stays between 0 and 1 for Lerp
                t = Mathf.Clamp01(t);

                // Use Lerp to smoothly transition between point A and point B
                transporterPad.position = Vector3.Lerp(spawnLocation, landingZone, t);

                if(Vector2.Distance(transporterPad.position, landingZone) < stoppingDistance)
                {
                    destinationReached.OnNext((entity, transporterPad));
                }
            });

        destinationReached
            .TakeUntilDestroy(this)
            .Subscribe(tuple =>
            {
                var (entity, transporterPad) = tuple;

                var movementComponent = entity.GetEntityComponent<IMovementComponent>();

                movementComponent.EnableGravity();
                movementComponent.EnableMovement();
                entity.GetEntityComponent<IAnchoringComponent>().SetParent(null);
                EntityRegistry.RegisterEntity(entity);
                Destroy(transporterPad.gameObject);
            });
    }

    public void RespawnEntity(IEntity entity)
    {
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

        int verticalRayCount = 20;
        float yOffset = 0.05f;
        float xOffset = 2;

        // Spacing between each ray along the top of the screen
        float verticalRaySpacing = (screenMax.x - screenMin.x) / (verticalRayCount - 1);

        // Check if location is on the left or right side of the screen
        bool isOnLeftSide = location.x < (screenMin.x + screenMax.x) / 2;

        // Calculate the initial ray starting x position based on the side
        float initialXPos = isOnLeftSide ? screenMin.x + xOffset : screenMax.x - xOffset;

        // Adjust the spacing direction based on the side
        float xDirection = isOnLeftSide ? 1 : -1;

        for (int i = 0; i < verticalRayCount; i++)
        {
            // Calculate x position for each ray along the top of the screen, moving towards the center
            float xPos = initialXPos + (i * verticalRaySpacing * xDirection);
            Vector2 rayOrigin = new Vector2(xPos, screenMax.y - yOffset); // Position at the location's y-value

            // Shoot a ray downward with length set to the height of the camera
            float rayLength = Camera.main.orthographicSize * 2f; // Height of the camera
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength, spawnLayerMask);

            Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red, 2f); // Visualize in Scene view

            if (hit.collider != null)
            {
                return hit.point;
            }
        }

        return screenMin;
    }
}
