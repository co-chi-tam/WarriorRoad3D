
const websocketServer = require ('ws').Server;

require('./Utils/Log')();

const wsRouter = require ('./WSRouter');
const WSPacket = require ('./WSPacket');
const EEnginePacketType = require('./EEnginePacketType');
const ESocketPacketType = require('./ESocketPacketType');
const SocketModel = require('./models/socketModel');
const user = require('./controllers/user_controller');

var wsServer = function (server, database, broadcastEvent) {
	// INIT USER
	user.init(database);
	// SCOPE
	var self = this;
	// CLIENTS
	var clientConnected = [];
	// SOCKET MODEL.
	var socket = new SocketModel();
	// CREATE NEW SOCKET SERVER.
	const wsSV = new websocketServer({ 
		server: server, 
		verifyClient: function (info, cb) {
			var userName = info.req.headers.username;
			var token = info.req.headers.token;
			user.handleUserAuthorise(userName, token)
			// USER AUTHORISE
			.then ((comp) => {
				cb(true);
			})
			// USER NOT FOUND
			.catch ((err) => {
				cb(false, 404, 'Unauthorized');
			});
		}
	});
	// HANDLE CONNECTION
	wsSV.on('connection', function (wsClient, request) {
		print ('New client connection.');
		// SEND WELCOME MESSAGE
		socket.sendMessage (wsClient, 'clientInit', {'msgText': 'This is welcome message.'});
		// CLOSE EVENT
		wsClient.on('close', function(message) {
			self.removeClient(wsClient);
			print ('Client disconnected. ' + clientConnected.length);
		});
		// ADD NEW CLIENT
		wsRouter(wsClient, self, request, database);
		self.addClient(wsClient);
		print ('New client added. Total connect: ' + clientConnected.length);
	});
	// HEADERS
	wsSV.on ('headers', function (headers) {
		// print (headers);
	});
	// ADD CLIENT
	this.addClient = function(client) {
		if (!clientConnected.includes(client)) {
			clientConnected.push (client);
		}
	};
	// REMOVE CLIENT
	this.removeClient = function(client) {
		if (clientConnected.indexOf(client) !== -1) {
			clientConnected.splice (clientConnected.indexOf(client), 1);
		}
	}
	// BROADCAST ALL CLIENT
	this.broadcast = function (clientEvent, clientData) {
		if (broadcastEvent) {
			broadcastEvent ({ eventName: clientEvent, data: clientData });
		}
	}
	// RECEVEIVE BROADCAST
	this.receiveBroadCast = function (clientEvent, clientData) {
		clientConnected.forEach(function (item) {
			socket.sendMessage (item, clientEvent, clientData);
		});
	}
}

module.exports = wsServer;