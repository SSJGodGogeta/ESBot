# Exercise 8.1: Unit Test Analysis of Domain Models

## Approach

We looked at the validation attributes and relationships in the domain models
and compared them with the xUnit tests that already exist. For the review we
used:

- **Equivalence classes**: valid input, missing input and invalid relations
- **Boundary values**: empty text, maximum allowed length and one above it
- **State combinations**: for example an active/ended session or an answer
  with/without an evaluation result

## Findings

### UserSession
- Valid combinations: Partly covered. Creating a session and adding a message are tested. A finished session with an EndedAt value is not tested
- Boundaries: There is no test for the timestamp order
- Invalid inputs: A null message is tested, but a session where EndedAt is before StartedAt is not tested

### Message
- Valid combinations: Partly covered. User and bot messages are used in tests, but a message without a session or with an invalid role is not checked
- Boundaries: Empty content, length 4000 and length 4001 are tested
- Invalid inputs: null and empty content are tested, but whitespace-only content is not

### QuizRequest
- Valid combinations: Partly covered. A normal request with linked quiz items is tested, but a missing session or invalid difficulty is not
- Boundaries: Topic lengths 200 and 201 are tested. Empty or whitespace-only topics are not tested
- Invalid inputs: A missing topic is tested, but invalid relations and invalid enum values are not

### QuizItem
- Valid combinations: Partly covered. A valid item and its submitted answer are tested, but an item without a QuizRequest is not
- Boundaries: Inputs above the limits are tested, but the exact valid limits (2000/1000) are not
- Invalid inputs: null question/answer values are tested, while empty or whitespace-only values are not

### SubmittedAnswer
- Valid combinations: Mostly covered for the current model. Tests exist for answers with and without an evaluation result
- Boundaries: Lengths 2000 and 2001 are tested, but empty or whitespace-only answers are not
- Invalid inputs: A missing answer and a missing quiz item are tested; whitespace-only input is not

### EvaluationResult
- Valid combinations: Partly covered. Valid results and optional feedback are tested, but there is no defined rule for valid scores
- Boundaries: Negative scores and scores above 1 are stored successfully, so no score boundary is currently enforced
- Invalid inputs: A result without a submitted answer is tested and fails because of the database relationship
