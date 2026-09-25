namespace ExamSystem.Domain;

/// <summary>Represents one possible answer of a question (AnswerId, AnswerText).</summary>
public class Answer : ICloneable, IComparable<Answer>
{
    private string _answerText = string.Empty;

    public int AnswerId { get; set; }

    public string AnswerText
    {
        get => _answerText;
        set => _answerText = string.IsNullOrWhiteSpace(value)
            ? throw new ArgumentException("Answer text can't be empty.", nameof(value))
            : value;
    }

    // Constructor chaining: default ctor -> full ctor
    public Answer() : this(0, "N/A") { }

    public Answer(int answerId, string answerText)
    {
        AnswerId = answerId;
        AnswerText = answerText;
    }

    public object Clone() => new Answer(AnswerId, AnswerText);

    public int CompareTo(Answer? other) => other is null ? 1 : AnswerId.CompareTo(other.AnswerId);

    public override bool Equals(object? obj) =>
        obj is Answer a && a.AnswerId == AnswerId && a.AnswerText == AnswerText;

    public override int GetHashCode() => HashCode.Combine(AnswerId, AnswerText);

    public override string ToString() => $"{AnswerId}. {AnswerText}";
}
