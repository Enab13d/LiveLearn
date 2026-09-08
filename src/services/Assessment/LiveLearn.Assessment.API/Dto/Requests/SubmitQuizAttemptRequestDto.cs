namespace LiveLearn.Assessment.API.Dto.Requests;


public record struct SubmitQuizAttemptRequestDto(Dictionary<Guid, Guid> Answers);
