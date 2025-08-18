using UnityEngine;
using UnityEngine.Networking;
using System;

namespace WeatherApp.Services.ErrorHandling
{
    /// <summary>
    /// Handles weather API error messages and provides user-friendly error descriptions
    /// Following Single Responsibility Principle - this class only handles error messaging
    /// </summary>
    public static class WeatherErrorHandler
    {
        /// <summary>
        /// Get a user-friendly error message based on the API response code
        /// </summary>
        public static string GetUserFriendlyMessage(UnityWebRequest.Result result, long responseCode, string error)
        {
            switch (result)
            {
                case UnityWebRequest.Result.Success:
                    return "Success";

                case UnityWebRequest.Result.ConnectionError:
                    return "Unable to connect to the weather service. Please check your internet connection and try again.";

                case UnityWebRequest.Result.ProtocolError:
                    return GetHttpErrorMessage(responseCode, error);

                case UnityWebRequest.Result.DataProcessingError:
                    return "There was a problem processing the weather data. Please try again later.";

                default:
                    return "An unexpected error occurred. Please try again later.";
            }
        }

        /// <summary>
        /// Get a user-friendly message for HTTP error codes
        /// </summary>
        private static string GetHttpErrorMessage(long responseCode, string error)
        {
            switch (responseCode)
            {
                case 401:
                    return "Invalid API key. Please check your configuration.";
                
                case 404:
                    return "City not found. Please check the spelling and try again.";
                
                case 429:
                    return "Too many requests. Please wait a moment before trying again.";
                
                case 500:
                    return "Weather service is experiencing problems. Please try again later.";
                
                default:
                    return $"Error accessing weather service ({responseCode}). Please try again later.";
            }
        }

        /// <summary>
        /// Get a user-friendly message for JSON parsing errors
        /// </summary>
        public static string GetJsonParseErrorMessage(Exception ex)
        {
            return "Unable to read weather data. The service might be experiencing issues.";
        }

        /// <summary>
        /// Get a user-friendly message for validation errors
        /// </summary>
        public static string GetValidationErrorMessage(string city)
        {
            return $"Received invalid weather data for {city}. Please try again.";
        }

        /// <summary>
        /// Get a user-friendly message for configuration errors
        /// </summary>
        public static string GetConfigurationErrorMessage()
        {
            return "Weather service is not configured. Please set up your API key in the configuration file.";
        }
    }
}
