using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ExpectedCharge {
    None,
    Positive,
    Negative,
    Both,
}

public class ParticleCounter : MonoBehaviour
{
    public static ParticleCounter instance;
    public ParticleDestroyer particleDestroyer;
    public float maxCharge;
    public float chargeDecrease;
    public ExpectedCharge expectedCharge;
    public float currentCharge;

    public System.Action successDelegate;
    public UnityEngine.Events.UnityEvent successEvent;

    public float chargeRatio { get { return currentCharge / maxCharge; }}

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        particleDestroyer.particleReceivedDelegate += OnReceiveParticle;
    }

    public void OnReceiveParticle(float charge)
    {
        float score = 0;
        switch(expectedCharge)
        {
            case ExpectedCharge.Positive:
                score = charge;
                break;
            case ExpectedCharge.Negative:
                score = -charge;
                break;
            case ExpectedCharge.Both:
                score = Mathf.Abs(charge);
                break;
        }
        currentCharge += score;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
        if(currentCharge == maxCharge)
        {
            successDelegate?.Invoke();
            successEvent.Invoke();
            VictoryHandler.instance.OnVictory();
        }
    }

    public void Update()
    {
        currentCharge -= chargeDecrease * Time.deltaTime;
        currentCharge = Mathf.Clamp(currentCharge, 0, maxCharge);
    }
}
