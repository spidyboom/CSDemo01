using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

{
    public class DataReader
    {
        public static List<Film> ReadData(string path)
        {
            var result = new List<Film>();
            using var reader = new StreamReader(path);
            string headerLine = reader.ReadLine();
            var columns = ParseCsv(headerLine);
            int idIndex = Array.IndexOf(columns, "movie_id");
            int titleIndex = Array.IndexOf(columns, "title");
            int castIndex = Array.IndexOf(columns, "cast");
            int crewIndex = Array.IndexOf(columns, "crew");
            if (idIndex < 0 || titleIndex < 0 || castIndex < 0 || crewIndex < 0)
                throw new Exception("Missing required columns");

            string row;
            while ((row = reader.ReadLine()) != null)
            {
                var cells = ParseCsv(row);
                try
                {
                    var film = new Film();
                    film.Id = int.Parse(cells[idIndex]);
                    film.Name = cells[titleIndex];
                    film.Actors = ReadActors(cells[castIndex]);
                    film.Staff = ReadStaff(cells[crewIndex]);
                    result.Add(film);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error parsing row: " + ex.Message);
                }
            }
            return result;
        }

        public static string[] ParseCsv(string line)
        {
            var parts = new List<string>();
            bool inQuotes = false;
            var current = new System.Text.StringBuilder();
            for (int i = 0; i < line.Length; i++)
            {
                char ch = line[i];
                if (ch == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"'); i++;
                    }
                    else inQuotes = !inQuotes;
                }
                else if (ch == ',' && !inQuotes)
                {
                    parts.Add(current.ToString()); current.Clear();
                }
                else current.Append(ch);
            }
            parts.Add(current.ToString());
            return parts.ToArray();
        }

        private static List<Actor> ReadActors(string json)
        {
            try
            {
                var array = JArray.Parse(json);
                var list = new List<Actor>();
                foreach (var item in array)
                {
                    list.Add(new Actor
                    {
                        Id = item.Value<int?>("id") ?? 0,
                        Name = item.Value<string>("name") ?? "",
                        Role = item.Value<string>("character") ?? ""
                    });
                }
                return list;
            }
            catch
            {
                return new List<Actor>();
            }
        }

        private static List<Staff> ReadStaff(string json)
        {
            try
            {
                var array = JArray.Parse(json);
                var list = new List<Staff>();
                foreach (var item in array)
                {
                    list.Add(new Staff
                    {
                        Id = item.Value<int?>("id") ?? 0,
                        Name = item.Value<string>("name") ?? "",
                        Dept = item.Value<string>("department") ?? "",
                        Role = item.Value<string>("job") ?? ""
                    });
                }
                return list;
            }
            catch
            {
                return new List<Staff>();
            }
        }
    }
}