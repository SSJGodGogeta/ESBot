describe('User and Session Creation', () => {
  const apiBaseUrl = Cypress.env('API_BASE_URL') || 'http://localhost:5243';

  beforeEach(() => {
    cy.visit('/');
  });

  it('should create a user and then create a session', () => {
    // Verify the page loaded correctly
    cy.contains('h1', 'ESBot Frontend').should('be.visible');

    // Configure API base URL
    cy.get('#apiBaseUrl').clear().type(apiBaseUrl);

    // Ping the API to verify it's reachable
    cy.get('#apiPingButton').click();
    cy.get('#apiStatus').should('contain', 'API is reachable', { timeout: 10000 });

    // Generate unique user credentials
    const timestamp = Date.now();
    const username = `testuser${timestamp}`;
    const email = `testuser${timestamp}@example.com`;
    const password = 'testpassword123';

    // Fill in user creation form
    cy.get('#username').type(username);
    cy.get('#email').type(email);
    cy.get('#password').type(password);

    // Create user
    cy.get('#createUserButton').click();

    // Verify user was created successfully
    cy.get('#userStatus', { timeout: 10000 })
      .should('contain', `Created user ${username}`)
      .invoke('text')
      .should('match', /\([a-f0-9-]+\)\./); // Verify UUID is present with period

    // Fill in session title
    const sessionTitle = `Test Session ${timestamp}`;
    cy.get('#sessionTitle').type(sessionTitle);

    // Create session
    cy.get('#createSessionButton').click();

    // Verify session was created successfully
    cy.get('#sessionStatus', { timeout: 10000 })
      .should('contain', `Session created: ${sessionTitle}`)
      .invoke('text')
      .should('match', /\([a-f0-9-]+\)\./); // Verify UUID is present with period

    // Verify chat section becomes visible
    cy.get('#chatSection').should('not.have.class', 'hidden');

    // Verify sessions list is visible with the created session
    cy.get('#sessionsListContainer').should('not.have.class', 'hidden');
    cy.get('#sessionsList li').should('contain', sessionTitle);
  });

  it('should not create session without a user', () => {
    // Configure API base URL
    cy.get('#apiBaseUrl').clear().type(apiBaseUrl);

    // Try to create a session without creating a user first
    const sessionTitle = `Test Session ${Date.now()}`;
    cy.get('#sessionTitle').type(sessionTitle);
    cy.get('#createSessionButton').click();

    // Verify error message
    cy.get('#sessionStatus').should('contain', 'Create a user before creating a session');
  });

  it('should validate user input fields', () => {
    // Try to create user with empty fields
    cy.get('#createUserButton').click();
    cy.get('#userStatus').should('contain', 'Please enter valid user data');

    // Try with short password
    cy.get('#username').type('testuser');
    cy.get('#email').type('test@example.com');
    cy.get('#password').type('short'); // Less than 8 characters
    cy.get('#createUserButton').click();
    cy.get('#userStatus').should('contain', 'at least 8 characters');
  });
});
