using UnityEngine;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Linq;
using WeatherApp.Data;

namespace Databases
{
    /// Game Data Manager for handling SQLite database operations
    public class GameDataManager : MonoBehaviour
    {
        [Header("Database Configuration")]
        [SerializeField] private string databaseName = "GameData.db";
        
        private SQLiteConnection _database;
        private string _databasePath;
        
        // Singleton pattern for easy access
        public static GameDataManager Instance { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// TODO: Students will implement this method
        private void InitializeDatabase()
        {
            try
            {
                // Set up database path using Application.persistentDataPath
                _databasePath = Path.Combine(Application.persistentDataPath, databaseName);
                
                // Create SQLite connection
                _database = new SQLiteConnection(_databasePath);
                
                // Create tables for game data
                _database.CreateTable<HighScore>();
                
                Debug.Log($"Database initialized at: {_databasePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize database: {ex.Message}");
            }
        }
        

        public void AddHighScore(string playerName, int score, string levelName = "Default")
        {
            var highScore = new HighScore { 
                PlayerName = playerName, 
                Score = score,
                LevelName = levelName
            };
            _database.Insert(highScore);
        }

        public List<HighScore> GetTopHighScores(int limit = 10)
        {
            return _database.Table<HighScore>().OrderByDescending(hs => hs.Score).Take(limit).ToList();
        }

        public void UpdateHighScore(int id, int newScore)
        {
            var highScore = _database.Find<HighScore>(id);
            if (highScore != null)
            {
                highScore.Score = newScore;
                _database.Update(highScore);
            }
        }

        public void DeleteHighScore(int id)
        {
            _database.Delete<HighScore>(id);
        }
        
        /// TODO: Students will implement this method
        public List<HighScore> GetHighScoresForLevel(string levelName, int limit = 10)
        {
            try
            {
                // Query the database for scores filtered by level name, ordered by score descending
                return _database.Table<HighScore>()
                    .Where(hs => hs.LevelName == levelName)
                    .OrderByDescending(hs => hs.Score)
                    .Take(limit)
                    .ToList();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get level high scores: {ex.Message}");
                return new List<HighScore>();
            }
        }
        
        public void SaveWeatherData(WeatherData weatherData)
        {
            var record = new HighScore
            {
                PlayerName = weatherData.CityName,
                Score = (int)weatherData.TemperatureInCelsius, // Example: using temperature as score
                LevelName = "Weather"
            };
            _database.Insert(record);
        }
        
        
        #region Database Utility Methods
        
        /// TODO: Students will implement this method
        public int GetHighScoreCount()
        {
            try
            {
                // Count all records in the HighScore table
                return _database.Table<HighScore>().Count();
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to get high score count: {ex.Message}");
                return 0;
            }
        }
        
        /// TODO: Students will implement this method
        public void ClearAllHighScores()
        {
            try
            {
                // Delete all records from the HighScore table
                _database.DeleteAll<HighScore>();
                
                Debug.Log("All high scores cleared");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to clear high scores: {ex.Message}");
            }
        }
        
        /// <summary>
        /// Close the database connection when the application quits
        /// </summary>
        private void OnApplicationQuit()
        {
            _database?.Close();
        }
        
        #endregion
    }
}