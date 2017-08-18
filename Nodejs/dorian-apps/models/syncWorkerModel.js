require('../Utils/Log')();

var syncWorkerModel = function () {
	// SCOPE
	var self = this;
	// SEND SYNC WORKER 
	this.sendSyncData = function (name, data) {
		return {
			workerSync: {
				syncEvent: name,
				syncData: data
			}
		};
	}
};

module.exports = syncWorkerModel;