using UnityEngine;

public class CarModel 
{
    public string CarName { get; private set; } = System.String.Empty;
    public string Label { get; set; }
    public float Price { get; private set; }
    public int NumberOfCheckPointHit { get; private set; }
    public float DistanceToNextChekpoint { get; set; }

    public void CheckpointWasHit()
    {
        NumberOfCheckPointHit++;
    }
    
}
