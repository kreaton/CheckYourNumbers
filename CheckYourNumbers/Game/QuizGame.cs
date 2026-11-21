namespace CheckYourNumbers.Game;

public enum Stage
{
    /// <summary>Listen: waiting for the answer. Read: waiting for the reveal.</summary>
    Asking,

    /// <summary>Read only: the answer is shown and heard; waiting for the self-grade.</summary>
    Revealed,

    /// <summary>Listen, numbers: a wrong number was typed (and counted as missed); the answer stays
    /// hidden while the player listens again and retries, or gives up.</summary>
    Retrying,

    /// <summary>Listen only: answered (or timed out); waiting for the next round.</summary>
    Done,
}

public sealed class QuizGame
{
    public const int MaxDigits = 2;
    private const int ChoiceCount = 4;

    private readonly Random _random = new();
    private readonly System.Diagnostics.Stopwatch _watch = new();
    private readonly Queue<Card> _bag = new();
    private IReadOnlyList<Card> _cards = [];

    public Language Language { get; private set; } = Language.All[0];
    public DeckKind Deck { get; private set; } = DeckKind.Numbers;
    public Mode Mode { get; private set; } = Mode.Listen;

    public Card? Current { get; private set; }
    public Stage Stage { get; private set; }

    /// <summary>Listen, numbers: the digits typed so far.</summary>
    public string Typed { get; private set; } = "";

    /// <summary>Listen, words: the meanings to pick from, one of them right.</summary>
    public IReadOnlyList<string> Choices { get; private set; } = [];

    /// <summary>Listen: the answer given (digits or a meaning); null after a timeout.</summary>
    public string? Given { get; private set; }

    /// <summary>Listen: whether the answer was right, once Done.</summary>
    public bool Right { get; private set; }

    /// <summary>Listen, numbers: whether a wrong number was typed this round, so it is a retry.</summary>
    public bool Missed { get; private set; }

    /// <summary>Seconds allowed per round from the next round; null means no timer.</summary>
    public int? TimerSeconds { get; set; }

    /// <summary>Seconds allowed for the current round, fixed when it starts.</summary>
    public int? TimeLimit { get; private set; }

    /// <summary>Whether the current round's time ran out.</summary>
    public bool TimedOut { get; private set; }

    public TimeSpan Elapsed => _watch.Elapsed;

    public double? Remaining => TimeLimit is { } limit ? Math.Max(0, limit - Elapsed.TotalSeconds) : null;

    public int LastPoints { get; private set; }
    public int Points { get; private set; }
    public int Correct { get; private set; }
    public int Wrong { get; private set; }
    public int Streak { get; private set; }
    public int BestStreak { get; private set; }

    /// <summary>The card after the current one, so its audio can be fetched ahead.</summary>
    public Card? Upcoming => _bag.TryPeek(out var next) ? next : null;

    public QuizGame() => BuildCards();

    public void Select(Language language, DeckKind deck, Mode mode)
    {
        Language = language;
        Deck = language.Decks.Contains(deck) ? deck : DeckKind.Numbers;
        Mode = mode;
        Current = null;
        BuildCards();
    }

    private void BuildCards()
    {
        _cards = Language.CardsFor(Deck);
        _bag.Clear();
    }

    public void SetScore(int correct, int wrong, int bestStreak, int points)
    {
        Correct = correct;
        Wrong = wrong;
        BestStreak = bestStreak;
        Points = points;
        Streak = 0;
    }

    public void ResetScore() => SetScore(0, 0, 0, 0);

    /// <summary>
    /// A right answer scores 10 points, plus a speed bonus of up to 10 more that shrinks to nothing at
    /// the time limit (without a timer, at 6 seconds). A self-graded "Got it" scores 10.
    /// </summary>
    public static int ScoreFor(Mode mode, TimeSpan elapsed, int? timeLimit)
    {
        if (mode == Mode.Read) return 10;
        var speed = Math.Clamp(1 - elapsed.TotalSeconds / (timeLimit ?? 6.0), 0, 1);
        return 10 + (int)Math.Round(10 * speed);
    }

    /// <summary>Every card comes once, in random order, before any repeats.</summary>
    public void NewRound()
    {
        if (_cards.Count == 0) return;
        if (_bag.Count == 0)
        {
            var shuffled = _cards.ToArray();
            _random.Shuffle(shuffled);
            if (shuffled.Length > 1 && shuffled[0] == Current)
                (shuffled[0], shuffled[^1]) = (shuffled[^1], shuffled[0]);
            foreach (var card in shuffled) _bag.Enqueue(card);
        }

        Current = _bag.Dequeue();
        Stage = Stage.Asking;
        Typed = "";
        Given = null;
        Right = false;
        Missed = false;
        TimedOut = false;
        LastPoints = 0;
        TimeLimit = TimerSeconds;
        Choices = Mode == Mode.Listen && Deck == DeckKind.Words ? MakeChoices(Current) : [];
        _watch.Restart();
    }

    private string[] MakeChoices(Card card)
    {
        var others = _cards.Where(c => c.Answer != card.Answer).Select(c => c.Answer).Distinct().ToArray();
        _random.Shuffle(others);
        var choices = others.Take(ChoiceCount - 1).Append(card.Answer).ToArray();
        _random.Shuffle(choices);
        return choices;
    }

    /// <summary>
    /// Handles running out of time. Listen: the round is missed. Read: the answer is revealed, with no
    /// penalty, as the old "seconds before the answer" setting did. Returns true when it just happened.
    /// </summary>
    public bool CheckTimeout()
    {
        if (Current is null || Stage != Stage.Asking || Remaining is not 0) return false;
        TimedOut = true;
        if (Mode == Mode.Read)
        {
            Stage = Stage.Revealed;
        }
        else
        {
            _watch.Stop();
            Finish(right: false);
        }
        return true;
    }

    /// <summary>Listen, numbers: adds a digit. Returns true when that filled the answer, which is then checked.</summary>
    public bool TypeDigit(char digit)
    {
        if (!CanType || !char.IsAsciiDigit(digit) || CheckTimeout()) return false;
        // A leading 0 is only ever the answer 0, so the next digit replaces it.
        Typed = Typed == "0" ? digit.ToString() : Typed + digit;
        if (Typed.Length < MaxDigits) return false;
        Submit();
        return true;
    }

    public void Backspace()
    {
        if (CanType && Typed.Length > 0) Typed = Typed[..^1];
    }

    public bool CanType => Mode == Mode.Listen && Deck == DeckKind.Numbers && Stage is Stage.Asking or Stage.Retrying
                           && Current is not null;

    /// <summary>
    /// Listen, numbers: checks the typed digits. A first wrong number counts as missed and leaves the
    /// answer hidden for a retry; a right retry ends the round without points.
    /// </summary>
    public void Submit()
    {
        if (!CanType || Typed.Length == 0 || CheckTimeout()) return;
        var right = Typed == Current!.Answer;
        Given = Typed;
        if (Stage == Stage.Asking)
        {
            _watch.Stop();
            if (right)
            {
                Finish(right: true);
                return;
            }
            Finish(right: false);
            Missed = true;
            Stage = Stage.Retrying;
            Typed = "";
        }
        else if (right)
        {
            Right = true;
            Stage = Stage.Done;
        }
        else
        {
            Typed = "";
        }
    }

    /// <summary>Listen, numbers: stops retrying and shows the answer.</summary>
    public void GiveUp()
    {
        if (Stage == Stage.Retrying) Stage = Stage.Done;
    }

    /// <summary>Listen, words: picks a meaning.</summary>
    public void Choose(string meaning)
    {
        if (Mode != Mode.Listen || Deck != DeckKind.Words || Stage != Stage.Asking || Current is null || CheckTimeout())
            return;
        _watch.Stop();
        Given = meaning;
        Finish(right: meaning == Current.Answer);
    }

    /// <summary>Read: shows (and the page plays) the answer.</summary>
    public void Reveal()
    {
        if (Mode == Mode.Read && Stage == Stage.Asking && Current is not null) Stage = Stage.Revealed;
    }

    /// <summary>Read: the player's own verdict, after which the next round starts.</summary>
    public void Grade(bool knewIt)
    {
        if (Mode != Mode.Read || Stage != Stage.Revealed) return;
        _watch.Stop();
        Finish(knewIt);
        NewRound();
    }

    private void Finish(bool right)
    {
        Right = right;
        Stage = Stage.Done;
        if (right)
        {
            LastPoints = ScoreFor(Mode, Elapsed, TimeLimit);
            Points += LastPoints;
            Correct++;
            Streak++;
            BestStreak = Math.Max(BestStreak, Streak);
        }
        else
        {
            LastPoints = 0;
            Wrong++;
            Streak = 0;
        }
    }
}
