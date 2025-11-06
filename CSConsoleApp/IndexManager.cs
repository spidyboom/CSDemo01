using System.Collections.Generic;
using System.Linq;

{
    public class IndexManager
    {
        public static Dictionary<string, List<Film>> CreateActorIndex(List<Film> films)
        {
            var index = new Dictionary<string, List<Film>>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in films)
            {
                foreach (var a in f.Actors)
                {
                    if (!index.TryGetValue(a.Name, out var lst)) { lst = new(); index[a.Name] = lst; }
                    lst.Add(f);
                }
                foreach (var s in f.Staff)
                {
                    if (!index.TryGetValue(s.Name, out var lst)) { lst = new(); index[s.Name] = lst; }
                    lst.Add(f);
                }
            }
            return index;
        }

        public static Dictionary<string, List<(Film film, Staff staff)>> CreateDeptIndex(List<Film> films)
        {
            var dict = new Dictionary<string, List<(Film, Staff)>>(StringComparer.OrdinalIgnoreCase);
            foreach (var f in films)
            {
                foreach (var s in f.Staff)
                {
                    if (!dict.TryGetValue(s.Dept, out var lst)) { lst = new(); dict[s.Dept] = lst; }
                    lst.Add((f, s));
                }
            }
            return dict;
        }
    }
}