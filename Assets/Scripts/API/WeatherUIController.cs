using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WeatherApp.Services;
using WeatherApp.Data;
using System.Threading.Tasks;

namespace WeatherApp.UI
{
    /// <summary>
    /// UI Controller for the Weather Application
    /// </summary>
    public class WeatherUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TMP_InputField cityInputField;
        [SerializeField] private Button getWeatherButton;
        [SerializeField] private TextMeshProUGUI weatherDisplayText;
        [SerializeField] private TextMeshProUGUI statusText;

        [Header("API Client")]
        [SerializeField] private WeatherApiClient apiClient;

        private void Start()
        {
            // Set up button click listener
            getWeatherButton.onClick.AddListener(OnGetWeatherClicked);
            SetStatusText("Enter a city name and click Get Weather");
        }

        private async void OnGetWeatherClicked()
        {
            string cityName = cityInputField.text;

            if (string.IsNullOrWhiteSpace(cityName))
            {
                SetStatusText("⚠ Please enter a city name");
                return;
            }

            getWeatherButton.interactable = false;
            SetStatusText("🌤 Fetching weather data...");
            weatherDisplayText.text = "";

            try
            {
                WeatherData weatherData = await apiClient.GetWeatherDataAsync(cityName);
               

                if (weatherData == null || !weatherData.IsValid)
                {
                    SetStatusText("❌ Weather data could not be retrieved");
                    return;
                }

                DisplayWeatherData(weatherData);
                SetStatusText("✅ Weather data loaded!");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"❌ Error getting weather data: {ex.Message}");
                SetStatusText("❌ An error occurred. Please try again.");
            }
            finally
            {
                getWeatherButton.interactable = true;
            }
        }

        private void DisplayWeatherData(WeatherData weatherData)
        {
            string displayText = 
                $"📍 City: {weatherData.CityName}\n" +
                $"🌡 Temperature: {weatherData.TemperatureInCelsius:F1}°C (Feels like: {weatherData.Main.FeelsLike - 273.15f:F1}°C)\n" +
                $"🌥 Description: {weatherData.PrimaryDescription}\n" +
                $"💧 Humidity: {weatherData.Main.Humidity}%\n" +
                $"🎈 Pressure: {weatherData.Main.Pressure} hPa";

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