require('./Utils/Log')();

const WSPacket = require('./WSPacket');
const EEnginePacketType = require('./EEnginePacketType');
const ESocketPacketType = require('./ESocketPacketType');

module.exports = function() {
	
	this.Encode = function(packet) {
		var wsEncode = '';
		// ENGINE PACKET
		var engineType = packet.enginePacketType.value;
		wsEncode += engineType;
		if (packet.enginePacketType != EEnginePacketType.MESSAGE.value)
			return wsEncode;
		// SOCKET PACKET
		var socketType = packet.socketPacketType.value;
		wsEncode += socketType;
		// DATA PACKET
		wsEncode += "[\"" + packet.packetName + "\",";
		wsEncode += JSON.stringify (packet.packetData);
		wsEncode += "]";
		return wsEncode;
	};
	
}