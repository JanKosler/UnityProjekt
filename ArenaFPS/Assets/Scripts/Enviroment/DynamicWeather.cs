using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicWeather : MonoBehaviour
{
    [SerializeField] private WeatherStates _weatherState;

    public enum WeatherStates
    {
        PickWeather,
        SunnyWeather,
        SnowyWeather,
        MistWeather,
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private IEnumerator Weather()
    {
        while (true)
        {
            switch (_weatherState)
            {
                case WeatherStates.PickWeather:
                    PickWeather();
                    break;
                case WeatherStates.SunnyWeather:
                    SunnyWeather();
                    break;
                case WeatherStates.SnowyWeather:
                    SnowyWeather();
                    break;
                case WeatherStates.MistWeather:
                    MistWeather();
                    break;
            }
            yield return null;
        }
    }
    private void PickWeather()
    {

    }
    private void SunnyWeather()
    {

    }
    private void SnowyWeather()
    {

    }
    private void MistWeather()
    {

    }
}
