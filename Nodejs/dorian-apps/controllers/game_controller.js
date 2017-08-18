"use strict";

const Promise = require('promise');
const UserModel = require('../models/userModel');
const HeroModel = require('../models/heroModel');
const MonsterModel = require('../models/monsterModel');
const SkillModel = require('../models/skillModel');
const MapModel = require('../models/mapModel');
const SocketModel = require('../models/socketModel');
const GameModel = require('../models/gameModel');
const _ = require ('underscore');

require('../Utils/Log')();
require('../models/resultResponse')();
require('../models/broadcastModel')();

var self = this;
var user;
var hero;
var monster;
var skill;
var map;
var socket;
var game;

exports.init = function(data) {
	user 	= new UserModel(data);
	hero 	= new HeroModel(data);
	monster = new MonsterModel (data);
	map 	= new MapModel (data);
	skill	= new SkillModel(data);
	socket 	= new SocketModel();
	game 	= new GameModel();
};

// GENERATE MAP
exports.getGenerateMap = function (request, response) {
	var userToken = request.query.token;
	if (userToken) {
		// AUTHORISE TOKEN
		user.verifyToken (userToken)
		.then ((comp) => {
			hero.findHero(comp.userId)
			// FOUND HERO
			.then ((foundHero) => {
				var charLevel = parseInt (foundHero.characterLevel);
				self.generateMap (charLevel, 
				foundHero.uID, 
				charLevel, charLevel + 3,
				charLevel + 4)
				// GENERATE MAP
				.then ((mapBlocks) => {
					response.end(createResult(1, mapBlocks));
				})
				// GENERATE ERROR
				.catch ((error) => {
					response.end(createErrorCode(1, error));
				});
			})
			// NOT FOUND MONSTER
			.catch ((errorFound) => {
				response.end(createErrorCode(1, errorFound));
			});
		})
		// ERROR
		.catch ((err) => {
			response.end(createErrorCode(1, err));
		});
	} else {
		response.end(createErrorCode(1, "Field is empty."));
	}
}

// CLIENT GENERATE MAP
exports.clientInitMap = function (sender) {
	var userTmpDatabase = sender.userTmpDatabase;
	var clientEvent = 'clientInitMap';
	var clientData = {};
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((foundHero) => {
		map.findMap(userTmpDatabase.userId)
		// FOUND MAP 
		.then((foundMap) => {
			clientData.mapPath = foundMap.mapPath;
			clientData.mapBlocks = foundMap.mapObjects;
			socket.sendMessage (sender, clientEvent, clientData);
		})
		.catch((errorFoundMap) => {
			var charLevel = parseInt (foundHero.characterLevel);
			self.generateMap (charLevel, 
			foundHero.uID, 
			charLevel, charLevel + 3,
			charLevel + 4)
			// GENERATE MAP
			.then ((mapBlocks) => {
				var mapPath = map.mapPathTemplate[(charLevel - 1) % 2];
				map.createMap (userTmpDatabase.userId, mapPath, mapBlocks)
				// CREATED MAP
				.then((created) => {
					clientData.mapPath = mapPath;
					clientData.mapBlocks = mapBlocks;
					socket.sendMessage (sender, clientEvent, clientData);
				})
				.catch ((error) => {
					// SEND CLIENT ERROR
					clientEvent = 'error';
					clientData = { 'error': error };
					socket.sendMessage (sender, clientEvent, clientData);
				});
			})
			// GENERATE ERROR
			.catch ((error) => {
				// SEND CLIENT ERROR
				clientEvent = 'error';
				clientData = { 'error': error };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		});
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		clientEvent = 'error';
		clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});
}

// CLIENT COMPLETED MAP
exports.clientCompletedMap = function (sender) {
	var userTmpDatabase = sender.userTmpDatabase;
	var clientEvent = 'clientChangeTask';
	var clientData = {};
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((foundHero) => {
		var charLevel = parseInt (foundHero.characterLevel);
		// LEVEL UP
		var charLevelUp = charLevel + 1;
		// GENERATE MAP
		self.generateMap (charLevelUp, 
		foundHero.uID, 
		charLevel, charLevel + 3,
		charLevelUp + 4)
		// GENERATE MAP
		.then ((mapBlocks) => {
			var mapPath = map.mapPathTemplate[(charLevel - 1) % 2];
			map.updateMap (userTmpDatabase.userId, mapPath, mapBlocks)
			// UPDATE MAP
			.then((updated) => {
				var currentGold = foundHero.currentGold + 200;
				hero.updateHero (userTmpDatabase.userId, { 
					characterStep: 0, 
					characterLevel: charLevelUp, 
					characterHealthPoint: foundHero.characterMaxHealthPoint, 
					lastUpdateEnergy: new Date(), 
					currentGold: currentGold })
				// UPDATE COMPLETED
				.then ((updated) => {
					foundHero.characterStep = 0;
					foundHero.characterLevel = charLevelUp;
					foundHero.characterHealthPoint = foundHero.characterMaxHealthPoint;
					foundHero.currentGold = currentGold;
					skill.findSkills (foundHero.characterClass, foundHero.characterLevel)
					// FOUND
					.then((skills) => {
						clientData.taskChange   = 'LobbyScene';
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
				// UPDATE ERROR
				.catch ((errorUpdate) => {
					// SEND CLIENT ERROR
					clientEvent = 'error';
					clientData = { 'error': errorUpdate };
					socket.sendMessage (sender, clientEvent, clientData);
				});
			})
			.catch ((error) => {
				// SEND CLIENT ERROR
				clientEvent = 'error';
				clientData = { 'error': error };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		})
		// GENERATE ERROR
		.catch ((error) => {
			// SEND CLIENT ERROR
			clientEvent = 'error';
			clientData = { 'error': error };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		clientEvent = 'error';
		clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});
}

// CLIENT END GAME
exports.clientEndGame = function (sender) {
	var userTmpDatabase = sender.userTmpDatabase;
	var clientEvent = 'clientChangeTask';
	var clientData = {};
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((foundHero) => {
		var charLevel = parseInt (foundHero.characterLevel);
		// GENERATE MAP
		self.generateMap (charLevel, 
		foundHero.uID, 
		charLevel, charLevel + 3,
		charLevel + 4)
		// GENERATE MAP
		.then ((mapBlocks) => {
			var mapPath = map.mapPathTemplate[(charLevel - 1) % 2];
			map.updateMap (userTmpDatabase.userId, mapPath, mapBlocks)
			// UPDATE MAP
			.then((updated) => {
				hero.updateHero (userTmpDatabase.userId, { characterStep: 0, characterHealthPoint: foundHero.characterMaxHealthPoint })
				// UPDATE COMPLETED
				.then ((updated) => {
					foundHero.characterStep = 0;
					foundHero.characterHealthPoint = foundHero.characterMaxHealthPoint;
					skill.findSkills (foundHero.characterClass, foundHero.characterLevel)
					// FOUND
					.then((skills) => {
						clientData.taskChange   = 'LobbyScene';
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
				// UPDATE ERROR
				.catch ((errorUpdate) => {
					// SEND CLIENT ERROR
					clientEvent = 'error';
					clientData = { 'error': errorUpdate };
					socket.sendMessage (sender, clientEvent, clientData);
				});
			})
			.catch ((error) => {
				// SEND CLIENT ERROR
				clientEvent = 'error';
				clientData = { 'error': error };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		})
		// GENERATE ERROR
		.catch ((error) => {
			// SEND CLIENT ERROR
			clientEvent = 'error';
			clientData = { 'error': error };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		clientEvent = 'error';
		clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});
}

// CLIENT ROLL DICE
exports.clientRollDice = function (sender) {
	var userTmpDatabase = sender.userTmpDatabase;
	var randomStep = getRandomInt(1, 6);
	var clientEvent = 'clientReceiveDice';
	var clientData = {};
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((foundHero) => {
		var currentEnergy = foundHero.currentEnergy;
		var maxEnergy = foundHero.maxEnergy;
		var currentGold = foundHero.currentGold;
		var goldPerStep = foundHero.goldPerStep * foundHero.characterLevel * randomStep;
		var totalGold = currentGold + goldPerStep;
		totalGold = totalGold >= foundHero.maxGold ? foundHero.maxGold : totalGold;
		if (currentEnergy > 0) {
			hero.modifyHero (userTmpDatabase.userId, 
			{$inc:{characterStep: randomStep}, 
			$set: {currentEnergy: currentEnergy - 1, 
					lastUpdateEnergy: new Date(),
					currentGold: totalGold}})
			// UPDATE COMPLETED
			.then ((updated) => {
				clientEvent = 'clientReceiveDice';
				clientData = {
					diceStep: randomStep,
					currentEnergy: currentEnergy - 1,
					maxEnergy: maxEnergy
				};
				socket.sendMessage (sender, clientEvent, clientData);
			})
			// UPDATE ERROR
			.catch ((errorUpdate) => {
				// SEND CLIENT ERROR
				clientEvent = 'error';
				clientData = { 'error': errorUpdate };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		} else {
			// SEND CLIENT ERROR
			clientEvent = 'warning';
			clientData = { 'warning': 'Not enough energy. Plz try later.' };
			socket.sendMessage (sender, clientEvent, clientData);
		}
	})
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		clientEvent = 'error';
		clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});
}

// CLIENT SEND CHAT
exports.clientSendChat = function(sender, data, server) {
	var userTmpDatabase = sender.userTmpDatabase;
	var clientEvent = 'clientReceiveChat';
	var clientData = {};
	var chatStr = data.chatString;
	if (chatStr) {
		hero.findHero(userTmpDatabase.userId)
		// FOUND HERO
		.then ((foundHero) => {
			clientData.chatOwner = foundHero.objectName;
			clientData.chatStr = chatStr;
			server.sendTo (sendBroadcastData (clientEvent, clientData));
		})
		// ERROR FIND
		.catch ((errorFind) => {
			// SEND CLIENT ERROR
			clientEvent = 'error';
			clientData = { 'error': errorFind };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	} else {
		// SEND CLIENT ERROR
		clientEvent = 'error';
		clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	}
}

// GENERATE MAP 
exports.generateMap = function (levelMap, exceptID, min, max, size) {
	return new Promise(function (resolve, reject) {
		monster.getMonsters ()
		// FOUND MONSTER
		.then ((foundMonster) => {
			// var levelInt = parseInt (levelMap);
			var mapBlockCount = 20;
			var mapBlocks = [];
			var fitSize = size > mapBlockCount ? mapBlockCount : size;
			hero.findHeroBaseLevel(exceptID, min, max, size)
			// FOUND HERO BASE LEVEL
			.then ((foundHeroes) => {
				var foundIndex = 0;
				// PUSH MAP OBJECTS
				for (var i = 0; i < mapBlockCount; i++) {
					// NEED UPDATE
					if (foundIndex < foundHeroes.length) {
						var heroData = foundHeroes[foundIndex];
						foundIndex++;
						mapBlocks.push (heroData);
					} else {
						var randomIndex = getRandomInt(0, foundMonster.length);
						var monsterData = foundMonster[randomIndex];
						if (typeof monsterData !== 'undefined') {
							monsterData.characterLevel = levelMap;
							mapBlocks.push (monsterData);
						} else {
							mapBlocks.push (null);
						}
					}
				}
				shuffleArray (mapBlocks);
				resolve (mapBlocks);
			})
			// ERROR FOUND
			.catch ((errorFound) => {
				reject (errorFound);
			});
		})
		// NOT FOUND MONSTER
		.catch ((errorFound) => {
			reject (errorFound);
		});
	});
}

function shuffleArray(array) {
    for (var i = array.length - 1; i > 0; i--) {
        var j = Math.floor(Math.random() * (i + 1));
        var temp = array[i];
        array[i] = array[j];
        array[j] = temp;
    }
    return array;
}

















