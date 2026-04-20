using FluentAssertions;
using Reqnroll;

namespace ESBot.Tests.steps;

[Binding]
[Scope(Feature = "Evaluating Quiz Answers")]
public class EvaluateQuizAnswersSteps(ScenarioContext  scenarioContext)
{
    private readonly EvaluateQuizAnswersState _state = scenarioContext.Get<EvaluateQuizAnswersState>();
    private readonly StubAiInferenceClient _aiClient = scenarioContext.Get<StubAiInferenceClient>();
    private readonly IEvaluateQuizAnswersUseCase _useCase = scenarioContext.Get<IEvaluateQuizAnswersUseCase>();
    private readonly Guid _submittedAnswerId = scenarioContext.Get<Guid>("SubmittedAnswerId");
    
    [Given("I have completed a quiz and received my answers")]
    public void GivenIHaveCompletedAQuizAndReceivedMyAnswers()
    {
        _state.Feedback = null;
        _state.ErrorMessage = null;
    }

    [Given("the mock AI provider returns {string}")]
    public void GivenTheMockAiProviderReturns(string p0)
    {
        _aiClient.NextResponse = p0;
    }

    [When("I ask for feedback on a specific question {string}")]
    public void WhenIAskForFeedbackOnASpecificQuestion(string p0)
    {
        var result = _useCase.Evaluate(_submittedAnswerId, p0);
        _state.Feedback = result.Feedback;
        _state.ErrorMessage = result.ErrorMessage;
    }

    [Then("I should receive the feedback {string}")]
    public void ThenIShouldReceiveTheFeedback(string p0)
    {
        _state.Feedback.Should().Be(p0);
    }

    [Then("I should receive an error message indicating that the quiz question cannot be empty")]
    public void ThenIShouldReceiveAnErrorMessageIndicatingThatTheQuizQuestionCannotBeEmpty()
    {
        _state.ErrorMessage.Should().Be("the quiz question cannot be empty");
    }
}
