using Reqnroll;

namespace ESBot.Tests.steps;

[Binding]
public class EvaluateQuizAnswersSteps
{
    [Given("I have completed a quiz and received my answers")]
    public void GivenIHaveCompletedAQuizAndReceivedMyAnswers()
    {
        throw new PendingStepException();
    }

    [Given("the mock AI provider returns {string}")]
    public void GivenTheMockAIProviderReturns(string p0)
    {
        throw new PendingStepException();
    }

    [When("I ask for feedback on a specific question {string}")]
    public void WhenIAskForFeedbackOnASpecificQuestion(string p0)
    {
        throw new PendingStepException();
    }

    [Then("I should receive the feedback {string}")]
    public void ThenIShouldReceiveTheFeedback(string p0)
    {
        throw new PendingStepException();
    }

    [Then("I should receive an error message indicating that the quiz question cannot be empty")]
    public void ThenIShouldReceiveAnErrorMessageIndicatingThatTheQuizQuestionCannotBeEmpty()
    {
        throw new PendingStepException();
    }
}
