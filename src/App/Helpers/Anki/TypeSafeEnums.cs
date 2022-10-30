namespace Nihongo.App.Helpers.Anki;

public class Deck
{
    private readonly string name;
    private readonly int value;

    // Decks
    public static readonly Deck KanjiWriting = new Deck(1, "Kanji Writing");

    private Deck(int value, string name)
    {
        this.name = name;
        this.value = value;
    }

    public override string ToString()
    {
        return name;
    }
}

public class NoteType
{
    private readonly string name;
    private readonly int value;

    // Card Types
    public static readonly NoteType KanjiWriting = new NoteType(1, "Kanji Writing");

    private NoteType(int value, string name)
    {
        this.name = name;
        this.value = value;
    }

    public override string ToString()
    {
        return name;
    }
}