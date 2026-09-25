namespace ExamSystem.Domain;

/// <summary>Multiple choice question (choose one answer) - used by Final and Practical exams.</summary>
public class MCQQuestion : Question
{
    public override QuestionType Type => QuestionType.MCQ;

    public MCQQuestion(string header, string body, int mark, Answer[] answerList, int rightAnswerId)
        : base(header, body, mark, answerList, rightAnswerId)
    {
    }

    public override object Clone() =>
        new MCQQuestion(Header, Body, Mark, CloneAnswers(), RightAnswer.AnswerId);
}
