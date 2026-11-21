namespace CheckYourNumbers.Game;

/// <summary>What is practised: the numbers 0–99, or a list of words.</summary>
public enum DeckKind { Numbers, Words }

/// <summary>How a round is played.</summary>
public enum Mode
{
    /// <summary>Hear it, then type the number or pick the meaning.</summary>
    Listen,

    /// <summary>See it, say it out loud, reveal it to hear it, and grade yourself.</summary>
    Read,
}

/// <summary>A word with its English meaning and its audio file name (without extension).</summary>
public sealed record WordEntry(string Word, string Meaning, string File);

/// <summary>
/// One thing to practise. For numbers, Prompt and Answer are the digits ("47"); for words, both are
/// the English meaning. Written is the target language ("syvogfyrre", "gennaio").
/// </summary>
public sealed record Card(string Id, string Prompt, string Answer, string Written, string Audio);

public sealed class Language
{
    public required string Code { get; init; }

    /// <summary>The English name, used in prompts ("Say it in Danish").</summary>
    public required string Name { get; init; }

    /// <summary>The language's own name, shown as its button's tooltip.</summary>
    public required string Native { get; init; }

    /// <summary>The country whose flag is on the button (wwwroot/flags/{flag}.svg); the code when not set.</summary>
    public string? Flag { get; init; }

    public string FlagFile => $"flags/{Flag ?? Code}.svg";

    public required Func<int, string> NumberWord { get; init; }

    /// <summary>
    /// The file extension of the recorded numbers in audio/{code}/numbers/0..99. Null while the
    /// recordings are missing; the language is then left out of the app.
    /// </summary>
    public string? NumberAudio { get; init; }

    public IReadOnlyList<WordEntry> Words { get; init; } = [];

    public string WordAudio { get; init; } = "mp3";

    public bool Available => NumberAudio is not null;

    public IReadOnlyList<DeckKind> Decks => Words.Count > 0 ? [DeckKind.Numbers, DeckKind.Words] : [DeckKind.Numbers];

    public IReadOnlyList<Card> CardsFor(DeckKind deck) => deck switch
    {
        DeckKind.Words => Words
            .Select(w => new Card(w.File, w.Meaning, w.Meaning, w.Word, $"audio/{Code}/words/{w.File}.{WordAudio}"))
            .ToList(),
        _ => Enumerable.Range(0, 100)
            .Select(n => new Card(n.ToString(), n.ToString(), n.ToString(), NumberWord(n),
                $"audio/{Code}/numbers/{n}.{NumberAudio}"))
            .ToList(),
    };

    public static string DeckName(DeckKind deck) => deck == DeckKind.Words ? "Words" : "0–99";

    /// <summary>Every language the app knows. Only those with recordings are shown.</summary>
    private static readonly Language[] Known =
    [
        new() { Code = "da", Name = "Danish", Native = "Dansk", Flag = "dk", NumberWord = NumberWords.Danish, NumberAudio = "mp3" },
        new() { Code = "en", Name = "English", Native = "English", Flag = "gb", NumberWord = NumberWords.English, NumberAudio = "mp3" },
        new() { Code = "de", Name = "German", Native = "Deutsch", NumberWord = NumberWords.German, NumberAudio = "mp3" },
        // Swiss Standard German (a Swiss voice, standard words), in the folder audio/ch.
        new()
        {
            Code = "ch", Name = "Swiss German", Native = "Deutsch (CH)", NumberWord = NumberWords.SwissGerman,
            NumberAudio = "mp3",
        },
        new() { Code = "fr", Name = "French", Native = "Français", NumberWord = NumberWords.French, NumberAudio = "mp3" },
        new() { Code = "it", Name = "Italian", Native = "Italiano", NumberWord = NumberWords.Italian, NumberAudio = "wav" },
        new() { Code = "es", Name = "Spanish", Native = "Español", NumberWord = NumberWords.Spanish, NumberAudio = "mp3" },
        new() { Code = "nl", Name = "Dutch", Native = "Nederlands", NumberWord = NumberWords.Dutch, NumberAudio = "mp3" },
    ];

    /// <summary>The languages shown, in button order.</summary>
    public static readonly IReadOnlyList<Language> All = Known.Where(l => l.Available).ToList();

    public static Language Get(string? code) => All.FirstOrDefault(l => l.Code == code) ?? All[0];
}
