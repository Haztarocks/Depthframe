using UnityEngine;

public class TorchBattery : MonoBehaviour
{
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float batteryDrainRate = 10f;

    public bool HasBattery()
    {
        return currentBattery > 0;
    }

    // Returns true if battery is still available after draining
    public bool DrainBattery(float deltaTime)
    {
        if (currentBattery > 0)
        {
            currentBattery -= batteryDrainRate * deltaTime;
            if (currentBattery <= 0)
            {
                currentBattery = 0;
                return false;
            }
            return true;
        }
        return false;
    }

    // Call this when picking up a battery
    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0, maxBattery);
    }

    // Example: call this from a pickup object
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BatteryPickup"))
        {
            AddBattery(25f); // Or whatever amount you want
            Destroy(other.gameObject);
        }
    }
}