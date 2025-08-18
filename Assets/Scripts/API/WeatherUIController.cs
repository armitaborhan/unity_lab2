using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeatherApp.Services;
using WeatherApp.Data;
using Databases;

namespace WeatherApp.UI
{
    /// <summary>
    /// UI Controller for the Weather Application
    /// Students will connect this to the API client and handle user interactions
    /// </summary>
    public class WeatherUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField cityInputField = default;
        [SerializeField] private Button getWeatherButton = default;
        [SerializeField] private TextMeshProUGUI weatherDisplayText = default;
        [SerializeField] private TextMeshProUGUI statusText = default;
        
        [Header("API Client")]
        [SerializeField] private WeatherApiClient apiClient = default;
        
        private void Start()
        {
            // Set up button click listener
            getWeatherButton.onClick.AddListener(OnGetWeatherClicked);

            // Initialize UI state
            SetStatusText("Enter a city name and click Get Weather");
        }
        
        private async void OnGetWeatherClicked()
        {
            string cityName = cityInputField.text;
            if (string.IsNullOrWhiteSpace(cityName))
            {
                SetStatusText("Please enter a city name");
                return;
            }

            getWeatherButton.interactable = false;
            SetStatusText("Loading weather data...");
            weatherDisplayText.text = "";

            try
            {
                var weatherData = await apiClient.GetWeatherDataAsync(cityName);
                if (weatherData != null && weatherData.IsValid)
                {
                    DisplayWeatherData(weatherData);
                    GameDataManager.Instance.SaveWeatherData(weatherData);
                }
                else
                {
                    SetStatusText("Failed to get weather data");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error getting weather data: {ex.Message}");
                SetStatusText("An error occurred. Please try again.");
            }
            finally
            {
                getWeatherButton.interactable = true;
            }
        }
        
        private void DisplayWeatherData(WeatherData weatherData)
        {
            string displayText = $"City: {weatherData.CityName}\n" +
                         $"Temperature: {weatherData.TemperatureInCelsius:F1}°C\n" +
                         $"Description: {weatherData.PrimaryDescription}\n" +
                         $"Humidity: {weatherData.Main.Humidity}%\n" +
                         $"Pressure: {weatherData.Main.Pressure} hPa";
            weatherDisplayText.text = displayText;
        }
        
        private void SetStatusText(string message)
        {
            if (statusText != null)
            {
                statusText.text = message;
            }
        }
        
        public void ClearDisplay()
        {
            weatherDisplayText.text = "";
            cityInputField.text = "";
            SetStatusText("Enter a city name and click Get Weather");
        }
    }
}