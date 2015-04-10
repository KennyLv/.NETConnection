(function(exports){

	// 事件类型
	exports.USER_EVENT = {
		'COMMON_EVENT' : {
		    'LOGIN': 'LOGIN',
		    'LOGOUT': 'LOGOUT',
		    'CREATE_ROOM': 'CREATE_ROOM',
		    'ENTER_ROOM': 'ENTER_ROOM',
		    'LEAVE_ROOM': 'LEAVE_ROOM',
		    'PUBLIC_SPEAK': 'PUBLIC_SPEAK',
		    'VIEW_HISTORY': 'VIEW_HISTORY',
		    'UN_PARSEABLE_MESSAGE': 'UN_PARSEABLE_MESSAGE'
		},
		'PRIVATE_CHAT' : {
		    'INVITE': 'INVITE',
		    'BEEN_INVITED': 'BEEN_INVITED',
		    'ACCEPT': 'ACCEPT',
		    'ON_ACCEPTED': 'ON_ACCEPTED',
		    'REFUSE': 'REFUSE',
		    'ON_REFUSED': 'ON_REFUSED',
		    'CHAT_MESSAGE': 'CHAT_MESSAGE' //Suport Image
		},
		'MULTI_PERSON_GAME' : {
		    'AVAILABLE_ROOMS': 'AVAILABLE_ROOMS',
		    'ON_GOING_MESSAGE': {
				'START':'START',
				'IN_TURN':'IN_TURN',
				'END':'END'
		    }
		}
	}

	// 服务端口
	exports.PORT = 8025;
	// 服务端口
	exports.HOST = "10.20.71.62"; //"localhost";

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