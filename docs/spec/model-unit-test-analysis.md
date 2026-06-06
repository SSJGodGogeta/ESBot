# Exercise 8.1: Unit Test Analysis of Domain Models

## Approach

For this review we looked at the validation attributes, relationships and helper methods in the domain models. Then we compared those rules with the existing unit tests.
We did not use every technique for every class. Just the technique that made the most sense for the shape of the model. Some of them would be:

- **Equivalence Partitioning**
- **Boundary Value Analysis**
- **Decision Table Testing**
- **Experience-Based Testing**


Notice: Codex has been used to summarize and organize the findings, but the actual analysis was done manually by reviewing the code and tests.
It was additionally used at the end to point out some of the uncovered cases that we might have missed.



## Analysis

### UserSession
|                                       | Rule 1  | Rule 2  | Rule 3  | Rule 4  | Rule 5  | Rule 6  |
|---------------------------------------|---------|---------|---------|---------|---------|---------|
| **Conditions**                        |         |         |         |         |         |         |
| User is linked                        | Y       | Y       | Y       | Y       | -       | N       |
| EndedAt is set                        | N       | Y       | Y       | -       | -       | -       |
| EndedAt is after StartedAt            | -       | Y       | N       | -       | -       | -       |
| Message passed to AddMessage          | -       | -       | -       | Valid   | Null    | -       |
| **Actions**                           |         |         |         |         |         |         |
| Save active session successfully      | X       | -       | -       | -       | -       | -       |
| Save ended session successfully       | -       | X       | -       | -       | -       | -       |
| Reject invalid timestamp order        | -       | -       | X       | -       | -       | -       |
| Link message to session on both sides | -       | -       | -       | X       | -       | -       |
| Throw ArgumentNullException           | -       | -       | -       | -       | X       | -       |
| Reject session without user           | -       | -       | -       | -       | -       | X       |

#### Coverage:
| Rule | Covered | Not Covered | Not Enforced |
|------|---------|-------------|--------------|
| 1    | X       | -           | -            |
| 2    | -       | X           | -            |
| 3    | -       | -           | X            |
| 4    | X       | -           | -            |
| 5    | X       | -           | -            |
| 6    | -       | X           | -            |

Note:
Rule 3 (enforcing timestamp order) is not currently enforced by the code, so it cannot be covered by tests until the implementation is updated.

### Message
#### Equivalence Partitioning
- **Content** property:
  - Valid classes: `1 <= length <= 4000`
  - Invalid classes: `length < 1`, `length > 4000` and `Content = null`

- **Role** property:
  - Valid classes: `Role = User || Bot`
  - Invalid classes: any role outside of the valid classes

#### Boundary Value Analysis
- **Content** property:
  - Test boundaries: `1`, `4000`
  - Test values just outside the boundaries: `0`, `4001`

#### Findings:
**Covered cases:**
- valid message content
- missing content
- content just outside the lower boundary `0` (`""`)
- content at maximum length `4000` & above maximum length `4001`
- valid role values and linked session


**Not covered cases:**
- content right at the lower boundary `1`
- whitespace-only content such as `"   "`
- missing session and invalid role values

### QuizRequest
#### Equivalence Partitioning
- **Topic** property:
  - Valid classes: `1 <= length <= 200`
  - Invalid classes: `length < 1`, `length > 200` and `Topic = null`

- **Difficulty** property:
  - Valid classes: `Difficulty = Easy || Medium || Hard`
  - Invalid classes: any difficulty outside the valid classes


#### Boundary Value Analysis
- **Topic** property:
  - Test boundaries: `1`, `200`
  - Test values just outside the boundaries: `0`, `201`


#### Findings:
**Covered cases:**

- valid topic
- missing topic
- topic at maximum length `200`
- topic above maximum length `201`
- valid difficulty and linked quiz items


**Not covered cases:**

- topic right at the lower boundary `1`
- topic just outside the lower boundary `0` (`""`)
- whitespace-only topic such as `"   "`
- missing session and invalid difficulty values



### QuizItem
#### Equivalence Partitioning

- **Question** property:
  - Valid classes: `1 <= length <= 2000`
  - Invalid classes: `length < 1`, `length > 2000` and `Question = null`

- **CorrectAnswer** property:
  - Valid classes: `1 <= length <= 1000`
  - Invalid classes: `length < 1`, `length > 1000` and `CorrectAnswer = null`


#### Boundary Value Analysis
- **Question** property:
  - Test boundaries: `1`, `2000`
  - Test values just outside the boundaries: `0`, `2001`

- **CorrectAnswer** property:
  - Test boundaries: `1`, `1000`
  - Test values just outside the boundaries: `0`, `1001`


#### Findings:
**Covered cases:**
- valid question and answer
- missing question
- missing correct answer
- question above maximum length `2001`
- correct answer above maximum length `1001`
- linked submitted answers


**Not covered cases:**
- question right at the lower boundary `1`
- question just outside the lower boundary `0` (`""`)
- whitespace-only question such as `"   "`
- question at maximum length `2000`
- correct answer right at the lower boundary `1`
- correct answer just outside the lower boundary `0` (`""`)
- whitespace-only correct answer such as `"   "`
- correct answer at maximum length `1000`
- missing quiz request

### SubmittedAnswer
#### Equivalence Partitioning
- **Answer** property:
  - Valid classes: `1 <= length <= 2000`
  - Invalid classes: `length < 1`, `length > 2000` and `Answer = null`

- **EvaluationResult** relationship:
  - Valid classes: answer without an evaluation result and answer with an evaluation result
  - Invalid classes: missing `QuizItem`, because the answer must belong to a quiz item

#### Boundary Value Analysis
- **Answer** property:
  - Test boundaries: `1`, `2000`
  - Test values just outside the boundaries: `0`, `2001`

#### Findings:
**Covered cases:**
- valid answer
- missing answer
- answer at maximum length `2000`
- answer above maximum length `2001`
- linked quiz item
- missing quiz item
- answer without evaluation result
- answer with evaluation result

**Not covered cases:**
- answer right at the lower boundary `1`
- answer just outside the lower boundary `0` (`""`)
- whitespace-only answer such as `"   "`
