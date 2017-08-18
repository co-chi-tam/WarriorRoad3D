"use strict";

const Promise = require('promise');
const UserModel = require('../models/userModel');
const HeroModel = require('../models/heroModel');
const SkillModel = require('../models/skillModel');
const SocketModel = require('../models/socketModel');
const _ = require ('underscore');

require('../Utils/Log')();
require('../models/resultResponse')();

var self = this;
var user;
var hero;
var skill;
var socket;

exports.init = function(data) {
	user 	= new UserModel(data);
	hero 	= new HeroModel(data);
	skill 	= new SkillModel(data);
	socket 	= new SocketModel();
};

// CREATE HERO
exports.postCreateHero = function (request, response) {
	var heroType = request.body.htype;
	var heroName = request.body.hname; 
	var userName = request.body.uname;
	var userToken = request.body.token;
	if (heroType && heroName && userName && userToken) {
		// AUTHORISE TOKEN
		user.UserAuthorise (userName, userToken)
		.then ((comp) => {
			hero.createHero(heroType, heroName, comp.userId)
			// CREATE HERO
			.then ((createdHero) => {
				response.end(createResult(1, createdHero));
			})
			// ERROR
			.catch ((err) => {
				response.end(createErrorCode(1, err));
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

// GET HERO
exports.getHero = function (request, response) {
	var userName = request.query.uname;
	var userToken = request.query.token;
	if (userName && userToken) {
		// AUTHORISE TOKEN
		user.UserAuthorise (userName, userToken)
		.then ((comp) => {
			hero.findHero(comp.userId)
			// FOUND HERO
			.then ((compHero) => {
				response.end(createResult(1, compHero));
			})
			// ERROR
			.catch ((err) => {
				response.end(createErrorCode(1, err));
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

// GET HERO BASE LEVEL
exports.getHeroBaseLevel = function(request, response) {
	var exceptID = request.query.expid;
	var minLevel = request.query.min;
	var maxLevel = request.query.max;
	var size = request.query.size;
	if (exceptID && minLevel && maxLevel && size) {
		hero.findHeroBaseLevel(exceptID, parseInt(minLevel), parseInt(maxLevel), parseInt (size))
		// FOUND HERO
		.then ((compHero) => {
			response.end(createResult(1, compHero));
		})
		// ERROR
		.catch ((err) => {
			response.end(createErrorCode(1, err));
		});
	} else {
		response.end(createErrorCode(1, "Field is empty."));
	}
}

// CLIENT CREATE HERO
exports.clientCreateHero = function (sender, data) {
	var heroType = data.htype;
	var heroName = data.hname; 
	var userName = data.uname;
	var userToken = data.token;
	if (heroType && heroName && userName && userToken) {
		var userTmpDatabase = sender.userTmpDatabase;
		hero.createHero(heroType, heroName, userTmpDatabase.userId)
		// CREATE HERO
		.then ((createdHero) => {
			// SEND CLIENT HERO DATA.
			var clientEvent = 'clientChangeTask';
			var clientData = {};
			clientData.taskChange = 'LobbyScene';
			var characterClass = createdHero.characterClass;
			var characterLevel = createdHero.characterLevel;
			skill.findSkills (characterClass, characterLevel)
			// FOUND
			.then((skills) => {
				clientData.heroData 	= createdHero;
				clientData.skillDatas 	= skills;
				socket.sendMessage (sender, clientEvent, clientData);
			})
			// ERROR
			.catch ((error) => {
				// SEND CLIENT ERROR
				var clientEvent = 'error';
				var clientData = { 'error': error };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		})
		// ERROR
		.catch ((err) => {
			var clientEvent = 'error';
			var clientData = { 'error': err };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	} else {
		var clientEvent = 'error';
		var clientData = { 'error': 'Field is empty.' };
		socket.sendMessage (sender, clientEvent, clientData);
	}
};

// CLIENT UPDATE HERO
exports.clientUpdateHero = function(sender, data) {
	var heroHealth = data.hhealth;
	if (heroHealth) {
		var userTmpDatabase = sender.userTmpDatabase;
		var valueUpdate = {
			characterHealthPoint: parseInt (heroHealth)
		}
		hero.updateHero (userTmpDatabase.userId, valueUpdate)
		// UPDATED
		.then ((updatedHero) => {
			// TODO.
			var clientEvent = 'clientUpdated';
			var clientData = { 'msg': 'Update '+ JSON.stringify(valueUpdate) };
			socket.sendMessage (sender, clientEvent, clientData);
		})
		// ERROR
		.catch ((errorHero) => {
			var clientEvent = 'error';
			var clientData = { 'error': errorHero };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	} else {
		var clientEvent = 'error';
		var clientData = { 'error': 'Field is empty.' };
		socket.sendMessage (sender, clientEvent, clientData);
	}
}



















