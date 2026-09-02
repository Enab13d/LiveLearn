namespace LiveLearn.Assessment.Application.Dto;


public sealed record QuestionDto(
    Guid Id, 
    string Text, 
    IReadOnlyList<AnswerDto> Answers, 
    Guid? CorrectAnswerId
);
