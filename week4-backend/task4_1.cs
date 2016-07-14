using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;

namespace week4._1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            List<Type> types = new List<Type> {
                typeof(Person),
                typeof(FootballClub),
                typeof(Officer),
                typeof(ColoredShape),
                typeof(MultipleFruit),
                typeof(ArithmExpression),
                typeof(Hero)
            };

            TestParsing();
            TestPropertyTypes(types);
            TestTypesCounts(types);
        }

        static void TestParsing()
        {
            string text = @"Mr. John Smith was born on 1999/12/12 bla hihi haha
	                hoho was born Arsenal from London, founded in 1897, Mr. John Smith was born on 1999/12/12";
            List<IParseable> entities = Parser.ParseText(text,
                new TextParser[] {
                    Person.ParseFromText,
                    FootballClub.ParseFromText,
                    Officer.ParseFromText,
                    ColoredShape.ParseFromText,
                    MultipleFruit.ParseFromText,
                    ArithmExpression.ParseFromText,
                    Hero.ParseFromText
                });
            foreach (var entity in entities)
                Console.WriteLine(entity.ToString());
        }

        static void TestPropertyTypes(List<Type> types)
        {
            Console.WriteLine("******** PROPERTY TYPES *********");
            var info = GetPropertyTypes(types);
            foreach (var pair in info)
            {
                Console.WriteLine(pair.Item1 + " " + pair.Item2);
            }
            Console.WriteLine();
        }

        static void TestTypesCounts(List<Type> types)
        {
            Console.WriteLine("******** PROPERTY TYPE COUNTS *********");
            var info = GetPropertyTypesCounts(types);
            foreach (var pair in info)
            {
                Console.WriteLine(pair.Key + " " + pair.Value);
            }
            Console.WriteLine();
        }

        static HashSet<Tuple<string, string>> GetPropertyTypes(List<Type> types)
        {
            HashSet<Tuple<string, string>> res = new HashSet<Tuple<string, string>>();
            foreach (var type in types)
            {
                foreach (var property in type.GetProperties())
                {
                    string strType = property.PropertyType.ToString();
                    string propName = property.Name;
                    res.Add(new Tuple<string, string>(propName, strType));
                }
            }
            return res;
        }

        static Dictionary<string, uint> GetPropertyTypesCounts(List<Type> types)
        {
            Dictionary<string, uint> res = new Dictionary<string, uint>();
            foreach (var type in types)
            {
                foreach (var property in type.GetProperties())
                {
                    string strType = property.PropertyType.ToString();
                    if (res.ContainsKey(strType))
                        res[strType]++;
                    else
                        res[strType] = 1;
                }
            }
            return res;
        }

        interface IParseable { }

        delegate IParseable[] TextParser(string text);

        [AttributeUsage(AttributeTargets.Class)]
        class ReguexAttribute : Attribute
        {
            public string Pattern { get; set; }

            public ReguexAttribute(string pattern) { Pattern = pattern; }
        }

        [Reguex(@"(Mr\.|Mrs\.|Ms\.)\s(\w+)\s(\w+)\swas\sborn\son\s(\d{4}/\d\d/\d\d)")]
        class Person : IParseable
        {
            private string FirstName { get; set; }
            private string LastName { get; set; }

            public string Name { get; private set; }
            public DateTime BirthDate { get; private set; }
            public int Age { get; private set; }
            public char Gender { get; private set; }

            public Person(string firstName, string lastName, string genderTitle, DateTime birthDate)
            {
                FirstName = firstName;
                LastName = lastName;

                Gender = (genderTitle == "Mr." ? 'm' : 'f');
                Name = String.Concat(firstName, lastName);

                BirthDate = birthDate;
                Age = DateTime.Now.Year - birthDate.Year;
                DateTime date2 = new DateTime(birthDate.Year, DateTime.Now.Month, DateTime.Now.Day);
                if (birthDate > date2)
                {
                    Age--;
                }
            }

            public override string ToString()
            {
                return $"{LastName}, {FirstName} ({Gender}, {Age})";
            }

            public static Person[] ParseFromText(string text)
            {
                ReguexAttribute attr = typeof(Person).GetTypeInfo().GetCustomAttribute<ReguexAttribute>();

                string regexPerson = attr.Pattern;
                var personsMatches = Regex.Matches(text, regexPerson);
                Person[] people = new Person[personsMatches.Count];

                for (int i = 0; i < personsMatches.Count; i++)
                {
                    var match = personsMatches[i];

                    try
                    {
                        string genderTitle = match.Groups[1].Value;

                        string firstName = match.Groups[2].Value;
                        string lastName = match.Groups[3].Value;

                        DateTime birthDate = DateTime.Parse(match.Groups[4].Value);

                        people[i] = new Person(firstName, lastName, genderTitle, birthDate);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.Write("Check indexes of matches");
                    }
                    catch (ArgumentNullException)
                    {
                        Console.Write("No text to parse for date");
                    }
                    catch (FormatException)
                    {
                        Console.Write("Date could not be parsed");
                    }
                }

                return people;
            }
        }

        [Reguex(@"(\w+)\sfrom\s(\w+),\sfounded\sin\s(\d{4})")]
        class FootballClub : IParseable
        {
            public string Name { get; private set; }
            public string OriginCity { get; private set; }
            public uint FoundationYear { get; private set; }

            public FootballClub(string name, string originCity, uint foundationYear)
            {
                Name = name;
                OriginCity = originCity;
                FoundationYear = foundationYear;
            }

            public override string ToString()
            {
                return $"{Name}, {OriginCity}, est. {FoundationYear}";
            }

            
            public static FootballClub[] ParseFromText(string text)
            {
                ReguexAttribute attr = typeof(FootballClub).GetTypeInfo().GetCustomAttribute<ReguexAttribute>();

                string regexPerson = attr.Pattern;
                var clubsMatches = Regex.Matches(text, regexPerson);
                FootballClub[] clubs = new FootballClub[clubsMatches.Count];

                for (int i = 0; i < clubsMatches.Count; i++)
                {
                    var match = clubsMatches[i];

                    try
                    {
                        string name = match.Groups[1].Value;

                        string originCity = match.Groups[2].Value;
                        uint foundationYear = UInt32.Parse(match.Groups[3].Value);

                        clubs[i] = new FootballClub(name, originCity, foundationYear);
                    }
                    catch (IndexOutOfRangeException)
                    {
                        Console.Write("Check indexes of matches");
                    }
                    catch (ArgumentNullException)
                    {
                        Console.Write("No text to parse for year");
                    }
                    catch (FormatException)
                    {
                        Console.Write("Year could not be parsed");
                    }
                }

                return clubs;
            }
        }

        [Reguex(@"Cop (\w+)\s(\w+)")]
        class Officer : IParseable
        {
            public string Rank { get; private set; }
            public string Surname { get; private set; }

            public Officer(string rank, string surname)
            {
                Rank = rank;
                Surname = surname;
            }

            public override string ToString()
            {
                return $"Cop {Rank} {Surname}";
            }


            public static Officer[] ParseFromText(string text)
            {
                ReguexAttribute attr = typeof(Officer).GetTypeInfo().GetCustomAttribute<ReguexAttribute>();

                string regexPerson = attr.Pattern;
                var officerMatches = Regex.Matches(text, regexPerson);
                Officer[] officers = new Officer[officerMatches.Count];

                for (int i = 0; i < officerMatches.Count; i++)
                {
                    var match = officerMatches[i];
                    string rank = match.Groups[1].Value;

                    string surname = match.Groups[2].Value;

                    officers[i] = new Officer(rank, surname);

                }

                return officers;
            }
        }

        [Reguex(@"2D (\w+)\s(\w+)")]
        class ColoredShape : IParseable
        {
            public string Color { get; private set; }
            public string Shape { get; private set; }

            public ColoredShape(string color, string shape)
            {
                Color = color;
                Shape = shape;
            }

            public override string ToString()
            {
                return $"2D {Color} {Shape}";
            }


            public static ColoredShape[] ParseFromText(string text)
            {
                ReguexAttribute attr = typeof(ColoredShape).GetTypeInfo().GetCustomAttribute<ReguexAttribute>();

                string regexPerson = attr.Pattern;
                var shapeMatches = Regex.Matches(text, regexPerson);
                ColoredShape[] shapes = new ColoredShape[shapeMatches.Count];

                for (int i = 0; i < shapeMatches.Count; i++)
                {
                    var match = shapeMatches[i];
                    string color = match.Groups[1].Value;

                    string shape = match.Groups[2].Value;

                    shapes[i] = new ColoredShape(color, shape);

                }

                return shapes;
            }
        }

        [Reguex(@"I bought (\d+)\s(\w+)")]
        class MultipleFruit : IParseable
        {
            public int Quantity { get; private set; }
            public string Fruit { get; private set; }

            public MultipleFruit(int count, string fruit)
            {
                Quantity = count;
                Fruit = fruit;
            }

            public override string ToString()
            {
                return $"I bought {Quantity} {Fruit}";
            }


            public static MultipleFruit[] ParseFromText(string text)
            {
                ReguexAttribute attr = typeof(MultipleFruit).GetTypeInfo().GetCustomAttribute<ReguexAttribute>();

                string regexPerson = attr.Pattern;
                var fruitMatches = Regex.Matches(text, regexPerson);
                MultipleFruit[] fruit = new MultipleFruit[fruitMatches.Count];

                for (int i = 0; i < fruitMatches.Count; i++)
                {
                    var match = fruitMatches[i];
                    int count = Int32.Parse(match.Groups[1].Value);

                    string fruitType = match.Groups[2].Value;

                    fruit[i] = new MultipleFruit(count, fruitType);

                }

                return fruit;
            }
        }

        [Reguex(@"(\d+)\w(\d+)")]
        class ArithmExpression : IParseable
        {
            public int LeftOperand { get; private set; }
            public int RightOperand { get; private set; }
            public char Sign { get; private set; }

            public ArithmExpression(int left, int right, char sign)
            {
                LeftOperand = left;
                RightOperand = right;
                Sign = sign;
            }

            public override string ToString()
            {
                return $"{LeftOperand}{Sign}{RightOperand}";
            }


            public static ArithmExpression[] ParseFromText(string text)
            {
                ReguexAttribute attr = typeof(ArithmExpression).GetTypeInfo().GetCustomAttribute<ReguexAttribute>();

                string regexPerson = attr.Pattern;
                var arexMatches = Regex.Matches(text, regexPerson);
                ArithmExpression[] arex = new ArithmExpression[arexMatches.Count];

                for (int i = 0; i < arexMatches.Count; i++)
                {
                    var match = arexMatches[i];
                    int left = Int32.Parse(match.Groups[1].Value);

                    char sign = match.Groups[2].Value[0];
                    int right = Int32.Parse(match.Groups[3].Value);

                    arex[i] = new ArithmExpression(left, right, sign);

                }

                return arex;
            }
        }

        [Reguex(@"(\w+)\sStr:(\d+)\sAgi:(\d+)\sInt:(\d+)")]
        class Hero : IParseable
        {
            public int Strength { get; private set; }
            public int Agility { get; private set; }
            public int Intelligence { get; private set; }
            public string Name { get; private set; }

            public Hero(string name, int str, int agi, int inte)
            {
                Name = name;
                Strength = str;
                Agility = agi;
                Intelligence = inte;
            }

            public override string ToString()
            {
                return $"{Name} Str:{Strength} Agi:{Agility} Int:{Intelligence}";
            }


            public static Hero[] ParseFromText(string text)
            {
                ReguexAttribute attr = typeof(Hero).GetTypeInfo().GetCustomAttribute<ReguexAttribute>();

                string regexPerson = attr.Pattern;
                var heroMatches = Regex.Matches(text, regexPerson);
                Hero[] heroes = new Hero[heroMatches.Count];

                for (int i = 0; i < heroMatches.Count; i++)
                {
                    var match = heroMatches[i];
                    string name = match.Groups[1].Value;
                    int str = Int32.Parse(match.Groups[2].Value);
                    int agi = Int32.Parse(match.Groups[3].Value);
                    int inte = Int32.Parse(match.Groups[4].Value);

                    heroes[i] = new Hero(name, str, agi, inte);

                }

                return heroes;
            }
        }

        class Parser
        {
            public static List<IParseable> ParseText(string text, TextParser[] parsers)
            {
                List<IParseable> result = new List<IParseable>();
                foreach (var parser in parsers)
                {
                    IParseable[] instances = parser(text);
                    foreach (var instance in instances)
                    {
                        result.Add(instance);
                    }
                }
                return result;
            }
        }
    }
}
