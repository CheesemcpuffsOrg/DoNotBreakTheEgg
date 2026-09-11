using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ThrowComponent : MonoBehaviour, IThrowComponent
{
    enum PowerCycle
    {
        PoweringUp,
        PoweringDown
    }



    [SerializeField] Transform holdAnchor;

    [SerializeField] Transform launchPoint;

    [SerializeField] float powerBase = 1f;
    [SerializeField] float powerMax = 10f;
    [SerializeField] float chargeTime = 2f;

    [Header("Power Slider")]
    //[SerializeField] Canvas canvas;
    [SerializeField] private Slider powerSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient throwStrengthGradient;

    [Header("Tags")]
    [SerializeField] TagScriptableObject isHeldTag;
    [SerializeField] TagScriptableObject isHoldingTag;
    [SerializeField] TagFilter catchableEntityFilter;
    [SerializeField] TagFilter catchingFilter;
    [SerializeField] TagFilter throwFilter;

    [Header("Audio")]
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData throwSoundData;
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData chargeThrowSoundData;
    [SerializeField, ColoredField(ColoredFieldAttribute.PresetColors.Sound)] SoundData maxChargeSoundData;


    [Header("Draw Trajectory Gizmo")]
    //[SerializeField] private float entityWeight = 1f;
    //[SerializeField] private int trajectorySteps = 10; // Number of points to simulate for the trajectory
   // [SerializeField] private float timeStep = 0.1f;

    IEntity entity;
    IEntitySoundComponent soundComponent;

    float powerCurrent;
    float powerRate;

    bool chargingShot;
    PowerCycle powerCycle = PowerCycle.PoweringUp;

    private void Awake()
    {
        entity = GetComponent<IEntity>();
        soundComponent = entity.GetEntityComponent<IEntitySoundComponent>();
    }

    private void Start()
    {
       // canvas.worldCamera = Camera.main;
       // powerSlider.gameObject.SetActive(false);
        powerSlider.maxValue = powerMax;
        powerSlider.minValue = powerBase;

        powerRate = (powerMax - powerBase) / chargeTime;

        powerCurrent = powerBase;
    }

    private void Update()
    {
        ChargeShot();
    }

    public void ChargeThrow()
    {
        if (!entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(throwFilter))
            return;

       
        chargingShot = true; // Start charging
        soundComponent.PlaySound(chargeThrowSoundData);
    }

    public void Throw()
    {
        if (!entity.GetEntityComponent<ITagComponent>().PassTagFilterCheck(throwFilter))
            return;

        if (!HoldEntityManager.Instance.TryGetHeldEntity(entity, out var heldEntity)) 
            return;

       
        HoldEntityManager.Instance.RemoveHeldEntity(entity);

        heldEntity.GetEntityComponent<IMovementComponent>().Throw(powerCurrent, (Vector2)launchPoint.up.normalized);

        soundComponent.StopSound(chargeThrowSoundData);
        soundComponent.PlaySound(throwSoundData);

       // powerSlider.gameObject.SetActive(false);

        chargingShot = false;
        powerCycle = PowerCycle.PoweringUp;
        fillImage.color = Color.white;
        powerCurrent = powerBase; // Reset power to the base value
        powerSlider.value = powerCurrent;
    }

    private void ChargeShot()
    {
        if (!chargingShot)
        {
            return;
        }


        powerCurrent += powerRate * Time.deltaTime;

        if (powerCurrent >= powerMax)
        {
            powerCurrent = powerBase;
        }

        /*if (powerCycle == PowerCycle.PoweringUp)
        {
            powerCurrent += powerRate * Time.deltaTime;

            if (powerCurrent >= powerMax)
            {
                powerCurrent = powerMax;
                powerCycle = PowerCycle.PoweringDown;
            }
        }
        else
        {
            powerCurrent -= powerRate * Time.deltaTime;

            if (powerCurrent <= powerBase)
            {
                powerCurrent = powerBase;
                powerCycle = PowerCycle.PoweringUp;
            }
        }*/

        //slider
        powerSlider.value = powerCurrent;
        float normalizedPower = Mathf.InverseLerp(powerBase, powerMax, powerCurrent);
        fillImage.color = throwStrengthGradient.Evaluate(normalizedPower);
    }
}
