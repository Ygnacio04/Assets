using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifuseLogicTest : MonoBehaviour
{
    [Header("Size")] 
    public AnimationCurve xtraSmall;
    public AnimationCurve small;
    public AnimationCurve moderate;
    public AnimationCurve large;

    public float xtraSmallValue;
    public float smallValue;
    public float moderateValue;
    public float largeValue;

    public float maxNumOfUnits = 40;
    public float inputValueDistance;
    public float inputValueSize;
    
    [Header("Distance")] 
    public AnimationCurve nearDistance;
    public AnimationCurve mediumDistance;
    public AnimationCurve farDistance;

    public float nearDistanceValue;
    public float mediumDistanceValue;
    public float farDistanceValue;

    public float maxDistance = 100;

    [Header("Response Values")] 
    public float smallResponseValue = 10;
    public float mediumResponseValue = 30;
    public float highResponseValue = 50;

    [Header("Evaluation Values")] 
    public float lowValue;
    public float mediumValue;
    public float highValue;

    public float numberOfResponseUnits;
    
    private void Start()
    {
        
    }
    
    private float EvaluateLowTable()
    {
        return Mathf.Max(
            Mathf.Min(mediumDistanceValue, xtraSmallValue),
            Mathf.Min(farDistanceValue, xtraSmallValue),
            Mathf.Min(mediumDistanceValue, smallValue),
            Mathf.Min(farDistanceValue, smallValue),
            Mathf.Min(farDistanceValue, moderateValue));
    }

    private float EvaluateMediumTable()
    {
        return Mathf.Max(
            Mathf.Min(nearDistanceValue, xtraSmallValue),
            Mathf.Min(mediumDistanceValue, moderateValue),
            Mathf.Min(farDistanceValue, largeValue));
    }
    
    private float EvaluateHighTable()
    {
        return Mathf.Max(
            Mathf.Min(nearDistanceValue, smallValue),
            Mathf.Min(nearDistanceValue, moderateValue),
            Mathf.Min(nearDistanceValue, largeValue),
            Mathf.Min(mediumDistanceValue, largeValue));
    }

    public void OnUnitsValueChanged(string value)
    {
        inputValueSize = float.Parse(value);
    }
    
    public void OnDistanceValueChanged(string value)
    {
        inputValueDistance = float.Parse(value);
    }

    public void OnEvaluateClick()
    {
        SetSizeValues();
        
        SetDistanceValues();
        
        EvaluateThreat();

        numberOfResponseUnits = CalculateResponse();
    }

    private void SetDistanceValues()
    {
        nearDistanceValue = Mathf.Clamp(nearDistance.Evaluate(inputValueDistance), 0, 1);
        mediumDistanceValue = Mathf.Clamp(mediumDistance.Evaluate(inputValueDistance), 0, 1);
        farDistanceValue = Mathf.Clamp(farDistance.Evaluate(inputValueDistance), 0, 1);
    }

    private void SetSizeValues()
    {
        xtraSmallValue = Mathf.Clamp(xtraSmall.Evaluate(inputValueSize), 0, 1);
        smallValue = Mathf.Clamp(small.Evaluate(inputValueSize), 0, 1);
        moderateValue = Mathf.Clamp(moderate.Evaluate(inputValueSize), 0, 1);
        largeValue = Mathf.Clamp(large.Evaluate(inputValueSize), 0, 1);
    }

    private void EvaluateThreat()
    {
        lowValue = EvaluateLowTable();
        mediumValue = EvaluateMediumTable();
        highValue = EvaluateHighTable();
    }

    private float CalculateResponse()
    {
        return (smallValue * smallResponseValue +
                mediumValue * mediumResponseValue +
                highValue * highResponseValue) /
               (smallValue + mediumValue + highValue);
    }
}
