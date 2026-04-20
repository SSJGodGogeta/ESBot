Feature: Answer Course Questions
    As a student
    I want to be able to ask questions about my course material
    So that I can get help understanding the content

    Scenario: Ask a question about a specific topic
        Given I open a new chat session to ask a question about the course material
        And the mock AI provider returns "Polymorphism means one thing can have different forms or behaviors."
        When I ask a question about "What is polymorphism in object-oriented programming?"
        Then I should receive the answer "Polymorphism means one thing can have different forms or behaviors."

    Scenario: Ask a question without entering a question
        Given I open a new chat session to ask a question about the course material
        When I ask a question about ""
        Then I should receive an error message indicating that the question cannot be empty
