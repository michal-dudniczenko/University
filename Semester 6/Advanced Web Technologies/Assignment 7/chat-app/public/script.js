let socket = null;

let username;

var messages = document.getElementById('messages');
var form = document.getElementById('form');
var input = document.getElementById('input');

const loginButton = document.getElementById('loginButton');
const loginDiv = document.getElementById('login');
const chatDiv = document.getElementById('chat');
const usernameInput = document.getElementById('usernameInput');
const leaveButton = document.getElementById('leaveButton');
const loginForm = document.getElementById('login-form');

let isTyping = false;

input.addEventListener('input', () => {
    if (input.value.trim() != '') {
        if (isTyping === false) {
            socket.emit('start typing', username);
            isTyping = true;
        }
    } else {
        if (isTyping === true) {
            socket.emit('stop typing', username);
            isTyping = false;
        }
    }
});

loginForm.addEventListener('submit', (e) => {
    e.preventDefault();
    const name = usernameInput.value.trim();
    if (name) {
        username = name;

        loginDiv.classList.remove('show');
        loginDiv.classList.add('hide');

        chatDiv.classList.add('show');
        chatDiv.classList.remove('hide');

        socket = io();

        socket.on('chat message', (obj) => {
            var item = document.createElement('li');
            if (obj.user == username) {
                item.innerHTML = `<div class="dymek niebieski">${dayjs(obj.datetime).format('DD.MM.YYYY HH:mm:ss')} <b>${obj.user}</b><br>${obj.message}</div>`;
                item.classList.add('align-right');
            } else {
                item.innerHTML = `<div class="dymek zielony">${dayjs(obj.datetime).format('DD.MM.YYYY HH:mm:ss')} <b>${obj.user}</b><br>${obj.message}</div>`;
            }
            messages.appendChild(item);
            window.scrollTo(0, document.body.scrollHeight);
        });

        socket.on('userDisconnected', (username) => {
            var item = document.createElement('li');
            item.innerHTML = `<i>${username} left chat.</i>`;
            item.classList.add('center');
            messages.appendChild(item);
            window.scrollTo(0, document.body.scrollHeight);
        });

        socket.on('userConnected', (username) => {
            var item = document.createElement('li');
            item.innerHTML = `<i>${username} joined chat.</i>`;
            item.classList.add('center');
            messages.appendChild(item);
            window.scrollTo(0, document.body.scrollHeight);
        });

        socket.on('start typing', (username) => {
            if (!document.getElementById(`${username}-typing`)){
                var item = document.createElement('li');
                item.innerHTML = `<i>${username} is typing...</i>`;
                item.classList.add('center');
                item.id = `${username}-typing`
                messages.append(item);
            }
        });

        socket.on('stop typing', (username) => {
            document.getElementById(`${username}-typing`)?.remove()
        });

        socket.emit('userConnected', username);
    }
});

leaveButton.addEventListener('click', () => {
    loginDiv.classList.remove('hide');
    loginDiv.classList.add('show');

    chatDiv.classList.add('hide');
    chatDiv.classList.remove('show');

    if (isTyping) {
        isTyping = false;
        socket.emit('stop typing', username);
    }

    socket.emit('userDisconnected', username);
    socket.disconnect();
    socket = null;
})

form.addEventListener('submit', function(e) {
    e.preventDefault();
    if (input.value) {
        socket.emit('chat message', { message: input.value, user: username, datetime: Date.now() });
        isTyping = false;
        socket.emit('stop typing', username);
        input.value = '';
    }
});
