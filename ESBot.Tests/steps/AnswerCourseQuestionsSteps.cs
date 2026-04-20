using Reqnroll;
using FluentAssertions;

namespace ESBot.Tests.steps;

[Binding]
[Scope(Feature = "Answer Course Questions")]
public class AnswerCourseQuestionsSteps(
    ScenarioContext scenarioContext)
{
    private readonly AnswerCourseQuestionsScenarioState _state = scenarioContext.Get<AnswerCourseQuestionsScenarioState>();
    private readonly StubAiInferenceClient _aiClient = scenarioContext.Get<StubAiInferenceClient>();
    private readonly IAskCourseQuestionUseCase _useCase = scenarioContext.Get<IAskCourseQuestionUseCase>();
    private readonly Guid _sessionId = scenarioContext.Get<Guid>("ActiveSessionId");

    [Given("I open a new chat session to ask a question about the course material")]
    public void GivenIOpenANewChatSessionToAskAQuestionAboutTheCourseMaterial()
    {
        _state.Answer = null;
        _state.ErrorMessage = null;
    }

    [Given("the mock AI provider returns {string}")]
    public void GivenTheMockAIProviderReturns(string p0)
    {
        _aiClient.NextResponse = p0;
    }

    [When("I ask a question about {string}")]
    public void WhenIAskAQuestionAbout(string p0)
    {
        var result = _useCase.Ask(_sessionId, p0);
        _state.Answer = result.Answer;
        _state.ErrorMessage = result.ErrorMessage;
    }

    [Then("I should receive the answer {string}")]
    public void ThenIShouldReceiveTheAnswer(string p0)
    {
        _state.Answer.Should().Be(p0);
    }

    [Then("I should receive an error message indicating that the question cannot be empty")]
    public void ThenIShouldReceiveAnErrorMessageIndicatingThatTheQuestionCannotBeEmpty()
    {
        _state.ErrorMessage.Should().Be("the question cannot be empty");
    }
}
