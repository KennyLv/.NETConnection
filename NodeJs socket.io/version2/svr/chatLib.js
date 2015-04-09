(function(exports){

	// 事件类型
	exports.EVENT_TYPE = {
	    'LOGIN': 'LOGIN',
	    'LOGOUT': 'LOGOUT',
	    'LIST_USER': 'LIST_USER',
	    'CREATE_ROOM': 'CREATE_ROOM',
	    'JOIN_ROOM': 'JOIN_ROOM',
	    'SPEAK': 'SPEAK',
	    'LIST_HISTORY': 'LIST_HISTORY',
	    'LEAVE_ROOM': 'LEAVE_ROOM',
	    'ERROR': 'ERROR'
	};
	
	exports.CLIENT_EVENT_TYPE = {
	    'LOGIN': 'LOGIN',
	    'LOGOUT': 'LOGOUT',
	    'CREATE_ROOM': 'CREATE_ROOM',
	    'JOIN_ROOM': 'JOIN_ROOM',
	    'SPEAK': 'SPEAK',
	    'LIST_HISTORY' : 'LIST_HISTORY',
	    'LEAVE_ROOM': 'LEAVE_ROOM'
	};
	
	exports.SERVER_NOTIFICATION_TYPE = {
	    'USER_LOGIN': 'USER_LOGIN',
	    'USER_LOGOUT': 'USER_LOGOUT',
	    'LIST_USER': 'LIST_USER',
	    'USER_CREATE_ROOM': 'USER_CREATE_ROOM',
	    'USER_JOIN_ROOM': 'USER_JOIN_ROOM',
	    'USER_MESSAGE': 'USER_MESSAGE',
	    'HISTORY': 'HISTORY',
	    'USER_LEAVE_ROOM': 'USER_LEAVE_ROOM',
	    'SERVER_ERROR': 'SERVER_ERROR'
	};

	// 服务端口
	exports.PORT = 8025;

	// 服务端口
	exports.HOST = "localhost";

	var analyzeMessageData = exports.analyzeMessageData = function(message) {
		try {
			return JSON.parse(message);
		} catch (error) {
			// 收到了非正常格式的数据
			console.log('method:analyzeMsgData,error:' + error);
		}

		return null;
	}

	var getMsgFirstDataValue = exports.getMsgFirstDataValue = function (mData) {
		if (mData && mData.values && mData.values[0]) {
			return mData.values[0];
		}
		return '';
	}

})( (function(){
    if(typeof exports === 'undefined') {
        window.chatLib = {};
        return window.chatLib;
    } else {
        return exports;
    }
})() );