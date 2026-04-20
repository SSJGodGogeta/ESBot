using Reqnroll;

namespace ESBot.Tests.steps;

[Binding]
public class UserAuthenticationSteps
{
    [Given("I am on the login page")]
    public void GivenIAmOnTheLoginPage()
    {
        throw new PendingStepException();
    }

    [When("I enter valid credentials")]
    public void WhenIEnterValidCredentials()
    {
        throw new PendingStepException();
    }

    [When("I enter invalid credentials")]
    public void WhenIEnterInvalidCredentials()
    {
        throw new PendingStepException();
    }

    [When("I leave the username and password fields empty")]
    public void WhenILeaveTheUsernameAndPasswordFieldsEmpty()
    {
        throw new PendingStepException();
    }

    [When("I click the login button")]
    public void WhenIClickTheLoginButton()
    {
        throw new PendingStepException();
    }

    [Then("I should be redirected to the dashboard")]
    public void ThenIShouldBeRedirectedToTheDashboard()
    {
        throw new PendingStepException();
    }

    [Then("I should see a welcome message")]
    public void ThenIShouldSeeAWelcomeMessage()
    {
        throw new PendingStepException();
    }

    [Then("I should see an error message indicating invalid credentials")]
    public void ThenIShouldSeeAnErrorMessageIndicatingInvalidCredentials()
    {
        throw new PendingStepException();
    }

    [Then("I should see an error message indicating that fields cannot be empty")]
    public void ThenIShouldSeeAnErrorMessageIndicatingThatFieldsCannotBeEmpty()
    {
        throw new PendingStepException();
    }
}
