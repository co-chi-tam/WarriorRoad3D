"use strict";

const EEnginePacketType = require('./EEnginePacketType');
const ESocketPacketType = require('./ESocketPacketType');

var WSPacket = function (engineType, socketType, id, name, data) {
	// ENGINE PACKET TYPE
	if (typeof engineType !== 'undefined') {
		this.enginePacketType = engineType;
	} else {
		this.enginePacketType = EEnginePacketType.UNKNOWN;
	}
	// SOCKET PACKET TYPE
	if (typeof socketType !== 'undefined') {
		this.socketPacketType = socketType;
	} else {
		this.socketPacketType = ESocketPacketType.UNKNOWN;
	}
	// ID
	if (typeof id !== 'undefined') {
		this.packetId = id;
	} else {
		this.packetId = -1;
	}
	// NAME
	if (typeof name !== 'undefined') {
		this.packetName = name;
	} else {
		this.packetName = 'UNKNOWN';
	}
	// DATA
	if (typeof data !== 'undefined') {
		this.packetData = data;
	} else {
		this.packetData = {};
	}
}

module.exports = WSPacket;