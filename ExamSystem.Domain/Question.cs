namespace ExamSystem.Domain;

/// <summary>Base class of every question type.</summary>
public abstract class Question : ICloneable, IComparable<Question>
{
    public string Header { get; }
    public string Body { get; }
    public int Mark { get; }
    public Answer[] AnswerList { get; }
    public Answer RightAnswer { get; }

    public abstract QuestionType Type { get; }

    protected Question(string header, string body, int mark, Answer[] answerList, int rightAnswerId)
    {
        if (string.IsNullOrWhiteSpace(header)) throw new ArgumentException("Header is required.", nameof(header));
        if (string.IsNullOrWhiteSpace(body)) throw new ArgumentException("Body is required.", nameof(body));
        if (mark <= 0) throw new ArgumentOutOfRangeException(nameof(mark), "Mark must be greater than zero.");
        if (answerList is null || answerList.Length < 2)
            throw new ArgumentException("A question needs at least two answers.", nameof(answerList));
        if (answerList.Select(a => a.AnswerId).Distinct().Count() != answerList.Length)
            throw new ArgumentException("Answer ids must be unique.", nameof(answerList));

        Header = header;
        Body = body;
        Mark = mark;
        AnswerList = answerList;
        RightAnswer = answerList.FirstOrDefault(a => a.AnswerId == rightAnswerId)
            ?? throw new ArgumentException("The right answer must be one of the answers.", nameof(rightAnswerId));
    }

    public bool HasAnswer(int answerId) => AnswerList.Any(a => a.AnswerId == answerId);

    public bool IsCorrect(int answerId) => RightAnswer.AnswerId == answerId;

    /// <summary>Deep copy (answers are cloned too).</summary>
    public abstract object Clone();

    protected Answer[] CloneAnswers() => AnswerList.Select(a => (Answer)a.Clone()).ToArray();

    /// <summary>Questions are ordered by their mark.</summary>
    public int CompareTo(Question? other) => other is null ? 1 : Mark.CompareTo(other.Mark);

    public override string ToString() => $"[{Type}] {Header}: {Body} ({Mark} mark{(Mark > 1 ? "s" : "")})";
}
