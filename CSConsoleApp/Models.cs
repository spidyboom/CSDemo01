using System.Collections.Generic;

{
    public class Film
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Actor> Actors { get; set; } = new();
        public List<Staff> Staff { get; set; } = new();
    }

    public class Actor 
    { 
        public int Id; 
        public string Name; 
        public string Role; 
    }

    public class Staff 
    { 
        public int Id; 
        public string Name; 
        public string Dept; 
        public string Role; 
    }
}