using RegexPuzzle;

Console.WriteLine(MatchFinder.FindMatchingString(
    File.ReadAllLines("regexes.txt")
        .Select(RegexParser.Parse)
        .Select(RegexToNfaConverter.Convert)
        .Select(NfaToDfaConverter.Convert)
        .ToList()
));