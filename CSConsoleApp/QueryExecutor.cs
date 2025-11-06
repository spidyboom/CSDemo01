using System;
using System.Collections.Generic;
using System.Linq;

{
    public class QueryExecutor
    {
        private List<Film> Films { get; }

        public QueryExecutor(List<Film> films)
        {
            Films = films;
        }

        public void FindDirectorFilms(string director)
        {
            var found = Films.Where(f => f.Staff.Any(s => 
                s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase) && 
                s.Name.Equals(director, StringComparison.OrdinalIgnoreCase))).ToList();
            Console.WriteLine($"Films by '{director}': {found.Count}");
            foreach (var f in found) Console.WriteLine($"{f.Id}\t{f.Name}");
        }

        public void FindActorRoles(string actor)
        {
            var roles = Films.SelectMany(f => f.Actors
                .Where(a => a.Name.Equals(actor, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Role)).Distinct().ToList();
            Console.WriteLine($"Roles for '{actor}': {roles.Count}");
            foreach (var r in roles) Console.WriteLine(r);
        }

        public void FindBiggestCasts()
        {
            var top = Films.OrderByDescending(f => f.Actors.Count).Take(5).ToList();
            Console.WriteLine("Top 5 films by cast size:");
            foreach (var f in top) Console.WriteLine($"{f.Name} (id={f.Id}) — cast={f.Actors.Count}");
        }

        public void FindPopularActors()
        {
            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in Films)
                foreach (var a in f.Actors) 
                    counts[a.Name] = counts.GetValueOrDefault(a.Name) + 1;
            
            foreach (var kv in counts.OrderByDescending(kv => kv.Value).Take(10)) 
                Console.WriteLine($"{kv.Key} — {kv.Value}");
        }

        public void ShowAllDepartments()
        {
            var depts = Films.SelectMany(f => f.Staff.Select(s => s.Dept))
                .Where(d => !string.IsNullOrEmpty(d))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(d => d).ToList();
            Console.WriteLine("Unique departments:");
            foreach (var d in depts) Console.WriteLine(d);
        }

        public void FindComposerFilms(string composer)
        {
            var found = Films.Where(f => f.Staff.Any(s => 
                s.Role.Equals("Original Music Composer", StringComparison.OrdinalIgnoreCase) && 
                s.Name.Equals(composer, StringComparison.OrdinalIgnoreCase))).ToList();
            Console.WriteLine($"Films with '{composer}' as composer: {found.Count}");
            foreach (var f in found) Console.WriteLine(f.Name);
        }

        public void ShowDirectorMap()
        {
            var map = new Dictionary<int, string>();
            foreach (var f in Films)
            {
                var director = f.Staff.FirstOrDefault(s => 
                    s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase));
                map[f.Id] = director?.Name ?? "";
            }
            Console.WriteLine("Movie ID -> Director:");
            foreach (var kv in map) Console.WriteLine($"{kv.Key} => {kv.Value}");
        }

        public void FindActorPairFilms(string[] actors)
        {
            var found = Films.Where(f => actors.All(actor => 
                f.Actors.Any(a => a.Name.Equals(actor, StringComparison.OrdinalIgnoreCase)))).ToList();
            Console.WriteLine($"Films with {string.Join(" and ", actors)}: {found.Count}");
            foreach (var f in found) Console.WriteLine(f.Name);
        }

        public void CountDepartmentPeople(string dept)
        {
            var people = Films.SelectMany(f => f.Staff
                .Where(s => s.Dept.Equals(dept, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Name))
                .Distinct(StringComparer.OrdinalIgnoreCase).Count();
            Console.WriteLine($"Unique people in '{dept}' department: {people}");
        }

        public void FindDoubleRoles(string filmName)
        {
            var film = Films.FirstOrDefault(f => f.Name.Equals(filmName, StringComparison.OrdinalIgnoreCase));
            if (film == null) { Console.WriteLine("Film not found"); return; }
            var actorNames = film.Actors.Select(a => a.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var staffNames = film.Staff.Select(s => s.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var both = actorNames.Intersect(staffNames, StringComparer.OrdinalIgnoreCase).ToList();
            Console.WriteLine($"People in both cast and crew for '{filmName}': {both.Count}");
            foreach (var name in both) Console.WriteLine(name);
        }

        public void FindDirectorTeam(string director, int topN)
        {
            var films = Films.Where(f => f.Staff.Any(s => 
                s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase) && 
                s.Name.Equals(director, StringComparison.OrdinalIgnoreCase))).ToList();
            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in films)
                foreach (var s in f.Staff.Where(s => !s.Role.Equals("Actor", StringComparison.OrdinalIgnoreCase)))
                    if (!s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase)) 
                        counts[s.Name] = counts.GetValueOrDefault(s.Name) + 1;
            
            foreach (var kv in counts.OrderByDescending(kv => kv.Value).Take(topN)) 
                Console.WriteLine($"{kv.Key} — collaborations: {kv.Value}");
        }

        public void FindFrequentPairs(int topN)
        {
            var pairs = new Dictionary<(string, string), int>();
            foreach (var f in Films)
            {
                var names = f.Actors.Select(a => a.Name).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(n => n).ToArray();
                for (int i = 0; i < names.Length; i++)
                    for (int j = i + 1; j < names.Length; j++)
                    {
                        var pair = (names[i], names[j]);
                        pairs[pair] = pairs.GetValueOrDefault(pair) + 1;
                    }
            }
            foreach (var kv in pairs.OrderByDescending(kv => kv.Value).Take(topN)) 
                Console.WriteLine($"{kv.Key.Item1} / {kv.Key.Item2} — films together: {kv.Value}");
        }

        public void FindVersatileCrew(int topN)
        {
            var personDepts = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in Films)
                foreach (var s in f.Staff)
                {
                    if (!personDepts.TryGetValue(s.Name, out var set)) { set = new(); personDepts[s.Name] = set; }
                    if (!string.IsNullOrEmpty(s.Dept)) set.Add(s.Dept);
                }
            foreach (var kv in personDepts.OrderByDescending(kv => kv.Value.Count).Take(topN)) 
                Console.WriteLine($"{kv.Key} — departments: {kv.Value.Count}");
        }

        public void FindMultiRoleFilms()
        {
            var results = new List<(Film, string)>();
            foreach (var f in Films)
            {
                var groups = f.Staff.GroupBy(s => s.Name, StringComparer.OrdinalIgnoreCase);
                foreach (var g in groups)
                {
                    var roles = g.Select(x => x.Role).Where(r => !string.IsNullOrEmpty(r))
                        .Select(r => r.Trim()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                    if (roles.Contains("Director") && 
                        (roles.Contains("Writer") || roles.Contains("Screenplay") || roles.Contains("Author")) && 
                        roles.Contains("Producer"))
                    {
                        results.Add((f, g.Key));
                    }
                }
            }
            Console.WriteLine($"Found multi-role films: {results.Count}");
            foreach (var r in results) Console.WriteLine($"{r.Item1.Name} — {r.Item2}");
        }

        public void FindBaconConnections(string bacon)
        {
            var baconFilms = Films.Where(f => f.Actors.Any(a => 
                a.Name.Equals(bacon, StringComparison.OrdinalIgnoreCase))).ToList();
            var firstDegree = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in baconFilms) 
                foreach (var a in f.Actors) 
                    if (!a.Name.Equals(bacon, StringComparison.OrdinalIgnoreCase)) 
                        firstDegree.Add(a.Name);
            
            Console.WriteLine($"Actors 1 degree from {bacon}: {firstDegree.Count}");
            var secondDegree = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in Films)
            {
                var names = f.Actors.Select(a => a.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (names.Overlaps(firstDegree)) 
                    foreach (var n in names) 
                        if (!firstDegree.Contains(n) && !n.Equals(bacon, StringComparison.OrdinalIgnoreCase)) 
                            secondDegree.Add(n);
            }
            Console.WriteLine($"Actors 2 degrees from {bacon}: {secondDegree.Count}");
            Console.WriteLine("First 50 from each degree:\n");
            foreach (var n in firstDegree.Take(50)) Console.WriteLine("1 degree: " + n);
            foreach (var n in secondDegree.Take(50)) Console.WriteLine("2 degree: " + n);
        }

        public void AnalyzeDirectorPatterns()
        {
            var byDirector = new Dictionary<string, List<Film>>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in Films)
            {
                var directors = f.Staff.Where(s => s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase))
                    .Select(s => s.Name).DefaultIfEmpty("(unknown)");
                foreach (var d in directors)
                {
                    if (!byDirector.TryGetValue(d, out var lst)) { lst = new(); byDirector[d] = lst; }
                    byDirector[d].Add(f);
                }
            }
            Console.WriteLine("Director \t avgCast \t avgCrew \t filmCount");
            foreach (var kv in byDirector.OrderByDescending(kv => kv.Value.Count).Take(200))
            {
                var avgCast = kv.Value.Average(f => f.Actors.Count);
                var avgCrew = kv.Value.Average(f => f.Staff.Count);
                Console.WriteLine($"{kv.Key}\t{avgCast:F2}\t{avgCrew:F2}\t{kv.Value.Count}");
            }
        }

        public void ShowActorCrossoverCareers()
        {
            var actors = new HashSet<string>(Films.SelectMany(f => f.Actors.Select(a => a.Name)), StringComparer.OrdinalIgnoreCase);
            var crew = Films.SelectMany(f => f.Staff).GroupBy(s => s.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);
            Console.WriteLine("Person -> most frequent department (actor-crew crossover)");
            foreach (var name in actors)
            {
                if (crew.TryGetValue(name, out var entries))
                {
                    var topDept = entries.GroupBy(e => e.Dept, StringComparer.OrdinalIgnoreCase)
                        .OrderByDescending(g => g.Count()).FirstOrDefault()?.Key ?? "(unknown)";
                    Console.WriteLine($"{name} => {topDept}");
                }
            }
        }

        public void FindDirectorOverlap(string dir1, string dir2)
        {
            var people1 = Films.Where(f => f.Staff.Any(s => 
                s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase) && 
                s.Name.Equals(dir1, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(f => f.Staff.Select(s => s.Name))
                .Concat(Films.Where(f => f.Staff.Any(s => 
                    s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase) && 
                    s.Name.Equals(dir1, StringComparison.OrdinalIgnoreCase)))
                    .SelectMany(f => f.Actors.Select(a => a.Name)))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            var people2 = Films.Where(f => f.Staff.Any(s => 
                s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase) && 
                s.Name.Equals(dir2, StringComparison.OrdinalIgnoreCase)))
                .SelectMany(f => f.Staff.Select(s => s.Name))
                .Concat(Films.Where(f => f.Staff.Any(s => 
                    s.Role.Equals("Director", StringComparison.OrdinalIgnoreCase) && 
                    s.Name.Equals(dir2, StringComparison.OrdinalIgnoreCase)))
                    .SelectMany(f => f.Actors.Select(a => a.Name)))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
            
            var common = people1.Intersect(people2, StringComparer.OrdinalIgnoreCase).ToList();
            Console.WriteLine($"People working with both {dir1} and {dir2}: {common.Count}");
            foreach (var p in common) Console.WriteLine(p);
        }

        public void RankDepartmentsByCast()
        {
            var deptFilms = new Dictionary<string, HashSet<int>>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in Films)
                foreach (var s in f.Staff)
                {
                    if (string.IsNullOrEmpty(s.Dept)) continue;
                    if (!deptFilms.TryGetValue(s.Dept, out var set)) { set = new HashSet<int>(); deptFilms[s.Dept] = set; }
                    set.Add(f.Id);
                }
            var rankings = new List<(string dept, double avgCast)>();
            foreach (var kv in deptFilms)
            {
                var deptFilmsList = Films.Where(f => kv.Value.Contains(f.Id)).ToList();
                var avg = deptFilmsList.Average(f => f.Actors.Count);
                rankings.Add((kv.Key, avg));
            }
            foreach (var r in rankings.OrderByDescending(x => x.avgCast)) 
                Console.WriteLine($"{r.dept} — average cast size = {r.avgCast:F2}");
        }

        public void AnalyzeActorArchetypes(string actor)
        {
            var roles = Films.SelectMany(f => f.Actors
                .Where(a => a.Name.Equals(actor, StringComparison.OrdinalIgnoreCase))
                .Select(a => a.Role))
                .Where(r => !string.IsNullOrEmpty(r)).ToList();
            var groups = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            foreach (var r in roles)
            {
                var firstWord = r.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
                groups[firstWord] = groups.GetValueOrDefault(firstWord) + 1;
            }
            Console.WriteLine($"Character archetypes for {actor}:");
            foreach (var kv in groups.OrderByDescending(kv => kv.Value)) 
                Console.WriteLine($"{kv.Key} — {kv.Value}");
        }
    }
}