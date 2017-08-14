require('../Utils/Log')();

module.exports = function() {
	this.createResult = function(resCode, resContent) {
		return JSON.stringify ({
			resultCode: resCode,
			resultContent: resContent
		});
	};
	this.createErrorCode = function(errCode, errContent) {
		return JSON.stringify ({
			errorCode: errCode,
			errorContent: errContent
		});
	}
}