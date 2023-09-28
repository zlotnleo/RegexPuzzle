namespace RegexPuzzle;

public interface IRegex
{
}

public record Empty : IRegex;

public record Literal(char C) : IRegex;

public record Star(IRegex R) : IRegex;

public record Union(IRegex R1, IRegex R2) : IRegex;

public record Concat(IRegex R1, IRegex R2) : IRegex;

public class RegexParser
{
    private readonly string input;
    private int index;

    public static IRegex Parse(string s)
    {
        return new RegexParser(s).Parse();
    }

    private RegexParser(string s)
    {
        input = s;
        index = 0;
    }

    private IRegex Parse()
    {
        var regexSoFar = ParseNonConcatenated(out var exception);
        if (regexSoFar == null)
        {
            throw exception!;
        }

        while (ParseNonConcatenated(out _) is { } currentPart)
        {
            regexSoFar = new Concat(regexSoFar, currentPart);
        }

        return regexSoFar;
    }

    private IRegex? ParseNonConcatenated(out Exception? exceptionIfRequired)
    {
        if (index >= input.Length)
        {
            exceptionIfRequired = GetUnexpectedEndException();
            return null;
        }

        if (TryConsume('('))
        {
            exceptionIfRequired = null;
            return ParseBracketed();
        }

        var c = input[index];
        if (c is >= 'a' and <= 'z')
        {
            index++;
            exceptionIfRequired = null;
            return new Literal(c);
        }

        exceptionIfRequired = GetUnexpectedCharacterException("a-z or (");
        return null;
    }

    private IRegex ParseBracketed()
    {
        var r = Parse();
        ThrowIfReachedEnd();

        var isUnion = false;
        if (TryConsume('|'))
        {
            isUnion = true;
            var r1 = Parse();
            ThrowIfReachedEnd();
            r = new Union(r, r1);
        }

        Consume(')');

        if (isUnion)
        {
            return r;
        }

        ThrowIfReachedEnd();

        if (TryConsume('*'))
        {
            return new Star(r);
        }

        if (TryConsume('+'))
        {
            return new Concat(r, new Star(r));
        }

        if (TryConsume('?'))
        {
            return new Union(new Empty(), r);
        }

        throw GetUnexpectedCharacterException("*, + or ?");
    }

    private void Consume(char c)
    {
        if (!TryConsume(c))
        {
            throw GetUnexpectedCharacterException($"{c}");
        }
    }

    private bool TryConsume(char c)
    {
        if (input[index] == c)
        {
            index++;
            return true;
        }

        return false;
    }

    private void ThrowIfReachedEnd()
    {
        if (index >= input.Length)
        {
             throw GetUnexpectedEndException();
        }
    }

    private ArgumentException GetUnexpectedCharacterException(string expected) => new(
        $"Expected {expected} at position {index}, found {input[index]}"
    );

    private static ArgumentException GetUnexpectedEndException() => new("Unexpected end of input");
}