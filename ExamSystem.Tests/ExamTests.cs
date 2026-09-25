using ExamSystem.Domain;
using Xunit;

namespace ExamSystem.Tests;

public class ExamTests
{
    private static Question Mcq(ExamType t, int mark = 2, int right = 2) =>
        QuestionFactory.CreateMcq(t, "H", "Body", mark, new[] { "a", "b", "c" }, right);

    [Fact]
    public void TrueOrFalse_HasTwoAnswers_AndCorrectRightAnswer()
    {
        var q = new TrueOrFalseQuestion("H", "B", 1, correctAnswerIsTrue: false);
        Assert.Equal(2, q.AnswerList.Length);
        Assert.Equal("False", q.RightAnswer.AnswerText);
    }

    [Fact]
    public void Mcq_WithUnknownRightAnswer_Throws()
    {
        Assert.Throws<ArgumentException>(() =>
            new MCQQuestion("H", "B", 1, new[] { new Answer(1, "a"), new Answer(2, "b") }, 9));
    }

    [Fact]
    public void PracticalExam_RejectsTrueOrFalse()
    {
        var subject = new Subject(1, "C#");
        var exam = subject.CreateExam(ExamType.Practical, 30, new[] { Mcq(ExamType.Practical) });
        var tf = new TrueOrFalseQuestion("H", "B", 1, true);
        Assert.Throws<InvalidOperationException>(() => exam.AddQuestion(tf));
    }

    [Fact]
    public void Factory_RejectsTrueOrFalse_ForPractical()
    {
        Assert.Throws<NotSupportedException>(() =>
            QuestionFactory.CreateTrueOrFalse(ExamType.Practical, "H", "B", 1, true));
    }

    [Fact]
    public void FinalExam_AcceptsBothTypes()
    {
        var subject = new Subject(1, "C#");
        var exam = subject.CreateExam(ExamType.Final, 30, new Question[]
        {
            Mcq(ExamType.Final),
            new TrueOrFalseQuestion("H", "B", 1, true)
        });
        Assert.Equal(2, exam.NumberOfQuestions);
        Assert.Equal(3, exam.TotalMark);
    }

    [Fact]
    public void Subject_CreateExam_AssociatesExamAndSubject()
    {
        var subject = new Subject(1, "C#");
        var exam = subject.CreateExam(ExamType.Final, 30, new[] { Mcq(ExamType.Final) });
        Assert.Same(exam, subject.Exam);
        Assert.Same(subject, exam.Subject);
    }

    [Fact]
    public void CalculateGrade_SumsMarksOfRightAnswers()
    {
        var subject = new Subject(1, "C#");
        var exam = subject.CreateExam(ExamType.Final, 30, new Question[]
        {
            Mcq(ExamType.Final, mark: 2, right: 2),
            new TrueOrFalseQuestion("H", "B", 1, true)
        });
        exam.SubmitAnswer(0, 2);   // right  (+2)
        exam.SubmitAnswer(1, 2);   // wrong  (+0)
        Assert.Equal(2, exam.CalculateGrade());
    }

    [Fact]
    public void FinalExam_ShowExam_PrintsQuestionsAnswersAndGrade()
    {
        var subject = new Subject(1, "C#");
        var exam = subject.CreateExam(ExamType.Final, 30, new[] { Mcq(ExamType.Final, 2, 2) });
        var output = new StringWriter();
        exam.Input = new StringReader("2\n");
        exam.Output = output;

        exam.ShowExam();

        string text = output.ToString();
        Assert.Contains("Your answer: 2. b", text);
        Assert.Contains("Grade: 2 / 2", text);
    }

    [Fact]
    public void PracticalExam_ShowExam_PrintsRightAnswer()
    {
        var subject = new Subject(1, "C#");
        var exam = subject.CreateExam(ExamType.Practical, 30, new[] { Mcq(ExamType.Practical, 2, 3) });
        var output = new StringWriter();
        exam.Input = new StringReader("1\n");
        exam.Output = output;

        exam.ShowExam();

        Assert.Contains("Right answer: 3. c", output.ToString());
    }

    [Fact]
    public void Question_Clone_IsDeepCopy()
    {
        var q = Mcq(ExamType.Final);
        var copy = (Question)q.Clone();
        Assert.NotSame(q, copy);
        Assert.NotSame(q.AnswerList[0], copy.AnswerList[0]);
        Assert.Equal(q.RightAnswer, copy.RightAnswer);
    }

    [Fact]
    public void Exam_Clone_IsIndependent()
    {
        var subject = new Subject(1, "C#");
        var exam = subject.CreateExam(ExamType.Final, 30, new[] { Mcq(ExamType.Final) });
        var copy = (Exam)exam.Clone();
        copy.AddQuestion(Mcq(ExamType.Final));
        Assert.Equal(1, exam.NumberOfQuestions);
        Assert.Equal(2, copy.NumberOfQuestions);
    }

    [Fact]
    public void CompareTo_OrdersByMark()
    {
        var small = Mcq(ExamType.Final, mark: 1);
        var big = Mcq(ExamType.Final, mark: 5);
        Assert.True(small.CompareTo(big) < 0);
        Assert.True(big.CompareTo(small) > 0);
        Assert.Equal(0, small.CompareTo((Question)small.Clone()));
    }
}
