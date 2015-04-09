
var currentUser = null;
var currentUserNick = null;
var onlineUsers = null;

var EVENT_TYPE = chatLib.EVENT_TYPE;

var clientSocket = null;
var socketAddress = chatLib.HOST + ":" + chatLib.PORT;

var socketEventListener = {
	"onSocketConnected" : this.onNotifySocketConnected,
	"onSocketError" : this.onNotifySocketError,
	"onSocketClosed" : this.onNotifySocketClosed,
	"onSocketMessageError" : this.onNotifyServerMessageError,
	"onSocketMessageReceived" : this.onNotifyServerMessageReceived
};

var _super = this;
function onNotifySocketConnected(){
	appendMessage("[网络已连接，登录中...]");
	clientSocket.sendMsg( EVENT_TYPE.LOGIN , currentUserNick);
}
function onNotifySocketError(){
	appendMessage("[网络出错啦，请稍后重试...]");
}
function onNotifySocketClosed(){
	appendMessage("[网络连接已被关闭...]");
}
function onNotifyMessageError(){
	appendMessage("[收到无法解析的数据...]");
}

function onNotifyServerMessageReceived(mData){

	switch (mData.EVENT) {
		case EVENT_TYPE.LOGIN: // 新用户连接
			onNotifyUserLogin(mData);
		break;
		case EVENT_TYPE.LOGOUT: // 用户退出
			onNotifyUserLogout(mData);
		break;
		case EVENT_TYPE.SPEAK: // 用户发言
			onNotifyNewMessage(mData);
		break;
		default:
		break;
	}
	
	/*
	var callBacks = {
		EVENT_TYPE.LOGIN  : this.onNotifyUserLogin,
		EVENT_TYPE.LOGOUT : this.onNotifyUserLogout,
		EVENT_TYPE.SPEAK  : this.onNotifyNewMessage,
		EVENT_TYPE.ERROR  : this.onNotifyMessageError
	};
	this.callBacks[mData.EVENT](mData);
	*/
}


function onNotifyUserLogin(mData){
	if(currentUser == null){
		//如果是当前用户，获取最近的历史消息
		currentUser = mData.user;
		updateHistoryMessage(mData.historyContent);
	}
	appendMessage(formatUserTalkString(mData.user) + "[进入房间]");
	//显示当前所有用户
	onlineUsers = mData.users;
	updateOnlineUser();
}

function onNotifyNewMessage(mData){
	var content = mData.values[0];
	appendMessage(formatUserTalkString(mData.user));
	appendMessage("<span>&nbsp;&nbsp;</span>" + content);
}

function onNotifyUserLogout(mData){
	var livingUser = mData.userInfo;
	if(livingUser.uid == currentUser.uid){
		resetSocketConnection();
		$("#prePage").show();
		$("#mainPage").hide();
	}else{
		//TODO : 删除该用户信息
		//onlineUsers.remove(livingUser.uid);
		updateOnlineUser();
		appendMessage(formatUserTalkString(user) + "[离开房间]");
	}
}

function updateOnlineUser() {
	var html = ["<div>在线用户(" + onlineUsers.length + ")</div>"];
	if (onlineUsers.length > 0) {
		var number = onlineUsers.length;
		for ( var i=0;i<number;i++) {
			html.push("<div>");
			if (onlineUsers[i].uid == currentUser.uid) {
				html.push("<b>" + formatUserString(onlineUsers[i]) + "(我)</b>");
			} else {
				html.push(formatUserString(onlineUsers[i]));
			}
			html.push("</div>");
		}
	}
	$("#onlineUsers").html(html.join(''));
}


function updateHistoryMessage(data) {
	if (data && data.length) {
	    var number = data.length;
	    for ( var i=0;i<number;i++) {
			appendMessage(formatUserTalkHisString(data[i].user, data[i].time));
			appendMessage("<span>&nbsp;&nbsp;</span>" + data[i].content);
	    }
	    appendMessage("<span class='gray'>==================以上为最近的历史消息==================</span>");
	}
}


function appendMessage(msg) {
	$("#talkFrame").append("<div>" + msg + "</div>");
}

function resetSocketConnection() {
	if (clientSocket) {
		clientSocket.terminateSession();
	}
	clientSocket = null;
}

function formatUserString(user) {
	if (!user) {
		return '';
	}
	return user.nick; //+ "<span class='gray'>(" + user.uid + ")</span> ";
}

function formatUserTalkString(user) {
	return formatUserString(user) + new Date().format("hh:mm:ss") + " ";
}

function formatUserTalkHisString(user, time) {
	return formatUserString(user) + new Date(time).format("yyyy-MM-dd hh:mm:ss") + " ";
}

$(document).ready(function() {
	
	if (typeof WebSocket === 'undefined') {
		$("#prePage").hide();
		$("#errorPage").show();
	}
	
	$("#open").click(function(event) {
		currentUserNick = $.trim($("#nickInput").val());
		if ('' == currentUserNick) {
			alert('请先输入昵称');
			return;
		}
		
		$("#prePage").hide();
		$("#mainPage").show();
		$("#onlineUsers").html("");
		$("#talkFrame").html("");
		$("#nickInput").val("");
		
		onlineUserMap = null;
		currentUser = null;
		
		resetSocketConnection();
		
		clientSocket = new clientSocketInstance();
		clientSocket.startSession(socketAddress,socketEventListener);
	});

	$("#message").keyup(function(event) {
		if (13 == event.keyCode) {
			var value = $.trim($("#message").val());
			if (value) {
				$("#message").val('');
				clientSocket.sendMsg( EVENT_TYPE.SPEAK , value);
			}
		}
	});
	
	$("#send").click(function(event) {
		var value = $.trim($("#message").val());
		if (value) {
			$("#message").val('');
			clientSocket.sendMsg( EVENT_TYPE.SPEAK , value);
		}
	});
	
	$("#logout").click(function(event){
		clientSocket.sendMsg(EVENT_TYPE.LOGOUT, currentUser);
	});
	
});
