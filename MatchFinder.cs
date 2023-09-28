using System.Collections.Immutable;

namespace RegexPuzzle;

public static class MatchFinder
{
    public static string? FindMatchingString(List<Dfa> automata)
    {
        var queue = new Queue<(ImmutableStack<char>, List<Dfa>)>();
        queue.Enqueue((ImmutableStack<char>.Empty, automata));

        while (queue.TryDequeue(out var value))
        {
            var (stringSoFar, dfas) = value;

            if (dfas.All(dfa => dfa.IsAccepting))
            {
                return string.Concat(stringSoFar.Reverse());
            }

            var transitionsToConsider = dfas[0].Transitions.Keys.ToHashSet();
            foreach (var dfa in dfas.Skip(1))
            {
                transitionsToConsider.IntersectWith(dfa.Transitions.Keys);
            }

            foreach (var c in transitionsToConsider)
            {
                queue.Enqueue((
                    stringSoFar.Push(c),
                    dfas.Select(dfa => dfa.Transitions[c]).ToList()
                ));
            }
        }

        return null;
    }
}