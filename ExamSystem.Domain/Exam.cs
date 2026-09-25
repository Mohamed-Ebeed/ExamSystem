namespace ExamSystem.Domain;

/// <summary>Base class with the common attributes of every exam.</summary>
public abstract class Exam : ICloneable, IComparable<Exam>
{
    private const int DefaultTimeMinutes = 60;

    private readonly List<Question> _questions = new();
    // question index -> chosen answer id
    protected readonly Dictionary<int, int> StudentAnswers = new();

    public int TimeMinutes { get; }
    public Subject Subject { get; internal set; }
    public IReadOnlyList<Question> Questions => _questions;
    public int NumberOfQuestions => _questions.Count;
    public int TotalMark => _questions.Sum(q => q.Mark);
    public abstract ExamType Type { get; }

    // Injectable so the exam can be tested without a real console.
    public TextReader Input { get; set; } = Console.In;
    public TextWriter Output { get; set; } = Console.Out;

    // Constructor chaining: (subject) -> (time, subject)
    protected Exam(Subject subject) : this(DefaultTimeMinutes, subject) { }

    protected Exam(int timeMinutes, Subject subject)
    {
        if (timeMinutes <= 0) throw new ArgumentOutOfRangeException(nameof(timeMinutes));
        TimeMinutes = timeMinutes;
        Subject = subject ?? throw new ArgumentNullException(nameof(subject));
    }

    /// <summary>Each exam type decides which question types it accepts.</summary>
    protected abstract bool IsQuestionAllowed(Question question);

    /// <summary>Show the exam; the implementation differs for each exam type.</summary>
    public abstract void ShowExam();

    public abstract object Clone();

    public void AddQuestion(Question question)
    {
        ArgumentNullException.ThrowIfNull(question);
        if (!IsQuestionAllowed(question))
            throw new InvalidOperationException($"{Type} exam does not accept {question.Type} questions.");
        _questions.Add(question);
    }

    public void SubmitAnswer(int questionIndex, int answerId)
    {
        if (questionIndex < 0 || questionIndex >= _questions.Count)
            throw new ArgumentOutOfRangeException(nameof(questionIndex));
        if (!_questions[questionIndex].HasAnswer(answerId))
            throw new ArgumentException("This answer does not belong to the question.", nameof(answerId));
        StudentAnswers[questionIndex] = answerId;
    }

    public int CalculateGrade()
    {
        int grade = 0;
        for (int i = 0; i < _questions.Count; i++)
            if (StudentAnswers.TryGetValue(i, out int id) && _questions[i].IsCorrect(id))
                grade += _questions[i].Mark;
        return grade;
    }

    /// <summary>Prints every question with its answers and reads the student's choice.</summary>
    protected void AskQuestions()
    {
        Output.WriteLine($"Subject: {Subject.SubjectName} | Time: {TimeMinutes} min | Questions: {NumberOfQuestions} | Total mark: {TotalMark}");

        for (int i = 0; i < _questions.Count; i++)
        {
            var q = _questions[i];
            Output.WriteLine();
            Output.WriteLine($"Q{i + 1}) {q}");
            foreach (var a in q.AnswerList)
                Output.WriteLine($"     {a}");

            while (true)
            {
                Output.Write("Your answer (id, Enter to skip): ");
                string? line = Input.ReadLine();
                if (line is null) return;               // input finished
                if (line.Trim().Length == 0) break;     // skipped
                if (int.TryParse(line.Trim(), out int id) && q.HasAnswer(id))
                {
                    StudentAnswers[i] = id;
                    break;
                }
                Output.WriteLine("Invalid answer, try again.");
            }
        }
    }

    protected string StudentAnswerText(int questionIndex) =>
        StudentAnswers.TryGetValue(questionIndex, out int id)
            ? _questions[questionIndex].AnswerList.First(a => a.AnswerId == id).ToString()
            : "(no answer)";

    /// <summary>Copies questions (deep) and the student's answers into a cloned exam.</summary>
    protected void CloneStateTo(Exam target)
    {
        foreach (var q in _questions)
            target._questions.Add((Question)q.Clone());
        foreach (var pair in StudentAnswers)
            target.StudentAnswers[pair.Key] = pair.Value;
    }

    /// <summary>Exams are ordered by total mark, then by time.</summary>
    public int CompareTo(Exam? other)
    {
        if (other is null) return 1;
        int byMark = TotalMark.CompareTo(other.TotalMark);
        return byMark != 0 ? byMark : TimeMinutes.CompareTo(other.TimeMinutes);
    }

    public override string ToString() =>
        $"{Type} exam of '{Subject.SubjectName}' - {NumberOfQuestions} question(s), {TimeMinutes} min, total mark {TotalMark}";
}
