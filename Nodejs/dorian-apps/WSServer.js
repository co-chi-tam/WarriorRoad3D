
const websocketServer = require ('ws').Server;

require('./Utils/Log')();

const wsRouter = require ('./WSRouter');
const WSPacket = require ('./WSPacket');
const EEnginePacketType = require('./EEnginePacketType');
const ESocketPacketType = require('./ESocketPacketType');
const SocketModel = require('./models/socketModel');
const user = require('./controllers/user_controller');
const _ = require ('underscore');

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
		print ('New client connected.');
		// SEND WELCOME MESSAGE
		socket.sendMessage (wsClient, 'clientInit', {'msgText': 'This is welcome message.'});
		// CLOSE EVENT
		wsClient.on('close', function(message) {
			self.removeClient(wsClient);
			print ('Client disconnected.');
		});
		// ADD NEW CLIENT
		wsRouter.initRouter(wsClient, self, request, database);
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
	// SEND TO MASTER
	this.sendTo = function (data) {
		if (broadcastEvent) {
			broadcastEvent (data);
		}
	}
	// RECEVEIVE BROADCAST
	this.receiveBroadCast = function (broadcastData) {
		var clientEvent = broadcastData.eventName;
		var clientData 	= broadcastData.eventData;
		clientConnected.forEach(function (item) {
			socket.sendMessage (item, clientEvent, clientData);
		});
	}
	// SERVER BROADCAST
	this.syncWorkerData = function (data) {
		var syncEvent 	= data.syncEvent;
		var syncData 	= data.syncData;
		wsRouter.serverEmitEvent(syncEvent, syncData, self);
	}
	// SERVER RECEIVE PRIVATE SYNC DATA
	this.receivePrivate = function (privateData) {
		var id 			= privateData.privateId; 
		var clientEvent = privateData.eventName;
		var clientData 	= privateData.eventData;
		var socketPrivate = _.find (clientConnected, function(item) {
			return item.userTmpDatabase.userId == id;
		});
		if (typeof socketPrivate !== 'undefined') {
			socket.sendMessage (socketPrivate, clientEvent, clientData);
		}
	}
}

module.exports = wsServer;