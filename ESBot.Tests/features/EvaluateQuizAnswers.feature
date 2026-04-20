Feature: Evaluating Quiz Answers
    As a student
    I want ESBot to evaluate my quiz answers and provide feedback on why my answers were correct or incorrect
    So that I can understand my mistakes and learn from them

    Scenario: Evaluate a quiz answer with feedback
        Given I have completed a quiz and received my answers
        And the mock AI provider returns "Incorrect. The capital of France is Paris."
        When I ask for feedback on a specific question "What is the capital of France?"
        Then I should receive the feedback "Incorrect. The capital of France is Paris."

    Scenario: Evaluate a quiz answer with an empty question
        Given I have completed a quiz and received my answers
        When I ask for feedback on a specific question ""
        Then I should receive an error message indicating that the quiz question cannot be empty
