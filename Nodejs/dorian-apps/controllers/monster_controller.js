"use strict";

const Promise = require('promise');
const UserModel = require('../models/userModel');
const MonsterModel = require('../models/monsterModel');
const SocketModel = require('../models/socketModel');
const _ = require ('underscore');

require('../Utils/Log')();
require('../models/resultResponse')();

var self = this;
var user;
var monster;
var socket;

exports.init = function(data) {
	user = new UserModel(data);
	monster = new MonsterModel(data);
	socket = new SocketModel();
};

// GET MONSTER
exports.getMonster = function (request, response) {
	var monsterName = request.query.mname;
	var userToken = request.query.token;
	if (monsterName && userToken) {
		// AUTHORISE TOKEN
		user.verifyToken (userToken)
		.then ((comp) => {
			monster.findMonster (monsterName)
			// FOUND MONSTER
			.then ((foundMonster) => {
				response.end(createResult(1, foundMonster));
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

// GET MONSTER
exports.getAllMonsters = function (request, response) {
	var userToken = request.query.token;
	if (userToken) {
		// AUTHORISE TOKEN
		user.verifyToken (userToken)
		.then ((comp) => {
			monster.getMonsters ()
			// FOUND MONSTER
			.then ((foundMonster) => {
				response.end(createResult(1, foundMonster));
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