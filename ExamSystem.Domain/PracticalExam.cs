namespace ExamSystem.Domain;

/// <summary>Practical exam: MCQ only. Shows the right answers after finishing the exam.</summary>
public class PracticalExam : Exam
{
    public override ExamType Type => ExamType.Practical;

    public PracticalExam(Subject subject) : base(subject) { }

    public PracticalExam(int timeMinutes, Subject subject) : base(timeMinutes, subject) { }

    protected override bool IsQuestionAllowed(Question question) => question is MCQQuestion;

    public override void ShowExam()
    {
        AskQuestions();

        Output.WriteLine();
        Output.WriteLine("========== Right Answers ==========");
        for (int i = 0; i < Questions.Count; i++)
        {
            var q = Questions[i];
            Output.WriteLine($"Q{i + 1}) {q.Header}: {q.Body}");
            Output.WriteLine($"     Right answer: {q.RightAnswer}");
        }
    }

    public override object Clone()
    {
        var copy = new PracticalExam(TimeMinutes, Subject);
        CloneStateTo(copy);
        return copy;
    }
}
