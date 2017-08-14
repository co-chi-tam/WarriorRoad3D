const PORT = process.env.PORT || 5000;
const HOST = process.env.HOST || '127.0.0.1';
const numCPUs = require('os').cpus().length || 4;

const cluster = require('cluster');
const express = require('express');
const socketIO = require('socket.io');

if (cluster.isMaster) {
	for (var i = 0; i < numCPUs; i++) {
		cluster.fork();
	}
	cluster.on('online', function(worker) {
		
	});
	cluster.on ('exit', function (worker, code, signal) {
		cluster.fork();
	});
} else {

	const app = express();

	app.use(express.static(__dirname + '/public'));

	// views is directory for all template files
	app.set('views', __dirname + '/views');
	// app.set('view engine', 'ejs'); 

	app.get('/', function(request, response) {
	  // response.render('pages/index');
	  response.end('index ' + PORT);
	});

	const server = app.listen(app.get('port'), function() {
	  console.log('Node app is running on port', app.get('port'));
	});
	
	const io = socketIO({
		transports: ['websocket'],
	}).listen(server);
	
	io.on('connection', (function(socket) {
		socket.on('disconnect', function() {
			
		});
		socket.emit ('welcome', { message: 'Connection complete', id: socket.id });
		io.sockets.emit ('welcome', { message: 'Welcome new player', id: socket.id });
		console.log ('Someone connected');
	}));
	
	setInterval(() => {
		io.sockets.emit('time', new Date().toTimeString());
		console.log ('ping ...');
		}, 4000);
}

	


