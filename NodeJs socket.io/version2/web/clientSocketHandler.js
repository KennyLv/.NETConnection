
function clientSocketInstance() {

	var _socket = null;
	var _socketChatChannel = null;

	this.terminateSession = function () {
		this._socket.close();
		this._socket = null;
	};

	this.sendMsg = function (event, value) {
		var data = JSON.stringify({
				'EVENT' : event,
				'values' : [value]
			});
		this._socket.emit('message', data);
	};

	this.startSession = function (serverAddress, options) {
		this._socket = io.connect(serverAddress); //'http://localhost:8025'
		this._socketChatChannel = io.connect(serverAddress + '/chat');

		this._socket.on('connect', function () {
			//options.onSocketConnected();
			console.log("in chat connect .....");
		});

		this._socket.on("message", function (message) {
		console.log(message);
		alert(message);
			var mData = null;
			try {
				var mData = JSON.parse(message);
			} catch (error) {
				// 收到了非正常格式的数据
				console.log('method:analyzeMsgData,error:' + error);
				options.onSocketMessageError();
			}
			if (mData && mData.EVENT) {
				options.onSocketMessageReceived(mData);
			}
		});

		this._socket.on("error", function () {
			// 网络连接异常
			console.log('Socket Error:' + error);
			options.onSocketError();
		});

		this._socket.on("close", function () {
			options.onSocketClosed();
		});

		return this._socket;
	};

}

console.log("clientSocket loaded...");
