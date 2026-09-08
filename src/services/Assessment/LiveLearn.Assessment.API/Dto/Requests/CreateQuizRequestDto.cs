namespace LiveLearn.Assessment.API.Dto.Requests;


public readonly record struct CreateQuizRequestDto(string Title, int PassingScore);
