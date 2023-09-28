namespace RegexPuzzle;

public class Nfa
{
    public (char Label, Nfa State)? CharTransition { get; }
    public List<Nfa> EmptyTransitions { get; }
    public int Id { get; }
    public bool IsAccepting { get; set; }

    public Nfa(int id, bool isAccepting, (char, Nfa)? charTransition = null,
        List<Nfa>? emptyTransitions = null)
    {
        Id = id;
        IsAccepting = isAccepting;
        CharTransition = charTransition;
        EmptyTransitions = emptyTransitions ?? new List<Nfa>();
    }

    public Nfa(int id, bool isAccepting, List<Nfa> emptyTransitions) : this(id, isAccepting, null,
        emptyTransitions)
    {
    }
}

public class RegexToNfaConverter
{
    public static Nfa Convert(IRegex regex) => new RegexToNfaConverter().ConvertInternal(regex).nfa;

    private int nextStateId;

    private RegexToNfaConverter()
    {
    }

    private (Nfa nfa, List<Nfa> acceptStates) ConvertInternal(IRegex regex)
    {
        switch (regex)
        {
            case Empty:
            {
                var state = new Nfa(nextStateId++, true);
                return (state, new List<Nfa> { state });
            }
            case Literal(var c):
            {
                var acceptState = new Nfa(nextStateId++, true);
                var startState = new Nfa(nextStateId++, false, (c, acceptState));
                return (startState, new List<Nfa> { acceptState });
            }
            case Union(var r1, var r2):
            {
                var (nfa1, nfa1AcceptingStates) = ConvertInternal(r1);
                var (nfa2, nfa2AcceptingStates) = ConvertInternal(r2);
                var startState = new Nfa(nextStateId++, false, new List<Nfa> { nfa1, nfa2 });
                var acceptStates = nfa1AcceptingStates.Concat(nfa2AcceptingStates).ToList();
                return (startState, acceptStates);
            }
            case Concat(var r1, var r2):
            {
                var (nfa1, nfa1AcceptingStates) = ConvertInternal(r1);
                var (nfa2, nfa2AcceptingStates) = ConvertInternal(r2);
                foreach (var nfa1AcceptingState in nfa1AcceptingStates)
                {
                    nfa1AcceptingState.IsAccepting = false;
                    nfa1AcceptingState.EmptyTransitions.Add(nfa2);
                }

                return (nfa1, nfa2AcceptingStates);
            }
            case Star(var r):
            {
                var (nfa, nfaAcceptingStates) = ConvertInternal(r);
                var state = new Nfa(nextStateId++, true, new List<Nfa> { nfa });
                foreach (var nfaAcceptingState in nfaAcceptingStates)
                {
                    nfaAcceptingState.IsAccepting = false;
                    nfaAcceptingState.EmptyTransitions.Add(state);
                }

                return (state, new List<Nfa> { state });
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(regex));
        }
    }
}