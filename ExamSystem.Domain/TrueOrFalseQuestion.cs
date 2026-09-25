namespace ExamSystem.Domain;

/// <summary>True / False question (Final exam only). Answers are created automatically.</summary>
public class TrueOrFalseQuestion : Question
{
    public override QuestionType Type => QuestionType.TrueOrFalse;

    public TrueOrFalseQuestion(string header, string body, int mark, bool correctAnswerIsTrue)
        : this(header, body, mark, CreateAnswers(), correctAnswerIsTrue ? 1 : 2)
    {
    }

    // Constructor chaining target
    private TrueOrFalseQuestion(string header, string body, int mark, Answer[] answers, int rightAnswerId)
        : base(header, body, mark, answers, rightAnswerId)
    {
    }

    private static Answer[] CreateAnswers() => new[] { new Answer(1, "True"), new Answer(2, "False") };

    public override object Clone() =>
        new TrueOrFalseQuestion(Header, Body, Mark, CloneAnswers(), RightAnswer.AnswerId);
}
