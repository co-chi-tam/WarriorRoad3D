"use strict";

var Enum = require('enum');

var EUserAction = new Enum ({
		'UNKNOWN'	: 			-1,
		'CONNECTED': 			0,
		'DISCONNECTED': 		1,
		'PROCESSING_PLAYER_QUEUE': 2,
		'CANCEL_PLAYER_QUEUE': 	3,
		'IN_MINI_BATTLE':		4,
		'ON_ROAD':				5
	});
	
module.exports = EUserAction;