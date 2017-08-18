"use strict";

const EventEmitter = require('events');
const WSPacket = require('./WSPacket');
const EEnginePacketType = require('./EEnginePacketType');
const ESocketPacketType = require('./ESocketPacketType');
const crypto = require('crypto');

require('./WSEncode')();
require('./WSDecode')();

class wsEvent extends EventEmitter {}

var wsEventRouter = new wsEvent ();
	
var user 		= require('./controllers/user_controller');
var hero 		= require('./controllers/hero_controller');
var monster 	= require('./controllers/monster_controller');
var game 		= require('./controllers/game_controller');
var skill 		= require('./controllers/skill_controller');
var miniFighting	= require('./controllers/mini_game_fighting_controller');

exports.initRouter = function (wsClient, wsServer, request, database) {
	// INIT
	var userName 	= request.headers['username'];
	var token 		= request.headers['token'];
	user.init		(database);
	hero.init		(database);
	monster.init	(database);
	game.init		(database);
	skill.init		(database);
	miniFighting.init	(database);
	// MESSAGE MIDDLEWARE
	wsClient.on('message', function(message) {
		var decode = Decode (message);
		if (decode.enginePacketType === EEnginePacketType.MESSAGE 
			&& decode.socketPacketType === ESocketPacketType.EVENT) {
			user.handleUserAuthorise(userName, token)
			// USER AUTHORISE
			.then ((comp) => {
				// SocketClient, SocketData, AllClients, Headers
				wsClient.userTmpDatabase = comp;
				wsEventRouter.emit(decode.packetName, wsClient, decode.packetData, wsServer);
			})
			// USER NOT FOUND
			.catch ((err) => {
				wsClient.close();
			});
		}
	});
}

// SERVER EVENT
exports.serverEmitEvent = function (eventName, eventData, wsServer) {
	wsEventRouter.emit(eventName, eventData, wsServer);	
}

// ============= CLIENT EVENTS ============= 
// TEST
wsEventRouter.on ('data_test', 			user.data_test);
// PING
wsEventRouter.on ('clientSendPing', 	user.clientSendPing);
// USER
wsEventRouter.on ('clientInitAccount', 	user.clientInitAccount);
wsEventRouter.on ('clientLeaveGame',	user.clientLeaveGame);
// HERO
wsEventRouter.on ('clientCreateHero', 	hero.clientCreateHero);
wsEventRouter.on ('clientUpdateHero', 	hero.clientUpdateHero);
// GAME
wsEventRouter.on ('clientInitMap', 		game.clientInitMap);
wsEventRouter.on ('clientCompletedMap', game.clientCompletedMap);
wsEventRouter.on ('clientEndGame', 		game.clientEndGame);
wsEventRouter.on ('clientRollDice', 	game.clientRollDice);
wsEventRouter.on ('clientSendChat',     game.clientSendChat);
// SKILL
wsEventRouter.on ('clientInitSkill', 	skill.clientInitSkill);
wsEventRouter.on ('clientSetupSkills', 	skill.clientSetupSkills);
// ROOM
wsEventRouter.on ('clientGetFightingRoomList',			miniFighting.clientGetFightingRoomList);
wsEventRouter.on ('clientRequestJoinFightingRoom',		miniFighting.clientRequestJoinFightingRoom);
wsEventRouter.on ('clientRequestLeaveFightingRoom',		miniFighting.clientRequestLeaveFightingRoom);
wsEventRouter.on ('clientSendDataFightingRoom',			miniFighting.clientSendDataFightingRoom);
// ============= SERVER EVENTS ============= 
// ROOM
wsEventRouter.on ('serverPlayerJoinedFightingRoom', 	miniFighting.serverPlayerJoinedFightingRoom);
wsEventRouter.on ('serverPlayerLeaveFightingRoom', 		miniFighting.serverPlayerLeaveFightingRoom);
wsEventRouter.on ('serverReceiveDataFightingRoom', 		miniFighting.serverReceiveDataFightingRoom);
























