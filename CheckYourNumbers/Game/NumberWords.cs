namespace CheckYourNumbers.Game;

/// <summary>The numbers 0–99 written out, one rule per language.</summary>
public static class NumberWords
{
    private static void Check(int n)
    {
        if (n is < 0 or > 99) throw new ArgumentOutOfRangeException(nameof(n), n, "Only 0–99 are supported.");
    }

    // ---- Danish: enogtyve, halvtreds, syvoghalvfems ----

    private static readonly string[] DaUnits =
    [
        "nul", "en", "to", "tre", "fire", "fem", "seks", "syv", "otte", "ni", "ti", "elleve", "tolv",
        "tretten", "fjorten", "femten", "seksten", "sytten", "atten", "nitten",
    ];

    private static readonly string[] DaTens = ["tyve", "tredive", "fyrre", "halvtreds", "tres", "halvfjerds", "firs", "halvfems"];

    public static string Danish(int n)
    {
        Check(n);
        if (n < 20) return DaUnits[n];
        var (tens, units) = (n / 10, n % 10);
        return units == 0 ? DaTens[tens - 2] : DaUnits[units] + "og" + DaTens[tens - 2];
    }

    // ---- English: forty-seven ----

    private static readonly string[] EnUnits =
    [
        "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve",
        "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen",
    ];

    private static readonly string[] EnTens = ["twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"];

    public static string English(int n)
    {
        Check(n);
        if (n < 20) return EnUnits[n];
        var (tens, units) = (n / 10, n % 10);
        return units == 0 ? EnTens[tens - 2] : EnTens[tens - 2] + "-" + EnUnits[units];
    }

    // ---- Italian: ventuno, ventotto (the tens lose their vowel), ventitré ----

    private static readonly string[] ItUnits =
    [
        "zero", "uno", "due", "tre", "quattro", "cinque", "sei", "sette", "otto", "nove", "dieci", "undici", "dodici",
        "tredici", "quattordici", "quindici", "sedici", "diciassette", "diciotto", "diciannove",
    ];

    private static readonly string[] ItTens = ["venti", "trenta", "quaranta", "cinquanta", "sessanta", "settanta", "ottanta", "novanta"];

    public static string Italian(int n)
    {
        Check(n);
        if (n < 20) return ItUnits[n];
        var (tens, units) = (n / 10, n % 10);
        var ten = ItTens[tens - 2];
        return units switch
        {
            0 => ten,
            1 or 8 => ten[..^1] + ItUnits[units],
            3 => ten + "tré",
            _ => ten + ItUnits[units],
        };
    }

    // ---- German: einundzwanzig, dreißig ----

    private static readonly string[] DeUnits =
    [
        "null", "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun", "zehn", "elf", "zwölf",
        "dreizehn", "vierzehn", "fünfzehn", "sechzehn", "siebzehn", "achtzehn", "neunzehn",
    ];

    private static readonly string[] DeTens = ["zwanzig", "dreißig", "vierzig", "fünfzig", "sechzig", "siebzig", "achtzig", "neunzig"];

    public static string German(int n)
    {
        Check(n);
        if (n < 20) return DeUnits[n];
        var (tens, units) = (n / 10, n % 10);
        if (units == 0) return DeTens[tens - 2];
        return (units == 1 ? "ein" : DeUnits[units]) + "und" + DeTens[tens - 2];
    }

    /// <summary>Swiss Standard German: as German, but Switzerland writes ss for ß (dreissig).</summary>
    public static string SwissGerman(int n) => German(n).Replace("ß", "ss");

    // ---- French: vingt et un, soixante et onze, quatre-vingts, quatre-vingt-dix-neuf ----

    private static readonly string[] FrUnits =
    [
        "zéro", "un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf", "dix", "onze", "douze",
        "treize", "quatorze", "quinze", "seize", "dix-sept", "dix-huit", "dix-neuf",
    ];

    private static readonly string[] FrTens = ["vingt", "trente", "quarante", "cinquante", "soixante"];

    public static string French(int n)
    {
        Check(n);
        if (n < 20) return FrUnits[n];
        if (n < 70)
        {
            var (tens, units) = (n / 10, n % 10);
            var ten = FrTens[tens - 2];
            return units switch { 0 => ten, 1 => ten + " et un", _ => ten + "-" + FrUnits[units] };
        }
        if (n < 80) return n == 71 ? "soixante et onze" : "soixante-" + FrUnits[n - 60];
        if (n == 80) return "quatre-vingts";
        return "quatre-vingt-" + FrUnits[n - 80];
    }

    // ---- Spanish: veintiuno, veintidós, treinta y uno ----

    private static readonly string[] EsUnits =
    [
        "cero", "uno", "dos", "tres", "cuatro", "cinco", "seis", "siete", "ocho", "nueve", "diez", "once", "doce",
        "trece", "catorce", "quince", "dieciséis", "diecisiete", "dieciocho", "diecinueve",
        "veinte", "veintiuno", "veintidós", "veintitrés", "veinticuatro", "veinticinco", "veintiséis", "veintisiete",
        "veintiocho", "veintinueve",
    ];

    private static readonly string[] EsTens = ["treinta", "cuarenta", "cincuenta", "sesenta", "setenta", "ochenta", "noventa"];

    public static string Spanish(int n)
    {
        Check(n);
        if (n < 30) return EsUnits[n];
        var (tens, units) = (n / 10, n % 10);
        return units == 0 ? EsTens[tens - 3] : EsTens[tens - 3] + " y " + EsUnits[units];
    }

    // ---- Dutch: eenentwintig, tweeëntwintig ----

    private static readonly string[] NlUnits =
    [
        "nul", "een", "twee", "drie", "vier", "vijf", "zes", "zeven", "acht", "negen", "tien", "elf", "twaalf",
        "dertien", "veertien", "vijftien", "zestien", "zeventien", "achttien", "negentien",
    ];

    private static readonly string[] NlTens = ["twintig", "dertig", "veertig", "vijftig", "zestig", "zeventig", "tachtig", "negentig"];

    public static string Dutch(int n)
    {
        Check(n);
        if (n < 20) return NlUnits[n];
        var (tens, units) = (n / 10, n % 10);
        if (units == 0) return NlTens[tens - 2];
        var unit = NlUnits[units];
        // A unit ending in e takes a trema on the joining "en": twee + en → tweeën.
        return unit + (unit.EndsWith('e') ? "ën" : "en") + NlTens[tens - 2];
    }
}
