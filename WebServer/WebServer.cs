﻿// Name: Harrison Doppelt and Victor Valdez Landa
// Date: 11/20/2024

// http://localhost:8080/

using GUI.Client.Controllers;
using GUI.Client.Models;
using MySql.Data.MySqlClient;
using MySqlX.XDevAPI;
using System.Linq;
using System.Text;

namespace WebServer
{
    internal class WebServer
    {
        private const string httpOkHeader = "HTTP/1.1 200 OK\r\n" + "Connection: close\r\n" + "Content-Type: text/html; charset=UTF-8\r\n";

        static void Main(string[] args)
        {
            Server.StartServer(HandleHttpConnection, 8080);

            // Prevent main from returning
            Console.Read();
        }

        private static void HandleHttpConnection(NetworkConnection client)
        {
            string request = client.ReadLine();
            string response = string.Empty;

            // If Specifc Game
            if (request.Contains("GET /games?gid="))
            {
                // Parse Game ID from the request URL
                int gameId = ParseGameId(request);
                response = GetSpecificGamePage(gameId);
            }

            // If Homepage
            else if (request.Contains("GET / "))
            {
                response = "<html><h3>Welcome to the Snake Game Database!</h3><a href=\"/games\">View Games</a></html>";
            }

            // If All Games
            else if (request.Contains("GET /games"))
            {
                response = GetGamePage();
            }

            // Calculate Content-Length dynamically
            int contentLength = Encoding.UTF8.GetByteCount(response);
            string header = httpOkHeader + $"Content-Length: {contentLength}\r\n\r\n";

            client.Send(header + response);
            client.Disconnect();
        }

        private static string GetGamePage()
        {
            var games = new List<(int GameId, string StartTime, string EndTime)>();

            using (MySqlConnection databaseConnection = new MySqlConnection(NetworkController.connectionString))
            {
                databaseConnection.Open();
                MySqlCommand command = databaseConnection.CreateCommand();
                command.CommandText = "SELECT id, start_time, end_time FROM Games;";

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    // Get the results out
                    while (reader.Read())
                    {
                        int gameId = reader.GetInt32(0);
                        string startTime = reader.GetDateTime(1).ToString("yyyy-MM-dd HH:mm:ss");
                        string endTime = reader.IsDBNull(2) ? "" : reader.GetDateTime(2).ToString("yyyy-MM-dd HH:mm:ss");

                        games.Add((gameId, startTime, endTime));
                    }
                }
            }

            string gamesHtml = string.Empty;
            gamesHtml += "<html><h3>All Games</h3><table border='1'>";
            gamesHtml += "<thead><tr><td>ID</td><td>Start</td><td>End</td></tr></thead><tbody>";

            foreach (var game in games)
            {
                gamesHtml += "<tr>";
                gamesHtml += $"<td><a href='/games?gid={game.GameId}'>{game.GameId}</a></td>";
                gamesHtml += $"<td>{game.StartTime}</td>";
                gamesHtml += $"<td>{game.EndTime}</td>";
                gamesHtml += "</tr>";
            }

            gamesHtml += "</tbody></table></html>";

            return gamesHtml;
        }

        private static int ParseGameId(string request)
        {
            int startIndex = request.IndexOf("?gid=") + 5;
            string gameIdString = request.Substring(startIndex).Split(' ')[0];

            if (int.TryParse(gameIdString, out int gameId))
            {
                return gameId;
            }

            return -1;
        }

        private static string GetSpecificGamePage(int gameId)
        {
            string playersHtml = string.Empty;

            using (MySqlConnection databaseConnection = new MySqlConnection(NetworkController.connectionString))
            {
                databaseConnection.Open();
                MySqlCommand command = databaseConnection.CreateCommand();
                command.CommandText = "SELECT id, name, max_score, enter_time, leave_time FROM Players WHERE game_id = @gameId;";
                command.Parameters.AddWithValue("@gameId", gameId);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    playersHtml = "<table border='1'><thead><tr>" +
                                  "<td>Player ID</td><td>Player Name</td><td>Max Score</td><td>Enter Time</td><td>Leave Time</td>" +
                                  "</tr></thead><tbody>";

                    while (reader.Read())
                    {
                        int playerId = reader.GetInt32(0);
                        string playerName = reader.GetString(1);
                        int maxScore = reader.GetInt32(2);
                        string enterTime = reader.GetDateTime(3).ToString("yyyy-MM-dd HH:mm:ss");
                        string leaveTime = reader.IsDBNull(4) ? "" : reader.GetDateTime(4).ToString("yyyy-MM-dd HH:mm:ss");

                        playersHtml += $"<tr><td>{playerId}</td><td>{playerName}</td><td>{maxScore}</td>" + $"<td>{enterTime}</td><td>{leaveTime}</td></tr>";
                    }
                }

                return $"<html><h3>Stats for Game {gameId}</h3>{playersHtml}</html>";
            }
        }
    }
}