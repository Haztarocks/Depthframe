using UnityEngine;

public class TorchBattery : MonoBehaviour
{
    public float maxBattery = 100f;
    public float currentBattery = 100f;
    public float batteryDrainRate = 10f;

    public delegate void OnBatteryChanged(float battery);
    public static event OnBatteryChanged BatteryChanged;

    public bool HasBattery()
    {
        return currentBattery > 0;
    }

    public bool DrainBattery(float deltaTime)
    {
        if (currentBattery > 0)
        {
            currentBattery -= batteryDrainRate * deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0, maxBattery);
            BatteryChanged?.Invoke(currentBattery);
            return currentBattery > 0;
        }
        return false;
    }

    public void AddBattery(float amount)
    {
        currentBattery = Mathf.Clamp(currentBattery + amount, 0, maxBattery);
        BatteryChanged?.Invoke(currentBattery);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BatteryPickup"))
        {
            AddBattery(25f);
            Destroy(other.gameObject);
        }
    }
}