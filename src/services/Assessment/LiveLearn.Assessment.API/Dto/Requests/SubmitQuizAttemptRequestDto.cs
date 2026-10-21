namespace LiveLearn.Assessment.API.Dto.Requests;


public record struct SubmitQuizAttemptRequestDto(Guid CourseId, Dictionary<Guid, Guid> Answers);
