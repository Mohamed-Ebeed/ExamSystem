namespace ExamSystem.Domain;

public class Subject : ICloneable, IComparable<Subject>
{
    public int SubjectId { get; }
    public string SubjectName { get; }
    public Exam? Exam { get; private set; }

    public Subject(int subjectId, string subjectName)
    {
        if (string.IsNullOrWhiteSpace(subjectName))
            throw new ArgumentException("Subject name is required.", nameof(subjectName));
        SubjectId = subjectId;
        SubjectName = subjectName;
    }

    /// <summary>Creates the exam of this subject (Final or Practical) and fills it with the questions.</summary>
    public Exam CreateExam(ExamType type, int timeMinutes, IEnumerable<Question> questions)
    {
        ArgumentNullException.ThrowIfNull(questions);

        Exam exam = type switch
        {
            ExamType.Final => new FinalExam(timeMinutes, this),
            ExamType.Practical => new PracticalExam(timeMinutes, this),
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        foreach (var question in questions)
            exam.AddQuestion(question);

        Exam = exam;
        return exam;
    }

    public object Clone()
    {
        var copy = new Subject(SubjectId, SubjectName);
        if (Exam is not null)
        {
            copy.Exam = (Exam)Exam.Clone();
            copy.Exam.Subject = copy;   // the cloned exam belongs to the cloned subject
        }
        return copy;
    }

    public int CompareTo(Subject? other) => other is null ? 1 : SubjectId.CompareTo(other.SubjectId);

    public override string ToString() => $"Subject #{SubjectId}: {SubjectName}";
}
