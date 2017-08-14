require('../Utils/Log')();
require('../WSEncode')();
require('../WSDecode')();

const WSPacket = require('../WSPacket');
const EEnginePacketType = require('../EEnginePacketType');
const ESocketPacketType = require('../ESocketPacketType');
const Promise = require('promise');

var paketIndex = -1;

var socketModel = function () {
	// SCOPE
	var self = this;
	// SEND MESSAGE TO CLIENT.
	this.sendMessage = function (sender, eventName, packetData) {
		return new Promise(function (resolve, reject) {
			paketIndex++;
			var packetMsg = new WSPacket(EEnginePacketType.MESSAGE, ESocketPacketType.EVENT, paketIndex, eventName, packetData);
			var msgEncode = Encode(packetMsg);
			sender.send (msgEncode, function (errorSend){
				if (errorSend) {
					reject(errorSend);
				} else {
					resolve (self);
				}
			});
		});
	};
	// SEND ERROR TO CLIENT.
	this.sendError = function(sender, eventName, packetData) {
		return new Promise(function (resolve, reject) {
			paketIndex++;
			var packetMsg = new WSPacket(EEnginePacketType.MESSAGE, ESocketPacketType.ERROR, paketIndex, eventName, packetData);
			var msgEncode = Encode(packetMsg);
			sender.send (msgEncode, function (errorSend){
				if (errorSend) {
					reject(errorSend);
				} else {
					resolve (self);
				}
			});
		});
	};
	
};

module.exports = socketModel;