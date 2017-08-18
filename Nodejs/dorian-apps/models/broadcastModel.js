require('../Utils/Log')();

var broadcastModel = function () {
	// SCOPE
	var self = this;
	// SEND SYNC WORKER 
	this.sendBroadcastData = function (name, data) {
		return {
			broadcastData: {
				eventName: name,
				eventData: data
			}
		};
	}
};

module.exports = broadcastModel;