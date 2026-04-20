Feature: User Authentication
    As a student
    I want to be able to authenticate myself
    So that I can access my own chat history and personalized features
    
    Scenario: Successful authentication with valid credentials
        Given I am on the login page
        When I enter valid credentials
        And I click the login button
        Then I should be redirected to the dashboard
        And I should see a welcome message
    
    Scenario: Unsuccessful authentication with invalid credentials
        Given I am on the login page
        When I enter invalid credentials
        And I click the login button
        Then I should see an error message indicating invalid credentials
    
    Scenario: Unsuccessful authentication with empty fields
        Given I am on the login page
        When I leave the username and password fields empty
        And I click the login button
        Then I should see an error message indicating that fields cannot be empty