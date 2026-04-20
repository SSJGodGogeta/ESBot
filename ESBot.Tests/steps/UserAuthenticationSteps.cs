using Reqnroll;
using FluentAssertions;

namespace ESBot.Tests.steps;

[Binding]
public class UserAuthenticationSteps(
    ScenarioContext scenarioContext)
{
    private readonly UserAuthenticationScenarioState _state = scenarioContext.Get<UserAuthenticationScenarioState>();
    private readonly IAuthenticationUseCase _authenticationUseCase = scenarioContext.Get<IAuthenticationUseCase>();

    [Given("I am on the login page")]
    public void GivenIAmOnTheLoginPage()
    {
        _state.Email = string.Empty;
        _state.Password = string.Empty;
        _state.Result = null;
    }

    [When("I enter valid credentials")]
    public void WhenIEnterValidCredentials()
    {
        _state.Email = "student@student.com";
        _state.Password = "password123";
    }

    [When("I enter invalid credentials")]
    public void WhenIEnterInvalidCredentials()
    {
        _state.Email = "student@example.com";
        _state.Password = "wrong-password";
    }

    [When("I leave the username and password fields empty")]
    public void WhenILeaveTheUsernameAndPasswordFieldsEmpty()
    {
        _state.Email = string.Empty;
        _state.Password = string.Empty;
    }

    [When("I click the login button")]
    public void WhenIClickTheLoginButton()
    {
        _state.Result = _authenticationUseCase.Authenticate(new AuthenticationRequest(_state.Email, _state.Password));
    }

    [Then("I should be redirected to the dashboard")]
    public void ThenIShouldBeRedirectedToTheDashboard()
    {
        _state.Result.Should().NotBeNull();
        _state.Result!.IsSuccessful.Should().BeTrue();
        _state.Result.DestinationPage.Should().Be("dashboard");
    }

    [Then("I should see a welcome message")]
    public void ThenIShouldSeeAWelcomeMessage()
    {
        _state.Result.Should().NotBeNull();
        _state.Result!.Message.Should().Be("Welcome, Student!");
    }

    [Then("I should see an error message indicating invalid credentials")]
    public void ThenIShouldSeeAnErrorMessageIndicatingInvalidCredentials()
    {
        _state.Result.Should().NotBeNull();
        _state.Result!.IsSuccessful.Should().BeFalse();
        _state.Result.Message.Should().Be("invalid credentials");
    }

    [Then("I should see an error message indicating that fields cannot be empty")]
    public void ThenIShouldSeeAnErrorMessageIndicatingThatFieldsCannotBeEmpty()
    {
        _state.Result.Should().NotBeNull();
        _state.Result!.IsSuccessful.Should().BeFalse();
        _state.Result.Message.Should().Be("fields cannot be empty");
    }
}
