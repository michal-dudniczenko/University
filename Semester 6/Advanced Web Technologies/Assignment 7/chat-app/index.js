const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);
const { Server } = require("socket.io");
const io = new Server(server);

app.use(express.static('public'));

app.get('/', (req, res) => {
  res.sendFile(__dirname + '/index.html');
});

io.on('connection', (socket) => {
    socket.on('userConnected', (username) => {
        socket.broadcast.emit('userConnected', username);
    });

    socket.on('userDisconnected', (username) => {
        socket.broadcast.emit('userDisconnected', username);
    });

    socket.on('chat message', (obj) => {
        io.emit('chat message', obj)
    });

    socket.on('start typing', (username) => {
        socket.broadcast.emit('start typing', username);
    });

    socket.on('stop typing', (username) => {
        socket.broadcast.emit('stop typing', username);
    });
    
});

server.listen(3000, () => {
  console.log('listening on http://localhost:3000');
});