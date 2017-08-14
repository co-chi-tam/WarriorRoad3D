"use strict";

var Enum = require('enum');

var ESocketPacketType = new Enum ({
		'UNKNOWN'		: -1,
		'CONNECT'      	:  0,
		'DISCONNECT'	:  1,
		'EVENT'			:  2,
		'ACK'			:  3,
		'ERROR'			:  4,
		'BINARY_EVENT' 	:  5,
		'BINARY_ACK'   	:  6,
		'CONTROL'		:  7
	});

module.exports = ESocketPacketType;

