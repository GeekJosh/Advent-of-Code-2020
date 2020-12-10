using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Advent_of_Code_2020
{
    class Program
    {
        static Dictionary<int, Action> DayActions = new Dictionary<int, Action>() {
            { 0, () => {
                    for (var x = 1; x < DayActions.Count; x++) {
                        Console.WriteLine($"Day {x}");
                        DayActions[x]();
                        Console.WriteLine();
                    }
                }
            },
            { 1, () => DayOne() },
            { 2, () => DayTwo() },
            { 3, () => DayThree() },
            { 4, () => DayFour() },
            { 5, () => DayFive() },
            { 6, () => DaySix() },
            { 7, () => DaySeven() },
            { 8, () => DayEight() },
            { 9, () => DayNine() },
            { 10, () => DayTen() }
        };

        static void Main(string[] args)
        {
            Console.WriteLine("    ADVENT OF CODE 2020    ");
            Console.WriteLine("===========================");

            while (true)
            {
                Console.WriteLine();
                Console.Write("Which day? ");
                var input = Console.ReadLine();
                if (int.TryParse(input, out var day) && DayActions.ContainsKey(day))
                {
                    DayActions[day]();
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }

        #region Day One
        static void DayOne()
        {
            Console.WriteLine("Day One");

            var input = File.ReadAllLines(@"01\input.txt").Select(l => int.Parse(l)).ToArray();
            for (var i = 0; i < input.Length; i++)
            {
                for (var j = 0; j < input.Length; j++)
                {
                    if (j == i) continue;
                    if (input[i] + input[j] == 2020)
                    {
                        Console.WriteLine($"Part 1 answer: {input[i] * input[j]}");
                        DayOne_2(input);
                        return;
                    }
                }
            }
            Console.WriteLine("Failed");
        }

        static void DayOne_2(int[] input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                for (var j = 0; j < input.Length; j++)
                {
                    if (j == i) continue;
                    for (var h = 0; h < input.Length; h++)
                    {
                        if (h == i || h == j) continue;
                        if (input[i] + input[j] + input[h] == 2020)
                        {
                            Console.WriteLine($"Part 2 answer: {input[i] * input[j] * input[h]}");
                            return;
                        }
                    }
                }
            }
            Console.WriteLine("Failed");
        }
        #endregion

        #region Day Two
        static void DayTwo()
        {
            var lines = File.ReadAllLines(@"02\input.txt");
            var validPasswords = lines.Select(l => new PasswordPolicy(l)).Where(p => p.IsValidPartOne());
            Console.WriteLine($"Part 1 answer: {validPasswords.Count()}");
            DayTwo_2(lines);
        }

        static void DayTwo_2(string[] input)
        {
            var validPasswords = input.Select(l => new PasswordPolicy(l)).Where(p => p.IsValidPartTwo());
            Console.WriteLine($"Part 2 answer: {validPasswords.Count()}");
        }
        #endregion

        #region Day Three
        static void DayThree()
        {
            var rows = File.ReadAllLines(@"03\input.txt");

            var trees = PlotRoute(rows, 3, 1);

            Console.WriteLine($"Part 1 answer: {trees}");
            DayThree_2(rows);
        }

        static void DayThree_2(string[] rows)
        {
            var acc = PlotRoute(rows, 1, 1);
            acc *= PlotRoute(rows, 3, 1);
            acc *= PlotRoute(rows, 5, 1);
            acc *= PlotRoute(rows, 7, 1);
            acc *= PlotRoute(rows, 1, 2);

            Console.WriteLine($"Part 2 answer: {acc}");
        }

        static int PlotRoute(string[] rows, int moveX, int moveY)
        {
            var x = moveX;
            var y = moveY;
            var trees = 0;

            while (y < rows.Length)
            {
                while (x >= rows[y].Length) rows[y] += rows[y];
                if (rows[y][x] == '#') trees++;
                x += moveX;
                y += moveY;
            }

            return trees;
        }

        #endregion

        #region Day Four

        static void DayFour()
        {
            var passports = new List<Dictionary<string, string>>();
            var lines = File.ReadAllLines(@"04\input.txt");

            var current = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                if (line.Trim().Length == 0)
                {
                    passports.Add(current);
                    current = new Dictionary<string, string>();
                    continue;
                }

                var parts = line.Split(' ');
                foreach (var part in parts)
                {
                    var kvp = part.Split(':');
                    current.Add(kvp[0], kvp[1]);
                }
            }

            if (current.Count > 0) passports.Add(current);

            var presentPassports = passports.Where(p =>
                p.ContainsKey("byr") &&
                p.ContainsKey("iyr") &&
                p.ContainsKey("eyr") &&
                p.ContainsKey("hgt") &&
                p.ContainsKey("hcl") &&
                p.ContainsKey("ecl") &&
                p.ContainsKey("pid"));

            Console.WriteLine($"Part 1 answer: {presentPassports.Count()}");
            DayFour_2(presentPassports.ToList());
        }

        static void DayFour_2(List<Dictionary<string, string>> presentPassports)
        {
            var validPassports = presentPassports.Count(p =>
                IsIntInRange(p["byr"], 1920, 2002) &&
                IsIntInRange(p["iyr"], 2010, 2020) &&
                IsIntInRange(p["eyr"], 2020, 2030) &&
                IsValidHeight(p["hgt"]) &&
                Regex.IsMatch(p["hcl"], @"^#[0-9a-f]{6}$") &&
                Regex.IsMatch(p["ecl"], @"^(amb|blu|brn|gry|grn|hzl|oth)$") &&
                Regex.IsMatch(p["pid"], @"^\d{9}$"));

            Console.WriteLine($"Part 2 answer: {validPassports}");
        }

        static bool IsIntInRange(string input, int min, int max)
        {
            return int.TryParse(input, out var output) && output >= min && output <= max;
        }

        static bool IsValidHeight(string height)
        {
            var rgx = new Regex(@"^(\d+)(cm|in)$", RegexOptions.IgnoreCase);
            if (rgx.IsMatch(height))
            {
                var matches = rgx.Matches(height);
                if (matches[0].Groups.Count == 3)
                {
                    switch (matches[0].Groups[2].Value)
                    {
                        case "in":
                            return IsIntInRange(matches[0].Groups[1].Value, 59, 76);
                        case "cm":
                            return IsIntInRange(matches[0].Groups[1].Value, 150, 193);
                    }
                }
            }

            return false;
        }

        #endregion

        #region Day Five

        static void DayFive()
        {
            var rows = File.ReadAllLines(@"05\input.txt");

            var boardingCards = rows.Select(r => new BoardingPass(r));
            Console.WriteLine($"Part 1 answer: {boardingCards.Max(b => b.Id)}");
            DayFive_2(boardingCards);
        }

        static void DayFive_2(IEnumerable<BoardingPass> boardingPasses)
        {
            var ids = boardingPasses.Select(b => b.Id);
            var min = ids.Min();
            var max = ids.Max();
            var x = 0;
            for (x = min; x < max; x++)
            {
                if (ids.Contains(x)) continue;
                if (ids.Contains(x + 1) && ids.Contains(x - 1)) break;
            }

            Console.WriteLine($"Part 2 answer: {x}");
        }

        #endregion

        #region Day Six

        static void DaySix()
        {
            var lines = File.ReadAllLines(@"06\input.txt");

            var groups = new List<string>();
            var group = "";
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    groups.Add(group);
                    group = "";
                    continue;
                }

                foreach (var c in line)
                {
                    if (group.Contains(c)) continue;
                    group += c;
                }
            }

            groups.Add(group);

            Console.WriteLine($"Part 1 answer: {groups.Sum(g => g.Length)}");
            DaySix_2(lines);
        }

        static void DaySix_2(string[] lines)
        {
            var groups = new List<string>();
            string group = null;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    groups.Add(group);
                    group = null;
                    continue;
                }

                if (group == null)
                {
                    group = line;
                    continue;
                }

                group = new string(line.Intersect(group).ToArray());
            }

            groups.Add(group);

            Console.WriteLine($"Part 2 answer: {groups.Sum(g => g.Length)}");
        }

        #endregion

        #region Day Seven

        static void DaySeven()
        {
            var input = File.ReadAllLines(@"07\input.txt");

            var rules = new Dictionary<string, Dictionary<string, int>>();

            foreach (var line in input)
            {
                var currentBag = Regex.Matches(line, @"^(\w+ \w+) bags contain")[0].Groups[1].Value;
                rules.Add(currentBag, new Dictionary<string, int>());

                var contains = Regex.Matches(line, @"(?:(\d+) (\w+ \w+))+");
                foreach (Match match in contains)
                    rules[currentBag].Add(match.Groups[2].Value, int.Parse(match.Groups[1].Value));
            }

            // find bags which contain shiny gold directly
            var total = rules.Count(r => CanContain(rules, r.Key, "shiny gold"));
            Console.WriteLine($"Part 1 answer: {total}");

            DaySeven_2(rules);
        }

        static void DaySeven_2(Dictionary<string, Dictionary<string, int>> rules)
        {
            var total = TotalBags(rules, "shiny gold");
            Console.WriteLine($"Part 2 answer: {total}");
        }

        static bool CanContain(Dictionary<string, Dictionary<string, int>> rules, string currentKey, string bagType)
        {
            if (rules[currentKey].ContainsKey(bagType)) return true;
            foreach (var rule in rules[currentKey].Keys)
                if (CanContain(rules, rule, bagType)) return true;

            return false;
        }

        static int TotalBags(Dictionary<string, Dictionary<string, int>> rules, string bagType)
        {
            var total = 0;
            foreach (var rule in rules[bagType])
                total += rule.Value + (rule.Value * TotalBags(rules, rule.Key));

            return total;
        }

        #endregion

        #region Day Eight

        static void DayEight()
        {
            var instructions = File.ReadAllLines(@"08\input.txt");

            ExecuteInstructions(instructions, out var acc);

            Console.WriteLine($"Part 1 answer: {acc}");

            DayEight_2(instructions);
        }

        static void DayEight_2(string[] instructions)
        {
            var acc = 0;
            for (int i = 0; i < instructions.Length; i++)
            {
                var newInstructions = new string[instructions.Length];
                instructions.CopyTo(newInstructions, 0);
                var op = newInstructions[i].Split(' ');
                if (op[0] == "jmp" || op[0] == "nop")
                {
                    newInstructions[i] = $"{(op[0] == "jmp" ? "nop" : "jmp")} {op[1]}";
                    if (ExecuteInstructions(newInstructions, out acc)) break;
                }
            }

            Console.WriteLine($"Part 2 answer: {acc}");
        }

        static bool ExecuteInstructions(string[] instructions, out int acc)
        {
            var executedInstructions = new List<int>();
            var i = 0;
            acc = 0;
            while (!executedInstructions.Contains(i))
            {
                if (i == instructions.Length) return true;

                executedInstructions.Add(i);
                var op = instructions[i].Split(' ');

                switch (op[0])
                {
                    case "nop":
                        i++;
                        continue;
                    case "acc":
                        i++;
                        acc += int.Parse(op[1]);
                        continue;
                    case "jmp":
                        i += int.Parse(op[1]);
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException("op", op[0], "Unrecognized instruction");
                }
            }

            return false;
        }

        #endregion

        #region Day Nine

        static void DayNine()
        {
            var input = File.ReadAllLines(@"09\input.txt").Select(i => long.Parse(i)).ToArray();

            for (var i = 25; i < input.Length; i++)
            {
                var valid = false;

                for (var x = i - 25; x < i; x++)
                {
                    for (var y = i - 25; y < i; y++)
                    {
                        if (x == y) continue;
                        if (input[x] + input[y] == input[i])
                        {
                            valid = true;
                            break;
                        }
                    }

                    if (valid) break;
                }

                if (!valid)
                {
                    Console.WriteLine($"Part 1 answer: {input[i]}");
                    DayNine_2(input, input[i]);
                    return;
                }
            }

            Console.WriteLine("Failed");
        }

        static void DayNine_2(long[] input, long target)
        {
            for (var x = 0; x < input.Length - 1; x++)
            {
                var found = false;
                var acc = input[x];
                var y = 0;
                for (y = x + 1; y < input.Length; y++)
                {
                    acc += input[y];
                    if (acc == target)
                    {
                        found = true;
                        break;
                    }

                    if (acc > target) break;
                }

                if (found)
                {
                    var range = input.Skip(x).Take(y - x);
                    var min = range.Min();
                    var max = range.Max();
                    Console.WriteLine($"Part 2 answer: {min + max}");
                    return;
                }
            }

            Console.WriteLine("Failed");

        }

        #endregion

        #region Day Ten

        static void DayTen()
        {
            var input = File.ReadAllLines(@"10\input.txt").Select(s => int.Parse(s)).ToList();
            input.Add(0);
            input.Add(input.Max() + 3);
            var adaptors = input.OrderBy(i=>i).ToArray();

            var joltDifs = new Dictionary<int, int>();

            for (var x = 1; x < adaptors.Length; x++)
            {
                var dif = adaptors[x] - adaptors[x - 1];
                if (!joltDifs.ContainsKey(dif)) joltDifs.Add(dif, 0);
                joltDifs[dif]++;
            }

            Console.WriteLine($"Part 1 answer: {joltDifs[1] * joltDifs[3]}");

            DayTen_2(adaptors);
        }

        static void DayTen_2(int[] adaptors) {
            var lookup = new Dictionary<int, long>();
            var total = CountTotals(0, ref adaptors, ref lookup);

            Console.WriteLine($"Part 2 answer: {total}");
        }

        static long CountTotals(int start, ref int[] input, ref Dictionary<int, long> lookup) {
            long total = 0;
            
            if (lookup.ContainsKey(start))
                return lookup[start];

            if (start == input.Length - 1)
                return 1;

            for(int x = start + 1; x < input.Length; x++) {
                if (input[x] - input[start] <= 3)
                    total += CountTotals(x, ref input, ref lookup);
                else
                    break;
            }

            lookup.Add(start, total);
            return total;
        }

        static List<int[]> GetPermutations(List<int> adaptors, int start, int target) {
            var workingSet = adaptors.Skip(adaptors.IndexOf(start)).ToArray();
            return null;
        }

        #endregion
    }

    class PasswordPolicy
    {
        public int Min { get; set; }
        public int Max { get; set; }
        public char Char { get; set; }
        public string Password { get; set; }

        public PasswordPolicy(string input)
        {
            var parts = input.Split(':');
            Password = parts[1].Trim();

            var policyParts = parts[0].Split(' ');
            Char = policyParts[1][0];

            var range = policyParts[0].Split('-');
            Min = int.Parse(range[0]);
            Max = int.Parse(range[1]);
        }

        public bool IsValidPartOne()
        {
            var count = Password.Count(c => c == Char);
            return count >= Min && count <= Max;
        }

        public bool IsValidPartTwo()
        {
            return Password[Min - 1] == Char ^ Password[Max - 1] == Char;
        }
    }

    class BoardingPass
    {
        public string Encoded { get; }
        public int Row
        {
            get
            {
                var min = 0;
                var max = 127;
                var row = 0;
                for (int x = 0; x < 7; x++)
                {
                    if (Encoded[x] == 'F')
                    {
                        max = (min + max) / 2;
                        row = min;
                    }
                    else
                    {
                        min = 1 + (min + max) / 2;
                        row = max;
                    }
                }
                return row;
            }
        }

        public int Column
        {
            get
            {
                var min = 0;
                var max = 7;
                var column = 0;
                for (int x = 7; x < 10; x++)
                {
                    if (Encoded[x] == 'L')
                    {
                        max = (min + max) / 2;
                        column = min;
                    }
                    else
                    {
                        min = 1 + (min + max) / 2;
                        column = max;
                    }
                }
                return column;
            }
        }

        public int Id => Row * 8 + Column;
        public BoardingPass(string encoded)
        {
            Encoded = encoded;
        }

        public override string ToString() => $"ID:{Id} R{Row} C{Column}";
    }
}
