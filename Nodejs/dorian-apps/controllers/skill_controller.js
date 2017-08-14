"use strict";

const Promise = require('promise');
const SkillModel = require('../models/skillModel');
const HeroModel = require('../models/heroModel');
const SocketModel = require('../models/socketModel');
const _ = require ('underscore');

require('../Utils/Log')();
require('../models/resultResponse')();

var self = this;
var skill;
var hero;
var socket;

exports.init = function(data) {
	skill 	= new SkillModel(data);
	hero 	= new HeroModel(data);
	socket 	= new SocketModel();
};

// GET HERO SKILLS
exports.getSkills = function (request, response) {
	var characterClass = request.query.charclass;
	var characterLevel = request.query.charlevel;
	if (characterClass && characterLevel) {
		skill.findSkills (characterClass, characterLevel)
		// FOUND
		.then((skills) => {
			response.end(createResult(1, skills));
		})
		// ERROR
		.catch ((error) => {
			response.end(createErrorCode(1, error));
		});
	} else {
		response.end(createErrorCode(1, "Field is empty."));
	}
}

// INIT HERO SKILLS
exports.clientInitSkill = function (sender) {
	var userTmpDatabase = sender.userTmpDatabase;
	var clientEvent = 'clientReceiveSkills';
	var clientData = {};
	hero.findHero(userTmpDatabase.userId)
	// FOUND HERO
	.then ((foundHero) => {
		var characterClass = foundHero.characterClass;
		var characterLevel = foundHero.characterLevel;
		skill.findSkills (characterClass, characterLevel)
		// FOUND
		.then((skills) => {
			clientData.skills = skills;
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
	// ERROR FIND
	.catch ((errorFind) => {
		// SEND CLIENT ERROR
		clientEvent = 'error';
		clientData = { 'error': errorFind };
		socket.sendMessage (sender, clientEvent, clientData);
	});
}

// INIT HERO SKILLS
exports.clientSetupSkills = function (sender, data) {
	var userTmpDatabase = sender.userTmpDatabase;
	var clientEvent = 'clientCompletedSetupSkill';
	var clientData = {};
	if (data.skills) {
		var skills = data.skills.split(",");
		console.log (skills[0] + " / " + skills[1]);
		skill.findSkillWithNames (skills)
		// FOUND
		.then ((foundSkills) => {
			// console.log (JSON.stringify (clientData));
			hero.updateHero (userTmpDatabase.userId, { characterSkillSlots: foundSkills })
			// UPDATED
			.then ((updatedHero) => {
				socket.sendMessage (sender, clientEvent, clientData);
			})
			// ERROR
			.catch ((errorHero) => {
				var clientEvent = 'error';
				var clientData = { 'error': errorHero };
				socket.sendMessage (sender, clientEvent, clientData);
			});
		})
		// ERROR
		.catch ((errorFound) => {
			// SEND CLIENT ERROR
			clientEvent = 'error';
			clientData = { 'error': errorFound };
			socket.sendMessage (sender, clientEvent, clientData);
		});
	} else {
		// SEND CLIENT ERROR
		clientEvent = 'error';
		clientData = { 'error': 'Skill data not found.' };
		socket.sendMessage (sender, clientEvent, clientData);
	}
	
}


























