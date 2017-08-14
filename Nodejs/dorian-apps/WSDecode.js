require('./Utils/Log')();

const WSPacket = require('./WSPacket');
const EEnginePacketType = require('./EEnginePacketType');
const ESocketPacketType = require('./ESocketPacketType');

module.exports = function() {
	
	this.Decode = function(msg) {
		var wsDecode = undefined;
		var engineType = EEnginePacketType.UNKNOWN;
		var socketType = ESocketPacketType.UNKNOWN;
		var packetId = -1;
		var packetName = 'UNKNOWN';
		var packetData = {};
		
		if (msg.length > 0) { // ENGINE PACKET
			var engineTypePart = parseInt (msg.substring (0, 1));
			engineType = EEnginePacketType.get (engineTypePart);
		}
		if (msg.length > 1) { // SOCKET PACKET
			var socketTypePart = parseInt (msg.substring (1, 2));
			socketType = ESocketPacketType.get (socketTypePart);
		}
		if (msg.length > 2) { // DATA PACKET
			var	packetJson = JSON.parse (msg.substring (2, msg.length));
			packetName = packetJson[0];
			packetData = packetJson[1];
		}
		var packet = new WSPacket(engineType, socketType, packetId, packetName, packetData);
		return packet;
	};
	
}