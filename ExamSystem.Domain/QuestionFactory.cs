namespace ExamSystem.Domain;

/// <summary>Creates questions and makes sure the type is allowed for the chosen exam.</summary>
public static class QuestionFactory
{
    public static Question CreateTrueOrFalse(ExamType examType, string header, string body, int mark, bool correctAnswerIsTrue)
    {
        EnsureAllowed(examType, QuestionType.TrueOrFalse);
        return new TrueOrFalseQuestion(header, body, mark, correctAnswerIsTrue);
    }

    /// <param name="choices">Answer texts; ids are generated starting from 1.</param>
    /// <param name="rightAnswerId">1-based id of the right choice.</param>
    public static Question CreateMcq(ExamType examType, string header, string body, int mark, string[] choices, int rightAnswerId)
    {
        EnsureAllowed(examType, QuestionType.MCQ);
        var answers = choices.Select((text, i) => new Answer(i + 1, text)).ToArray();
        return new MCQQuestion(header, body, mark, answers, rightAnswerId);
    }

    public static bool IsAllowed(ExamType examType, QuestionType questionType) => examType switch
    {
        ExamType.Final => questionType is QuestionType.TrueOrFalse or QuestionType.MCQ,
        ExamType.Practical => questionType is QuestionType.MCQ,
        _ => false
    };

    private static void EnsureAllowed(ExamType examType, QuestionType questionType)
    {
        if (!IsAllowed(examType, questionType))
            throw new NotSupportedException($"{examType} exam does not accept {questionType} questions.");
    }
}
