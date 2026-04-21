## 1. Differences Between Unit Tests and BDD/Acceptance Tests

When designing a comprehensive test strategy, it is crucial to understand the fundamental differences between unit tests and BDD (Behavior-Driven Development) or acceptance tests in terms of scope, purpose, and execution time:

* **Scope**: 
  * **Unit Tests**: Focus on the smallest testable parts of the application (e.g., individual methods, classes, or entities) in isolation. Dependencies are usually mocked.
  * **BDD/Acceptance Tests**: Focus on the system as a whole (or large subsystems) to verify that end-to-end user workflows work as intended. They test the integration of multiple components.
* **Purpose**:
  * **Unit Tests**: Ensure that the code functions correctly from a technical perspective (doing things right). They are meant for developers to get rapid feedback on local changes to logic.
  * **BDD/Acceptance Tests**: Ensure that the application meets business requirements and user expectations (doing the right thing). They are written in business-readable language (Gherkin format) to bridge the gap between technical and non-technical stakeholders.
* **Execution Time**:
  * **Unit Tests**: Highly granular and extremely fast. Hundreds can execute in milliseconds.
  * **BDD/Acceptance Tests**: Slower by nature, as they often require setting up a larger context, initializing databases, or coordinating multiple services. 

## 2. Recommended Configuration for ESBot's CI Pipeline

In many traditional projects, unit tests and BDD tests are separated: unit tests run on every commit, while slower acceptance tests run on pull requests, nightly, or in a separate CI stage. 

However, for ESBot, our recommendation is to run them together on every commit as part of the primary build process.

Because we have configured our BDD tests (via Reqnroll/xUnit) to execute immediately after `dotnet build`, every code change is instantly validated against business scenarios. This provides immediate, high-confidence feedback that neither technical logic nor business requirements have been broken, reducing the likelihood of regression bugs reaching the `main` branch. If the test suite grows substantially and slows down the build beyond an acceptable threshold (e.g., > 5-10 minutes), the strategy should be reconsidered to move BDD tests to a separate PR check step.

## 3. The Impact of the AI Mockability Requirement

The decision to run BDD tests rapidly and frequently is heavily dependent on the AI Mockability Requirement. 

Testing against a live AI model (like OpenAI or local LLMs) introduces three major problems for a CI pipeline:
1. **Time**: Real AI inference can take seconds per request, turning a suite of 50 BDD test cases into a 2-minute bottleneck.
2. **Cost**: Hitting external APIs charges per token, driving up operational costs drastically with every CI run.
3. **Determinism**: Live AI models may return slightly different phrasing for the same prompt, causing brittle, flaky tests that fail intermittently.

By designing the system to use "mock AI providers" instead of live models during tests, our BDD tests become entirely predictable, instant, and completely free of external dependencies (and cost). 

Because we do not have to wait for a real AI to generate a response, our acceptance test execution time is nearly indistinguishable from regular integration or unit tests. This makes running BDD tests alongside unit tests on every single commit not only feasible but highly efficient.


_Note:_ For this task we used ChatGPT-5.3 in order to improve code quality and understand the usage of ReqnRoll. We also used it to improve our writing for task 5.3 (grammar etc.).
