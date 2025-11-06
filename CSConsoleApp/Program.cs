using System;
using System.Collections.Generic;

{
    class Program
    {
        static string DataFile;
        static List<Film> Films;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: dotnet run -- <path_to_tmdb_5000_credits.csv>");
                return;
            }
            DataFile = args[0];
            Console.WriteLine("Loading: " + DataFile);
            Films = DataReader.ReadData(DataFile);
            Console.WriteLine($"Loaded films: {Films.Count}");

            var queryExecutor = new QueryExecutor(Films);

            while (true)
            {
                Console.WriteLine("\nSelect query (enter number) or 0 to exit:");
                Console.WriteLine("1. Films by director 'Steven Spielberg'");
                Console.WriteLine("2. Characters played by 'Tom Hanks'");
                Console.WriteLine("3. Top 5 films with largest cast");
                Console.WriteLine("4. Top 10 most frequent actors");
                Console.WriteLine("5. All unique departments");
                Console.WriteLine("6. Films with 'Hans Zimmer' as composer");
                Console.WriteLine("7. Dictionary: movie_id -> director name");
                Console.WriteLine("8. Films with Brad Pitt and George Clooney");
                Console.WriteLine("9. Total people in 'Camera' department");
                Console.WriteLine("10. People in both cast and crew for 'Titanic'");
                Console.WriteLine("11. Inner circle of Quentin Tarantino");
                Console.WriteLine("12. Top 10 actor pairs working together");
                Console.WriteLine("13. Top 5 crew members with most departments");
                Console.WriteLine("14. Films where one person was Director, Writer, Producer");
                Console.WriteLine("15. Two steps to Kevin Bacon");
                Console.WriteLine("16. Group by director with average cast/crew sizes");
                Console.WriteLine("17. Career paths for actor-crew members");
                Console.WriteLine("18. People working with both Martin Scorsese and Christopher Nolan");
                Console.WriteLine("19. Departments ranked by average cast size");
                Console.WriteLine("20. Johnny Depp character archetypes");

                Console.Write("Number: ");
                var input = Console.ReadLine();
                if (!int.TryParse(input, out int cmd)) break;
                if (cmd == 0) break;

                switch (cmd)
                {
                    case 1: queryExecutor.FindDirectorFilms("Steven Spielberg"); break;
                    case 2: queryExecutor.FindActorRoles("Tom Hanks"); break;
                    case 3: queryExecutor.FindBiggestCasts(); break;
                    case 4: queryExecutor.FindPopularActors(); break;
                    case 5: queryExecutor.ShowAllDepartments(); break;
                    case 6: queryExecutor.FindComposerFilms("Hans Zimmer"); break;
                    case 7: queryExecutor.ShowDirectorMap(); break;
                    case 8: queryExecutor.FindActorPairFilms(new[] { "Brad Pitt", "George Clooney" }); break;
                    case 9: queryExecutor.CountDepartmentPeople("Camera"); break;
                    case 10: queryExecutor.FindDoubleRoles("Titanic"); break;
                    case 11: queryExecutor.FindDirectorTeam("Quentin Tarantino", 5); break;
                    case 12: queryExecutor.FindFrequentPairs(10); break;
                    case 13: queryExecutor.FindVersatileCrew(5); break;
                    case 14: queryExecutor.FindMultiRoleFilms(); break;
                    case 15: queryExecutor.FindBaconConnections("Kevin Bacon"); break;
                    case 16: queryExecutor.AnalyzeDirectorPatterns(); break;
                    case 17: queryExecutor.ShowActorCrossoverCareers(); break;
                    case 18: queryExecutor.FindDirectorOverlap("Martin Scorsese", "Christopher Nolan"); break;
                    case 19: queryExecutor.RankDepartmentsByCast(); break;
                    case 20: queryExecutor.AnalyzeActorArchetypes("Johnny Depp"); break;
                    default: Console.WriteLine("Invalid choice"); break;
                }
            }
        }
    }
}