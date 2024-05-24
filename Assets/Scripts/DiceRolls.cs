using UnityEngine;

public class DiceRolls : MonoBehaviour
{
    public int soilQuality; //Light green dice
    public int temperature; //Green dice
    public int sun; //Yellow dice
    public int humidity; //Light Blue dice
    public int rain; //Blue dice
    public int radiation; //Red dice
    public int airPolution; //Pink dice

    public DiceRolls()
    {
        //Random Dice values between 1 and 6
        this.soilQuality = Random.Range(0, 6);
        this.temperature = Random.Range(0, 6);
        this.sun = Random.Range(0, 6);
        this.humidity = Random.Range(0, 6);
        this.rain = Random.Range(0, 6);
        this.radiation = Random.Range(0, 6);
        this.airPolution = Random.Range(0, 6);
    }

    public DiceRolls(int soilQuality, int temperature, int sun, int humidity, int rain, int radiation, int airPolution)
    {
        //Dice values between 0 and 5
        this.soilQuality = soilQuality - 1;
        this.temperature = temperature - 1;
        this.sun = sun - 1;
        this.humidity = humidity - 1;
        this.rain = rain - 1;
        this.radiation = radiation - 1;
        this.airPolution = airPolution - 1;
    }

    public float GetFactor(float value, float valueWeight)
    {
        return value * valueWeight;
    }

    public float GetFactorMin1(float value, float valueWeight)
    {
        return Mathf.Max(1, value * valueWeight);
    }

    /**
     * Value between -0.5 and 2
     */
    public float GetGrowFactor()
    {
        const int temperatureWeight = 7;
        const int sunWeight = 9;
        const int rainWeight = 9;
        const int soilQualityWeight = 11;
        const int humidityWeight = 11;
        int positiveTotalWeight = temperatureWeight + sunWeight + rainWeight + soilQualityWeight + humidityWeight;

        float growthValue = (GetFactor(temperature, temperatureWeight) + GetFactor(sun, sunWeight) + GetFactor(rain, rainWeight) + GetFactor(soilQuality, soilQualityWeight) + GetFactor(humidity, humidityWeight)) / positiveTotalWeight;
        growthValue = growthValue / 5; // Value between 0 and 1
        float growFactor = growthValue * 2.5f - 0.5f; // Value between -0.5 and 2
        return growFactor;
    }

    /**
     * Value between -0.5 and 1.5
     */
    public float GetLengthFactor()
    {
        const int temperatureWeight = 3;
        const int sunWeight = 7;
        const int rainWeight = 7;
        const int soilQualityWeight = 13;
        const int humidityWeight = 13;
        int positiveTotalWeight = temperatureWeight + sunWeight + rainWeight + soilQualityWeight + humidityWeight;

        const int airPolutionWeight = 3;
        const int radiationWeight = 5;
        int negativeTotalWeight = airPolutionWeight + radiationWeight;

        float positiveFactors = (GetFactor(temperature, temperatureWeight) + GetFactor(sun, sunWeight) + GetFactor(rain, rainWeight) + GetFactor(soilQuality, soilQualityWeight) + GetFactor(humidity, humidityWeight)) / positiveTotalWeight;
        positiveFactors = positiveFactors * 2 / 5*5; // Value between 0 and 2

        float negativeFactors = (GetFactor(airPolution, airPolutionWeight) + GetFactor(radiation, radiationWeight)) / negativeTotalWeight;
        negativeFactors = negativeFactors / 10; // value between 0 and 1

        float lengthFactor = positiveFactors - (negativeFactors * 0.5f); // Value between -0.5 and 1.5
        return lengthFactor;
    }

    /**
     * Number between -17.5 degrees and 17.5 degrees
     */
    public float GetRotationY()
    {
        float rotation = -this.soilQuality + this.temperature - this.sun + this.humidity + this.rain - this.radiation + this.airPolution - 2.5f;
        return rotation;
    }

    /**
     * Number between -17.5 degrees and 17.5 degrees
     */
    public float GetRotationZ()
    {
        float rotation = this.soilQuality - this.temperature + this.sun - this.humidity + this.rain + this.radiation - this.airPolution - 2.5f;
        return rotation;
    }

    /**
     * Value between -0.5 and 1
     */
    public float GetLushnessFactor()
    {
        const int soilQualityWeight = 9;
        const int humidityWeight = 3;
        const int airPolutionWeight = 13;
        int totalWeight = airPolutionWeight + soilQualityWeight + humidityWeight;

        int lushness = (int)(GetFactor(soilQuality, soilQualityWeight) + GetFactor(humidity, humidityWeight) - GetFactor(airPolution, airPolutionWeight)) / totalWeight; //Value between -5 and 10
        float lushnessFactor = lushness / 10; //Value between -0.5 and 1
        return lushnessFactor;
    }
}
