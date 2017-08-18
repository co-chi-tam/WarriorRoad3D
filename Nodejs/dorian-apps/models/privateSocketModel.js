require('../Utils/Log')();

var privateSocketModel = function () {
	// SCOPE
	var self = this;
	// SEND SYNC WORKER 
	this.sendPrivateData = function (id, name, data) {
		return {
			privateData: {
				privateId: id,
				eventName: name,
				eventData: data
			}
		};
	}
};

module.exports = privateSocketModel;