namespace RegexPuzzle;

public record Dfa(bool IsAccepting, Dictionary<char, Dfa> Transitions);

public static class NfaToDfaConverter
{
    public static Dfa Convert(Nfa nfa)
    {
        var dfaStateCache = new Dictionary<List<int>, Dfa>(DfaStateIdEqualityComparer.Instance);
        return GenerateDfaState(new List<Nfa> { nfa }, dfaStateCache);
    }

    private static Dfa GenerateDfaState(List<Nfa> nfaStates, Dictionary<List<int>, Dfa> cache)
    {
        var transitiveClosure = GetEmptyTransitionsTransitiveClosure(nfaStates);
        var dfaStateId = transitiveClosure.Select(s => s.Id).ToList();
        dfaStateId.Sort();

        if (cache.TryGetValue(dfaStateId, out var state))
        {
            return state;
        }

        var isDfaStateAccepting = transitiveClosure.Any(s => s.IsAccepting);
        var dfaStateTransitions = new Dictionary<char, Dfa>();
        var dfaState = new Dfa(isDfaStateAccepting, dfaStateTransitions);
        cache.Add(dfaStateId, dfaState);

        var charTransitions = transitiveClosure.Select(s => s.CharTransition)
            .Where(t => t.HasValue)
            .GroupBy(t => t!.Value.Label, t => t!.Value.State);
        foreach (var charTransitionGrouping in charTransitions)
        {
            dfaStateTransitions.Add(
                charTransitionGrouping.Key,
                GenerateDfaState(charTransitionGrouping.ToList(), cache)
            );
        }

        return dfaState;
    }

    private static HashSet<Nfa> GetEmptyTransitionsTransitiveClosure(List<Nfa> startStates)
    {
        var visited = new HashSet<Nfa>();
        var queue = new Queue<Nfa>(startStates);
        while (queue.TryDequeue(out var state))
        {
            if (!visited.Add(state))
            {
                continue;
            }

            foreach (var nextState in state.EmptyTransitions)
            {
                queue.Enqueue(nextState);
            }
        }

        return visited;
    }

    private class DfaStateIdEqualityComparer : EqualityComparer<List<int>>
    {
        public static readonly DfaStateIdEqualityComparer Instance = new();

        private DfaStateIdEqualityComparer()
        {
        }

        public override bool Equals(List<int>? x, List<int>? y) =>
            (x, y) switch
            {
                (null, null) => true,
                (null, _) or (_, null) => false,
                _ => x.SequenceEqual(y)
            };

        public override int GetHashCode(List<int> obj) => obj.Aggregate(HashCode.Combine);
    }
}