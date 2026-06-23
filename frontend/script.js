const apiBaseInput = document.getElementById('apiBaseUrl');
const apiPingButton = document.getElementById('apiPingButton');
const apiStatus = document.getElementById('apiStatus');
const createUserButton = document.getElementById('createUserButton');
const createSessionButton = document.getElementById('createSessionButton');
const chatForm = document.getElementById('chatForm');
const chatInput = document.getElementById('chatInput');
const chatPanel = document.getElementById('chatPanel');
const chatSection = document.getElementById('chatSection');
const sessionSection = document.getElementById('sessionSection');
const sessionsListContainer = document.getElementById('sessionsListContainer');
const sessionsList = document.getElementById('sessionsList');
const userStatus = document.getElementById('userStatus');
const sessionStatus = document.getElementById('sessionStatus');
const chatStatus = document.getElementById('chatStatus');

let currentUser = null;
let currentSession = null;
let sessions = [];

function getId(entity) {
  return entity?.id ?? entity?.Id;
}

function getProperty(entity, prop) {
  if (!entity) return undefined;
  return entity[prop] ?? entity[prop.charAt(0).toUpperCase() + prop.slice(1)];
}

function getApiUrl(path) {
  const base = apiBaseInput.value.trim().replace(/\/$/, '');
  return `${base}${path}`;
}

async function pingApi() {
  const url = getApiUrl('/v1/health');
  apiStatus.textContent = 'Checking API...';
  try {
    const response = await fetch(url);
    if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
    apiStatus.textContent = 'API is reachable.';
  } catch (error) {
    apiStatus.textContent = `API is not reachable: ${error.message}`;
  }
}

async function createUser() {
  const username = document.getElementById('username').value.trim();
  const email = document.getElementById('email').value.trim();
  const password = document.getElementById('password').value;

  if (!username || !email || password.length < 8) {
    userStatus.textContent = 'Please enter valid user data and use a password of at least 8 characters.';
    return;
  }

  createUserButton.disabled = true;
  userStatus.textContent = 'Creating user...';

  try {
    const response = await fetch(getApiUrl('/v1/user'), {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ username, email, password })
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || response.statusText);
    }

    currentUser = await response.json();
    userStatus.textContent = `Created user ${getProperty(currentUser, 'username')} (${getId(currentUser)}).`;
    sessionStatus.textContent = 'Now create a new session linked to this user.';
    await loadSessions();
  } catch (error) {
    userStatus.textContent = `User creation failed: ${error.message}`;
  } finally {
    createUserButton.disabled = false;
  }
}

async function createSession() {
  if (!currentUser) {
    sessionStatus.textContent = 'Create a user before creating a session.';
    return;
  }

  const title = document.getElementById('sessionTitle').value.trim();
  if (!title) {
    sessionStatus.textContent = 'Enter a session title.';
    return;
  }

  createSessionButton.disabled = true;
  sessionStatus.textContent = 'Creating session...';

  try {
    const response = await fetch(getApiUrl('/v1/session'), {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ userId: getId(currentUser), title })
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || response.statusText);
    }

    currentSession = await response.json();
    sessionStatus.textContent = `Session created: ${getProperty(currentSession, 'title')} (${getId(currentSession)}).`;
    chatSection.classList.remove('hidden');
    await loadSessions();
    await loadMessages();
  } catch (error) {
    sessionStatus.textContent = `Session creation failed: ${error.message}`;
  } finally {
    createSessionButton.disabled = false;
  }
}

async function loadSessions() {
  sessionsListContainer.classList.add('hidden');
  sessionsList.innerHTML = '';

  if (!currentUser) return;

  try {
    const response = await fetch(getApiUrl(`/v1/session?UserId=${getId(currentUser)}`));
    if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
    sessions = await response.json();

    if (sessions.length === 0) {
      sessionsListContainer.classList.add('hidden');
      return;
    }

    sessionsListContainer.classList.remove('hidden');
    sessions.forEach(session => {
      const li = document.createElement('li');
      li.textContent = `${getProperty(session, 'title')} (${new Date(getProperty(session, 'startedAt')).toLocaleString()})`;
      li.dataset.sessionId = getId(session);
      if (currentSession && getId(currentSession) === getId(session)) li.classList.add('active');
      li.addEventListener('click', async () => {
        currentSession = session;
        document.getElementById('sessionTitle').value = getProperty(session, 'title');
        chatSection.classList.remove('hidden');
        renderSessions();
        await loadMessages();
      });
      sessionsList.appendChild(li);
    });
  } catch (error) {
    sessionStatus.textContent = `Unable to load sessions: ${error.message}`;
  }
}

function renderSessions() {
  sessionsList.querySelectorAll('li').forEach(li => {
    li.classList.toggle('active', li.dataset.sessionId === getId(currentSession));
  });
}

async function loadMessages() {
  if (!currentSession) return;
  chatPanel.innerHTML = '';
  chatStatus.textContent = 'Loading messages...';

  try {
    const response = await fetch(getApiUrl(`/v1/message?SessionId=${getId(currentSession)}&page=1&pageSize=200`));
    if (!response.ok) throw new Error(`${response.status} ${response.statusText}`);
    const messages = await response.json();
    chatPanel.innerHTML = messages.map(renderMessageHtml).join('');
    chatStatus.textContent = `Loaded ${messages.length} message(s).`;
    chatPanel.scrollTop = chatPanel.scrollHeight;
  } catch (error) {
    chatStatus.textContent = `Unable to load messages: ${error.message}`;
  }
}

function renderMessageHtml(message) {
  const role = message.role ?? message.Role ?? 'Bot';
  const roleClass = role.toLowerCase();
  const text = escapeHtml(message.content ?? message.Content ?? '');
  return `
    <div class="chat-message ${roleClass}">
      <div class="bubble"><strong>${role}</strong><br/>${text}</div>
    </div>`;
}

function escapeHtml(text) {
  return text
    .replace(/&/g, '&amp;')
    .replace(/</g, '&lt;')
    .replace(/>/g, '&gt;')
    .replace(/"/g, '&quot;')
    .replace(/'/g, '&#039;')
    .replace(/\n/g, '<br/>');
}

async function sendMessage(event) {
  event.preventDefault();
  if (!currentSession) {
    chatStatus.textContent = 'Create or select a session first.';
    return;
  }

  const content = chatInput.value.trim();
  if (!content) return;
  chatInput.value = '';
  chatStatus.textContent = 'Sending message...';

  try {
    const response = await fetch(getApiUrl('/v1/message'), {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ sessionId: getId(currentSession), content, role: 'User' })
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(errorText || response.statusText);
    }

    await loadMessages();
    chatStatus.textContent = 'Message saved. Refreshing chat...';
  } catch (error) {
    chatStatus.textContent = `Unable to send message: ${error.message}`;
  }
}

apiPingButton.addEventListener('click', pingApi);
createUserButton.addEventListener('click', createUser);
createSessionButton.addEventListener('click', createSession);
chatForm.addEventListener('submit', sendMessage);
