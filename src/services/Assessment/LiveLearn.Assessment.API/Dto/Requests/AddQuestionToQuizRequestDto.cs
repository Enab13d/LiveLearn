namespace LiveLearn.Assessment.API.Dto.Requests;


public record struct AddQuestionToQuizRequest(
    string Text,
    List<string> Answers,
    int CorrectAnswerIdx
);
