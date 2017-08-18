"use strict";

const Promise = require('promise');
const UserModel = require('../models/userModel');
const HeroModel = require('../models/heroModel');
const SkillModel = require('../models/skillModel');
const SocketModel = require('../models/socketModel');
const _ = require ('underscore');

require('../Utils/Log')();
require('../models/resultResponse')();
require ('../models/syncWorkerModel')();

var self = this;
var user;
var hero;
var skill;
var socket;

// INIT
exports.init = function(data) {
	user 	= new UserModel(data);
	hero 	= new HeroModel(data);
	skill 	= new SkillModel(data);
	socket 	= new SocketModel();
};

// POST REGISTER
exports.postUserRegister = function (request, response) {
	var uName = request.body.uname;
	var uPass = request.body.upass;
	var uEmail = request.body.uemail;
	var uDisplayName = request.body.udisplayname;
	var uLoginMethod = request.body.uloginmethod;
	user.verifyRegisterInput (uName, uPass, uEmail, uDisplayName)
	// VERIFY COMPLETED
	.then (() => {
		user.UserRegister (uName, uPass, uEmail, uDisplayName, uLoginMethod)
		// REGISTER COMPLETED
		.then((compUser) => {
			response.end(createResult(1, compUser));
		})
		// REGISTER ERROR
		.catch ((errUser) => {
			response.end(createErrorCode(1, errUser));
		});
	})
	// VERIFY ERROR
	.catch ((errVerify) => {
		response.end(createErrorCode(1, errVerify));
	});
};

// LOGIN
exports.postUserLogin = function(request, response){
	var uName = request.body.uname;
	var uPass = request.body.upass;
	user.verifyLoginInput (uName, uPass)
	// VERIFY COMPLETED
	.then (() => {
		// LOGIN COMPLETED
		user.UserLogin (uName, uPass)
		.then ((compUser) => {
			response.end(createResult(1, compUser));
		})
		// LOGIn ERROR
		.catch ((errUser) => {
			response.end(createErrorCode(1, errUser));
		});
	})
	// VERIFY ERROR
	.catch ((error) => {
		response.end(createErrorCode(1, error));
	});
}

// AUTHORISE
exports.getUserAuthorise = function (request, response) {
	var userName = request.query.uname;
	var userToken = request.query.token;
	if (userName && userToken) {
		user.UserAuthorise (userName, userToken)
		// COMPLETE AUTHORISE
		.then ((comp) => {
			response.end(createResult(1, {
							userName: comp.userName,
							loginToken: comp.loginToken
			}));
		})
		// ERROR AUTHORISE
		.catch ((err) => {
			response.end(createErrorCode(1, err));
		});
	} else {
		response.end(createErrorCode(1, "Field is empty."));
	}
}

// HANDLE USER AUTHORISE
exports.handleUserAuthorise = function (userName, userToken) {
	return new Promise(function (resolve, reject) {
		if (userName && userToken) {
			user.UserAuthorise (userName, userToken)
			// COMPLETE AUTHORISE
			.then ((comp) => {
				resolve (comp);
			})
			// ERROR AUTHORISE
			.catch ((err) => {
				reject (err);
			});
		} else {
			reject ("Field is empty.");
		}
	});
};

// TEST DATA SEND
exports.data_test = function(sender, data, allClients){
	print ("Clients " + allClients.length + " server received " + JSON.stringify (data));
};

// SEND SEQUENCE TIME
exports.clientSendPing = function(sender){
	socket.sendMessage (sender, 'serverSendPing', {'msgText': 'This is server send ping.'});
};

// INIT ACCOUNT
exports.clientInitAccount = function (sender, data, clients) {
	var userTmpDatabase = sender.userTmpDatabase;
	var clientEvent = 'clientChangeTask';
	var clientData = {};
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((foundHero) => {
		// SEND CLIENT HERO DATA.
		clientData.taskChange = 'LobbyScene';
		var characterClass = foundHero.characterClass;
		var characterLevel = foundHero.characterLevel;
		var currentTime = new Date();
		var lastTime = new Date (Date.parse (foundHero.lastUpdateEnergy.toISOString()));
		var lostTime = currentTime.getTime() - lastTime.getTime();
		var timerPerEnergy = 10 * 60000; // 10 Minute
		var totalEnergy = Math.floor((lostTime / timerPerEnergy));
		var currentEnergy = foundHero.currentEnergy + totalEnergy;
		currentEnergy = currentEnergy > foundHero.maxEnergy ? foundHero.maxEnergy : currentEnergy;
		var updateTime = totalEnergy >= 1 ? currentTime : lastTime;
		hero.updateHero (userTmpDatabase.userId, {currentEnergy: currentEnergy, lastUpdateEnergy: updateTime})
		// UPDATE ENERGY
		.then ((updated) => {
			skill.findSkills (characterClass, characterLevel)
			// FOUND
			.then((skills) => {
				foundHero.currentEnergy = currentEnergy;
				clientData.heroData 	= foundHero;
				clientData.skillDatas 	= skills;
				socket.sendMessage (sender, clientEvent, clientData);
			})
			// ERROR
			.catch ((error) => {
				// SEND CLIENT ERROR
				clientEvent = 'error';
				clientData = { 'error': error };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		})
		// ERROR ENERGY
		.catch ((error) => {
			// SEND CLIENT ERROR
			clientEvent = 'error';
			clientData = { 'error': error };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT CREATE HERO SCENE.
		clientData.taskChange = 'CreateHeroScene';
		clientData.heroesTemplate = hero.heroesTemplate;
		socket.sendMessage (sender, clientEvent, clientData);
	});
};

exports.clientLeaveGame = function (sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((player) => {
		// SYNC WORKER ROOM INFO
		server.sendTo (sendSyncData ('onServerPlayerLeaveBingoRoom', {
			playerRequest: {
				userId: userTmpDatabase.userId,
				playerId: player.uID,
				objectName: player.objectName,
				objectModel: player.objectModel,
				objectAvatar: player.objectAvatar,
				currentGold: player.currentGold
			}
		}));
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		var clientEvent = 'error';
		var clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});	
}

// CLOSE IMMEDIATELY CONNECT
exports.closeConnect = function(sender, data) {
	socket.sendMessage (sender, 'closeConnect', {'msgText': 'Close connect immediately.'});
	sender.close();	
};

